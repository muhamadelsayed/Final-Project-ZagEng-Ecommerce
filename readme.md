# Ecommerce Platform - Backend (.NET 8)

Backend for the E-Commerce final project using **Onion Architecture**.

## Architecture
src/
├── Ecommerce.Domain          → Entities + Interfaces
├── Ecommerce.Application     → Business Logic + DTOs
├── Ecommerce.Infrastructure  → EF Core, JWT, Email, Repositories
└── Ecommerce.API             → Controllers + Program.cs


## Tech Stack

- .NET 8
- Entity Framework Core 8
- PostgreSQL (Supabase)
- JWT Bearer
- BCrypt
- MailKit

## How to Run

1. Clone the repo
2. Copy `appsettings.Example.json` → `appsettings.json` and fill your secrets
3. Run:
```bash
cd src/Ecommerce.API
dotnet run
Important Notes

Never commit appsettings.json with real passwords.
Use appsettings.Example.json as a template.
Database schema already exists on Supabase.