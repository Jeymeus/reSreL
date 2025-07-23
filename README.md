# 📘 Resources Relationnelle

Application de gestion des ressources humaines orientée relationnel, développée en C# avec .NET, conteneurisée avec Docker, et déployée sur Azure. Le projet intègre une démarche DevSecOps avec SonarQube, Snyk, GitHub Actions, et respecte les exigences RGPD.

---

## 🚀 Fonctionnalités

- Gestion des utilisateurs et relations (hiérarchie, collaboration)
- API REST sécurisée en .NET
- Base de données Azure SQL
- Authentification (à personnaliser selon le projet)
- Tableau de bord dynamique (si frontend)

---

## ⚙️ Architecture technique

- **Langage** : C#
- **Framework** : .NET 7/8
- **Base de données** : Azure SQL
- **Conteneurisation** : Docker
- **CI/CD** : GitHub Actions
- **Plateforme** : Azure App Service

> Voir le dossier `/docs` pour les diagrammes techniques.

---

## 🐳 Déploiement avec Docker & Azure

### Docker

docker build -t resources-relationnelle .
docker run -d -p 8080:80 resources-relationnelle

## Azure (via GitHub Actions)

Docker image push vers Azure Container Registry

Déploiement automatique sur Azure Web App for Containers

Voir .github/workflows/deploy.yml pour les détails.

## ✅ Qualité & Sécurité

SonarQube
Analyse statique intégrée au pipeline

Détection des bugs, code smells, duplications

Snyk
Scan de vulnérabilités sur :

Dépendances NuGet

Images Docker

## 🔁 GitHub & Gestion de projet

Code source : organisation en branches (main, dev, feature/*)

Issues : gestion des tâches, bugs, améliorations

Projets : suivi visuel (kanban)

Chaque issue est liée à une PR pour assurer la traçabilité.

## 🛡️ RGPD & Sécurité

Chiffrement des données sensibles

Consentement utilisateur (si applicable)

Journalisation et politique de conservation des données

Masquage et anonymisation possibles

## 🧠 Veille technologique

Suivi régulier des mises à jour :

.NET, Docker, Azure, Snyk

Sources :

Microsoft Learn, Snyk Blog, GitHub Security Advisories

## 📂 Structure du projet

/ResourcesRelationnelle
│
├── Controllers/
├── Models/
├── Services/
├── Data/
├── Dockerfile
├── sonar-project.properties
├── .github/workflows/
└── README.md

🧪 Lancer en local (dev)
dotnet restore
dotnet build
dotnet run
Naviguez sur http://localhost:5000 une fois l'application démarrée.

