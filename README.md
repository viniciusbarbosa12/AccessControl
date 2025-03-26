# AccessControl API

AccessControl is a backend system built with ASP.NET Core, designed to simulate a secure and testable door access control system. It includes features such as role-based authentication, card/door management, integration with JWT, health checks, test automation, and more.

---

## Features

- Role-based Authorization (Admin) using ASP.NET Identity
- JWT Authentication with custom policies (email domain, roles)
- Card Management with access permission per door
- Door Management with swipe simulation
- Automated Tests for controllers and services
- Health Checks + UI with `/health` and `/health-ui`
- Exception Handling Middleware
- Application Insights telemetry
- Email Notifications support
- Swagger/OpenAPI with JWT integration

---

## Technologies

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core (InMemory)
- Identity Core
- Swagger / Swashbuckle
- xUnit + WebApplicationFactory
- HealthChecks + HealthChecks.UI
- Application Insights (Telemetry)
- Custom Middlewares (Logging, Exceptions)
- Terraform + Azure App Service (optional deployment)

---

## Project Structure

```
├── .net-main (main API project)
├── Domain         (Entities, Models, Filters, Config)
├── Data           (DbContext, Repositories)
├── Services       (Services, Factories, Interfaces)
├── AccessControl.Tests (Automated tests)
├── Middlewares    (Exception + Request timing)
└── Seeding        (Admin user + Identity roles)
```

---

## Authentication

Use `/api/auth/register-admin` to create an admin user, then authenticate via `/api/auth/login` to receive a JWT token. Use this token in Swagger ("Authorize" button) to access secured endpoints.

**Admin only routes include:**

- `POST /api/cards/create`
- `POST /api/cards/grant-access`
- `POST /api/cards/cancel-permission`
- `POST /api/doors/create`
- `DELETE /api/doors/remove`
- `POST /api/doors/swipe`

**Default test admin credentials:**

- Username/Email: `admin@access.com`
- Password: `Admin123!`

---

## Run Locally

```bash
dotnet build
dotnet run --project .net-main
```

Then open `http://localhost:5000` in your browser.

---

## Health Check

- Basic health check: [GET] `/health`
- UI dashboard: [GET] `/health-ui`

---

## Run Tests

```bash
cd AccessControl.Tests
dotnet test
```

---

## Deployment (Optional)

Deployment via **Terraform + Azure App Service** is supported. The infrastructure can be provisioned with a `main.tf` and `terraform.tfvars`.

---

## Email Configuration

Email config is stored in `appsettings.json` under `EmailSettings`. You can inject and send via `IEmailService`.

---

## Contact

If you have questions or want to contribute, feel free to reach out.
