# Personel Website

> A robust, Clean Architecture-based CMS built with modern .NET and PostgreSQL.

---

## 🔹 Overview

**Personel Website** is a modular web application designed to manage personal portfolios and content with enterprise-grade architectural standards. Built on the principles of **Clean Architecture**, it ensures a strict separation of concerns, making the system highly maintainable, testable, and scalable.

This project addresses the limitations of rigid, monolithic personal site templates by offering a flexible backend capable of handling **complex identity management**, **dynamic localization**, and **granular configuration**, all wrapped in a professional-grade admin interface.

It is ideal for:

* .NET developers seeking a real-world reference architecture
* Professionals needing a self-hosted, customizable CMS for personal branding

---

## 🔹 Key Features

* **Clean Architecture**
  Strict separation into Domain, Application, Infrastructure, and Presentation layers to enforce dependency rules.

* **Role-Based Identity (RBAC)**
  Comprehensive user and role management built on **ASP.NET Core Identity**.

* **Dynamic Localization**
  Database-driven localization allowing real-time language management without redeployment.

* **Robust Admin Panel**
  Integrated **Metronic**-based administrative dashboard for managing content, users, and system settings.

* **Modular Design**
  Feature-based organization (e.g., Auth, Language, Configuration) for high extensibility.

* **Docker Ready**
  Fully containerized setup including the application and PostgreSQL database.

---

## 🔹 Tech Stack

### Core

* **Language:** C#
* **Framework:** .NET (ASP.NET Core)
* **Database:** PostgreSQL
* **ORM:** Entity Framework Core (Npgsql)

### Architecture & Libraries

* **Mapping:** AutoMapper
* **Validation:** FluentValidation
* **Dependency Injection:** Native ASP.NET Core DI

### Frontend

* **Rendering Engine:** ASP.NET Core MVC (Razor Views)
* **Admin Theme:** Metronic (HTML5, CSS3, JavaScript)
* **UI Libraries:** jQuery, Bootstrap, Flatpickr, Select2

### DevOps

* **Containerization:** Docker, Docker Compose

---

## 🔹 Architecture & Design

The solution follows the **Clean Architecture (Onion Architecture)** pattern:

```text
src/
├── Core/
│   ├── PW.Domain/          # Enterprise logic, Entities, Enums (No dependencies)
│   └── PW.Application/     # Application logic, DTOs, Interfaces (Depends on Domain)
├── Infrastructure/
│   ├── PW.Persistence/     # DbContexts, Repositories, Migrations
│   ├── PW.Identity/        # Identity services, Auth context, user logic
│   └── PW.Services/        # External services (File storage, Email, etc.)
└── Presentation/
    └── PW.Web/             # MVC app, Controllers, Views, API endpoints
```

### Key Patterns Used

* **Repository & Unit of Work** – Abstraction over data access logic
* **Orchestrator Pattern** – Used in the Web layer to coordinate Controller and Application logic
* **Dependency Injection** – Extensive use for decoupling and testability

---

## 🔹 Installation & Setup

### Prerequisites

* Docker Desktop
* .NET SDK (version defined in `global.json` or latest stable)

---

### Option 1: Docker Compose (Recommended)

This option starts both the application and PostgreSQL database automatically.

```bash
git clone https://github.com/karabeyogluonur/personel-website.git
cd personel-website
docker-compose up --build
```

* Application: [http://localhost:8080](http://localhost:8080)
* PostgreSQL: Port `5433`

---

### Option 2: Local Development

#### Configure Database

Ensure PostgreSQL is running and update `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=PersonelWebsiteDB;Username=your_user;Password=your_password"
}
```

#### Apply Migrations

```bash
cd src/Presentation/PW.Web
dotnet ef database update --context PWDbContext
dotnet ef database update --context AuthDbContext
```

#### Run the Application

```bash
dotnet run
```

---

## 🔹 Usage

### Admin Panel

* Default data is seeded during startup (see `IdentityInitialiser.cs`).
* Navigate to `/Admin` to access the dashboard.
* Use seeded credentials (if available) or create a user and assign roles manually.

### Localization

* Manage languages from the **Language** section in the Admin Panel.
* Add or update translation keys dynamically.
* Localization is handled via a custom `LanguageService` backed by the database.

---

## 🔹 Configuration

Configuration is handled via `appsettings.json` and environment variables.

| Setting Key                         | Description                  | Environment Variable                   |
| ----------------------------------- | ---------------------------- | -------------------------------------- |
| ConnectionStrings:DefaultConnection | PostgreSQL connection string | `ConnectionStrings__DefaultConnection` |
| ASPNETCORE_ENVIRONMENT              | Runtime environment          | `ASPNETCORE_ENVIRONMENT`               |

Application-wide constants such as **Roles**, **Area Names**, and **Storage Paths** are defined under:

```
PW.Application/Common/Constants
```

---

## 🔹 Deployment

### Docker Deployment

```bash
docker build -t personel-website .
docker run -d -p 80:8080 \
  -e ConnectionStrings__DefaultConnection="<PROD_DB_STRING>" \
  personel-website
```

> ⚠️ Ensure both `PWDbContext` and `AuthDbContext` migrations are applied in production.

---

## 🔹 Roadmap

* [ ] CMS modules (dynamic pages, blog management)
* [ ] Headless CMS support (REST / GraphQL)
* [ ] Unit & integration testing (xUnit)
* [ ] CI/CD with GitHub Actions

---

## 🔹 Contributing

Contributions are welcome.

```bash
git checkout -b feature/short-description
git commit -m "feat: add short feature description"
git push origin feature/short-description
```

Please follow existing architecture rules, naming conventions, and clean code principles.

---

## 🔹 License

This project is open-source. Refer to the repository for license details. If no license file is present, standard copyright laws apply.
