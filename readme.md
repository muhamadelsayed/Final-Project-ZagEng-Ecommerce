# Ecommerce Platform - Backend (.NET 8)

Backend for the E-Commerce final project using **Onion Architecture**.


## Architecture
src/
├── Ecommerce.Domain          → Entities + Interfaces
├── Ecommerce.Application     → Business Logic + DTOs + Service Interfaces
├── Ecommerce.Infrastructure  → EF Core, JWT, Email (OTP), Repositories
└── Ecommerce.API             → Controllers + Program.cs
text## Tech Stack

- .NET 8
- Entity Framework Core 8
- PostgreSQL (Supabase)
- JWT Bearer Authentication
- BCrypt (Password Hashing)
- MailKit (Email / OTP)
- Swagger / OpenAPI

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git
- Supabase account (Database already prepared)

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/muhamadelsayed/Final-Project-ZagEng-Ecommerce.git
cd Ecommerce
2. Configure Settings
Bashcd src/Ecommerce.API
cp appsettings.Example.json appsettings.json
Open appsettings.json and fill in:

Supabase Connection String
JWT Key
Email settings (for OTP)

3. Restore packages
Bashdotnet restore
4. Run the API
Bashdotnet run
The API will start on:

http://localhost:5075 (or the port shown in the terminal)
Swagger UI: http://localhost:5075/swagger

Project Structure Notes

Domain has zero dependencies on other layers.
Application depends only on Domain.
Infrastructure implements interfaces defined in Domain/Application.
API is the entry point.

Important Notes

Never commit appsettings.json with real passwords or connection strings.
Always use appsettings.Example.json as a template.
The database schema already exists on Supabase.
Current focus: Authentication (Register, OTP, Login, Roles).

Contribution

Create a new branch for your feature
Follow the existing Onion Architecture
Do not put infrastructure concerns inside Domain
Open a Pull Request
