using Xunit;
using Microsoft.AspNetCore.Mvc;
using reSreL.Controllers;
using reSreLData.Models;
using reSreLData.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using reSreLData.Data;
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Routing;

public class UsersControllerTests
{
    private readonly AppDbContext _context;
    private readonly UsersController _controller;
    private readonly UserRepository _userRepo;

    public UsersControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _userRepo = new UserRepository(_context);
        _controller = new UsersController(_userRepo);

        SetupHttpContextWithServices();
        SetupTempData();
    }

    private void SetupHttpContextWithServices()
    {
        var mockAuthService = new Mock<IAuthenticationService>();
        mockAuthService
            .Setup(a => a.SignInAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        mockAuthService
            .Setup(a => a.SignOutAsync(
                It.IsAny<HttpContext>(),
                CookieAuthenticationDefaults.AuthenticationScheme,
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);

        var mockUrlHelper = new Mock<IUrlHelper>();
        mockUrlHelper
            .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns("fake-url");

        var mockUrlHelperFactory = new Mock<IUrlHelperFactory>();
        mockUrlHelperFactory
            .Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
            .Returns(mockUrlHelper.Object);

        var services = new ServiceCollection()
            .AddSingleton(mockAuthService.Object)
            .AddSingleton<IUrlHelperFactory>(mockUrlHelperFactory.Object)
            .BuildServiceProvider();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services
        };

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _controller.Url = mockUrlHelper.Object;
    }

    private void SetupTempData()
    {
        var tempDataProviderMock = new Mock<ITempDataDictionaryFactory>();
        var tempDataDictionary = new TempDataDictionary(
            _controller.ControllerContext.HttpContext,
            Mock.Of<ITempDataProvider>()
        );

        tempDataProviderMock
            .Setup(factory => factory.GetTempData(It.IsAny<HttpContext>()))
            .Returns(tempDataDictionary);

        _controller.TempData = tempDataDictionary;
    }

    private void SetUserClaim(int userId)
    {
        var claims = new[] { new Claim("UserId", userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext.HttpContext.User = principal;
    }

    // Teste que si l'utilisateur fournit des identifiants valides, il est redirigé vers Home/Index
    [Fact]
    public async Task Login_Post_ValidUser_RedirectsToHomeIndex()
    {
        // Arrange : créer un utilisateur actif avec un mot de passe connu
        var email = "test@example.com";
        var password = "password";
        var user = new User { Nom = "Test", Email = email, MotDePasse = password, Actif = true, Role = "User" };
        await _userRepo.CreateAsync(user);

        // Act : tenter de se connecter
        var result = await _controller.Login(email, password);

        // Assert : vérifier la redirection correcte
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);
    }

    // Teste que si les identifiants sont invalides, la vue de login est renvoyée avec des erreurs
    [Fact]
    public async Task Login_Post_InvalidUser_ReturnsViewWithError()
    {
        // Act : tenter de se connecter avec des mauvais identifiants
        var result = await _controller.Login("bad@example.com", "badpass");

        // Assert : on reste sur la vue de login avec des erreurs de modèle
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
        Assert.True(_controller.ModelState.ErrorCount > 0);
    }

    // Teste que l'action Logout redirige correctement vers la page de connexion
    [Fact]
    public async Task Logout_RedirectsToLogin()
    {
        // Act : déconnexion
        var result = await _controller.Logout();

        // Assert : redirection vers Users/Login
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
        Assert.Equal("Users", redirect.ControllerName);
    }

    // Teste que lorsqu'un utilisateur connecté appelle Profil, ses données sont affichées
    [Fact]
    public async Task Profil_WithUserClaim_ReturnsViewWithUser()
    {
        // Arrange : créer un utilisateur, simuler son authentification
        var user = new User { Nom = "Test", Email = "u@example.com", MotDePasse = "pass", Actif = true };
        var created = await _userRepo.CreateAsync(user);
        SetUserClaim(created.Id);

        // Act : appel à l'action Profil
        var result = await _controller.Profil();

        // Assert : les données retournées sont celles de l'utilisateur connecté
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(created, view.Model);
    }

    // Teste que si l'utilisateur n'est pas connecté, il est redirigé vers Login depuis Profil
    [Fact]
    public async Task Profil_WithoutUserClaim_RedirectsToLogin()
    {
        // Act : appel de Profil sans authentification
        var result = await _controller.Profil();

        // Assert : redirection vers la page de connexion
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Login", redirect.ActionName);
    }

    // Teste que l'action GET EditProfil renvoie les infos de l'utilisateur connecté
    [Fact]
    public async Task EditProfil_Get_WithUserClaim_ReturnsViewWithUser()
    {
        // Arrange : créer un utilisateur, simuler l'authentification
        var user = new User { Nom = "Test", Email = "e@e.com", MotDePasse = "pass", Actif = true };
        var created = await _userRepo.CreateAsync(user);
        SetUserClaim(created.Id);

        // Act : appel GET EditProfil
        var result = await _controller.EditProfil();

        // Assert : la vue contient les données de l'utilisateur connecté
        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(created, view.Model);
    }

    // Teste que l'action POST EditProfil met à jour les infos utilisateur et redirige vers Profil
    [Fact]
    public async Task EditProfil_Post_ValidUpdate_RedirectsToProfil()
    {
        // Arrange : créer un utilisateur, puis préparer une version modifiée
        var original = new User
        {
            Nom = "Old",
            Prenom = "Name",
            Email = "old@mail.com",
            MotDePasse = "pass",
            Actif = true
        };
        var created = await _userRepo.CreateAsync(original);
        SetUserClaim(created.Id);

        var updated = new User
        {
            Id = created.Id,
            Nom = "New",
            Prenom = "Name",
            Email = "new@mail.com"
        };

        // Act : envoi de la modification
        var result = await _controller.EditProfil(updated);

        // Assert : redirection vers Profil et données mises à jour en base
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Profil", redirect.ActionName);

        var inDb = await _userRepo.GetByIdAsync(created.Id);
        Assert.Equal("New", inDb?.Nom);
        Assert.Equal("new@mail.com", inDb?.Email);
    }

    // Teste que si les deux nouveaux mots de passe ne correspondent pas, une erreur est affichée
    [Fact]
    public async Task ChangePassword_Post_PasswordsDontMatch_ReturnsViewWithError()
    {
        // Arrange : créer un utilisateur et simuler l'authentification
        var user = new User { Nom = "Test", Email = "pw@test.com", MotDePasse = "old", Actif = true };
        var created = await _userRepo.CreateAsync(user);
        SetUserClaim(created.Id);

        // Act : tenter de changer de mot de passe avec deux valeurs différentes
        var result = await _controller.ChangePassword("old", "new1", "new2");

        // Assert : une erreur est ajoutée au ModelState, on reste sur la vue
        var view = Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }
}