using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using reSreLData.Data;
using reSreLData.Models;
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
builder.Services.AddScoped<GameRepository>();


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

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    // Vérifie si la catégorie "Vidéo" existe
//    var videoCategory = context.Categories.FirstOrDefault(c => c.Nom == "Vidéo");
//    if (videoCategory == null)
//    {
//        videoCategory = new Categorie { Nom = "Vidéo" };
//        context.Categories.Add(videoCategory);
//        context.SaveChanges();
//    }

//    // Liste des vidéos à insérer
//    var ressources = new List<Ressource>
//    {
//        new Ressource
//        {
//            Nom = "What is Civic Engagement?",
//            Type = "Vidéo",
//            Lien = "https://www.youtube.com/watch?v=x6bNwmrBPXI",
//            UserId = 1,
//            Categories = new List<Categorie> { videoCategory }
//        },
//        new Ressource
//        {
//            Nom = "Transform Curiosity into Civic Engagement | TEDxPeabody Park Youth",
//            Type = "Vidéo",
//            Lien = "https://www.youtube.com/watch?v=QQZVA8QY2wI",
//            UserId = 1,
//            Categories = new List<Categorie> { videoCategory }
//        },
//        new Ressource
//        {
//            Nom = "The Power of Citizen Advocacy",
//            Type = "Vidéo",
//            Lien = "https://www.youtube.com/watch?v=ppw5FtkgKME",
//            UserId = 1,
//            Categories = new List<Categorie> { videoCategory }
//        },
//        new Ressource
//        {
//            Nom = "Assessing the State of Citizen-State Relations and Peacebuilding",
//            Type = "Vidéo",
//            Lien = "https://www.youtube.com/watch?v=q3FhGq1EL1U",
//            UserId = 1,
//            Categories = new List<Categorie> { videoCategory }
//        },
//        new Ressource
//        {
//            Nom = "What Are Examples Of Civic Engagement?",
//            Type = "Vidéo",
//            Lien = "https://www.youtube.com/watch?v=UfTeVP-nGgs",
//            UserId = 1,
//            Categories = new List<Categorie> { videoCategory }
//        },
//        new Ressource
//        {
//            Nom = "Citizen Diplomacy",
//            Type = "Vidéo",
//            Lien = "https://www.youtube.com/watch?v=nzyJDF3D5dY",
//            UserId = 1,
//            Categories = new List<Categorie> { videoCategory }
//        },
//        new Ressource
//        {
//            Nom = "In We the People: Civic Engagement in a Constitutional Democracy",
//            Type = "Vidéo",
//            Lien = "https://www.youtube.com/watch?v=O5ng6BVBIkQ",
//            UserId = 1,
//            Categories = new List<Categorie> { videoCategory }
//        },
//        new Ressource
//        {
//            Nom = "Exploring the Connection Between Civic Engagement and Happiness",
//            Type = "Vidéo",
//            Lien = "https://www.youtube.com/watch?v=2tmoR2HcqCc",
//            UserId = 1,
//            Categories = new List<Categorie> { videoCategory }
//        },
//        new Ressource
//        {
//            Nom = "The Intersection of Civic Engagement and Mental Health",
//            Type = "Vidéo",
//            Lien = "https://www.youtube.com/watch?v=AvmMeFXyZHo",
//            UserId = 1,
//            Categories = new List<Categorie> { videoCategory }
//        },
//        new Ressource
//        {
//            Nom = "Citizens of the World: Global Citizenship & U.S. Foreign Relations",
//            Type = "Vidéo",
//            Lien = "https://www.youtube.com/watch?v=1AK4i2Duavs",
//            UserId = 1,
//            Categories = new List<Categorie> { videoCategory }
//        }
//    };

//    // Ajoute les ressources à la base de données
//    context.Ressources.AddRange(ressources);
//    context.SaveChanges();
//    Console.WriteLine("✅ 10 ressources vidéo insérées avec la catégorie 'Vidéo'.");
//}


// ✅ Seed de test : insertion de 10 ressources liées à la catégorie "Article"
//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    var articleCategory = context.Categories.FirstOrDefault(c => c.Nom == "Article");
//    if (articleCategory == null)
//    {
//        articleCategory = new Categorie { Nom = "Article" };
//        context.Categories.Add(articleCategory);
//        context.SaveChanges();
//    }

//    var ressources = new List<Ressource>
//    {
//        new() {
//            Nom = "Comment améliorer les relations entre police et citoyens ?",
//            Type = "Article",
//            Lien = "https://lejournal.cnrs.fr/articles/comment-ameliorer-les-relations-entre-police-et-citoyens",
//            UserId = 1,
//            Categories = new List<Categorie> { articleCategory }
//        },
//        new() {
//            Nom = "Aux côtés des Roms",
//            Type = "Article",
//            Lien = "https://www.lemonde.fr/societe/article/2012/04/30/aux-cotes-des-roms_5990437_3224.html",
//            UserId = 1,
//            Categories = new List<Categorie> { articleCategory }
//        },
//        new() {
//            Nom = "Daniela Festa : En Italie, la lutte pour l'eau bien commun…",
//            Type = "Article",
//            Lien = "https://www.lemonde.fr/series-d-ete/article/2024/07/25/daniela-festa-juriste-et-geographe-en-italie-la-lutte-pour-l-eau-bien-commun-a-mis-en-lumiere-l-importance-du-role-des-citoyens-dans-la-gestion-des-biens-essentiels_6257685_3451060.html",
//            UserId = 1,
//            Categories = new List<Categorie> { articleCategory }
//        },
//        new() {
//            Nom = "La France s'est-elle vraiment droitisée ?",
//            Type = "Article",
//            Lien = "https://www.lemonde.fr/idees/article/2024/10/11/la-france-s-est-elle-vraiment-droitisee-entretien-croise-entre-jean-michel-blanquer-et-vincent-tiberj_6348689_3232.html",
//            UserId = 1,
//            Categories = new List<Categorie> { articleCategory }
//        },
//        new() {
//            Nom = "Hacène Belmessous : La ville inclusive remise en question",
//            Type = "Article",
//            Lien = "https://www.lemonde.fr/societe/article/2024/09/28/hacene-belmessous-chercheur-la-ville-inclusive-que-l-on-nous-vante-se-trouve-peu-a-peu-habillee-d-autres-qualificatifs_6337954_3224.html",
//            UserId = 1,
//            Categories = new List<Categorie> { articleCategory }
//        },
//        new() {
//            Nom = "Les relations ambiguës entre participation et politiques publiques",
//            Type = "Article",
//            Lien = "https://shs.cairn.info/revue-participations-2011-1-page-105.htm",
//            UserId = 1,
//            Categories = new List<Categorie> { articleCategory }
//        },
//        new() {
//            Nom = "Participation des citoyens et démocratie de proximité",
//            Type = "Article",
//            Lien = "https://www.researchgate.net/publication/231858871_Participation_des_citoyens_et_democratie_de_proximite_en_France_La_permanence_d%27un_mythe",
//            UserId = 1,
//            Categories = new List<Categorie> { articleCategory }
//        },
//        new() {
//            Nom = "Les relations entre citoyens et administration en Russie",
//            Type = "Article",
//            Lien = "https://publications.hse.ru/en/articles/198572084",
//            UserId = 1,
//            Categories = new List<Categorie> { articleCategory }
//        },
//        new() {
//            Nom = "Social protection and state‐citizen relations",
//            Type = "Article",
//            Lien = "https://onlinelibrary.wiley.com/doi/10.1111/spol.12959",
//            UserId = 1,
//            Categories = new List<Categorie> { articleCategory }
//        },
//        new() {
//            Nom = "Civicness, social relations and environmental behaviour",
//            Type = "Article",
//            Lien = "https://www.tandfonline.com/doi/abs/10.1080/13608746.2024.2343498",
//            UserId = 1,
//            Categories = new List<Categorie> { articleCategory }
//        }
//    };

//    context.Ressources.AddRange(ressources);
//    context.SaveChanges();
//    Console.WriteLine("✅ 10 ressources ajoutées avec la catégorie 'Article'");
//}

//using (var scope = app.Services.CreateScope())
//{
//    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//    // ✅ Empêche d’insérer en double
//    if (context.Ressources.Any(r => r.UserId == 1 && r.Type == "Podcast"))
//        return;

//    // 📌 Récupère ou crée la catégorie "Podcast"
//    var podcastCategory = context.Categories.FirstOrDefault(c => c.Nom == "Podcast");
//    if (podcastCategory == null)
//    {
//        podcastCategory = new Categorie { Nom = "Podcast" };
//        context.Categories.Add(podcastCategory);
//        context.SaveChanges();
//    }

//    // 🎧 Ressources podcast
//    var podcastRessources = new List<Ressource>
//    {
//        new() {
//            Nom = "Réussir la participation citoyenne – Démocratie participative : gadget ou nécessité ?",
//            Type = "Podcast",
//            Lien = "https://i-cpc.org/type_de_document/podcast/",
//            UserId = 1,
//            Categories = new List<Categorie> { podcastCategory }
//        },
//        new() {
//            Nom = "L'engagement civique a-t-il besoin d'être stimulé ?",
//            Type = "Podcast",
//            Lien = "https://www.radiofrance.fr/franceculture/podcasts/le-temps-du-debat/l-engagement-civique-a-t-il-besoin-d-etre-stimule-4539873",
//            UserId = 1,
//            Categories = new List<Categorie> { podcastCategory }
//        },
//        new() {
//            Nom = "Les citoyens au pouvoir ? - Podcast Le code a changé",
//            Type = "Podcast",
//            Lien = "https://www.franceinter.fr/emissions/le-code-a-change/les-citoyens-au-pouvoir",
//            UserId = 1,
//            Categories = new List<Categorie> { podcastCategory }
//        },
//        new() {
//            Nom = "Changer la société par le dialogue citoyen – Esprit civique",
//            Type = "Podcast",
//            Lien = "https://espritcivique.org/podcast/",
//            UserId = 1,
//            Categories = new List<Categorie> { podcastCategory }
//        },
//        new() {
//            Nom = "Participation citoyenne et transition écologique – AOC podcast",
//            Type = "Podcast",
//            Lien = "https://aoc.media/podcast/2023/04/17/participation-citoyenne-et-transition-ecologique/",
//            UserId = 1,
//            Categories = new List<Categorie> { podcastCategory }
//        },
//        new() {
//            Nom = "Écouter les citoyens, vraiment ? – Le Monde politique",
//            Type = "Podcast",
//            Lien = "https://podcasts.apple.com/fr/podcast/%C3%A9couter-les-citoyens-vraiment/id1462064820?i=1000548693349",
//            UserId = 1,
//            Categories = new List<Categorie> { podcastCategory }
//        },
//        new() {
//            Nom = "Citoyenneté et numérique – podcast Vie Publique",
//            Type = "Podcast",
//            Lien = "https://www.vie-publique.fr/podcast/citoyennete-et-numerique",
//            UserId = 1,
//            Categories = new List<Categorie> { podcastCategory }
//        },
//        new() {
//            Nom = "La démocratie au quotidien – podcast de La Croix",
//            Type = "Podcast",
//            Lien = "https://www.la-croix.com/Podcast-La-democratie-au-quotidien",
//            UserId = 1,
//            Categories = new List<Categorie> { podcastCategory }
//        },
//        new() {
//            Nom = "L'éducation à la citoyenneté – podcast du Cned",
//            Type = "Podcast",
//            Lien = "https://podcast.cned.fr/education-citoyennete/",
//            UserId = 1,
//            Categories = new List<Categorie> { podcastCategory }
//        },
//        new() {
//            Nom = "Des citoyens au service de la ville – podcast Métropoles",
//            Type = "Podcast",
//            Lien = "https://metropolespodcast.fr/episode/citoyens-et-ville/",
//            UserId = 1,
//            Categories = new List<Categorie> { podcastCategory }
//        }
//    };

//    context.Ressources.AddRange(podcastRessources);
//    context.SaveChanges();
//    Console.WriteLine("✅ 10 podcasts insérés avec la catégorie 'Podcast'");
//}


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
