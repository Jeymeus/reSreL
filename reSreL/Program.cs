using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using reSreLData.Data;
using reSreLData.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Connexion à la base de données (EF Core)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injection des repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RessourceRepository>();
builder.Services.AddScoped<CategorieRepository>();
builder.Services.AddScoped<CommentaireRepository>();

// Ajout des contrôleurs + vues (MVC)
builder.Services.AddControllersWithViews();

// Configuration de l'authentification par cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login";
        options.LogoutPath = "/Users/Logout";
    });

var app = builder.Build();

// Middleware d'erreur (prod)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Ordre CORRECT des middlewares :
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); // 👉 Obligatoire avant UseAuthentication

app.UseAuthentication(); // 👉 Doit être après UseRouting
app.UseAuthorization();  // 👉 Toujours après UseAuthentication

// Routing MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
