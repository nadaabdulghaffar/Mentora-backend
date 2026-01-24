# Mentora-backend
This repository contains the backend API for the Mentora application.

It is implemented in C# using .NET (Core / ASP.NET) and provides the server-side logic, database access, authentication, and RESTful endpoints required by the frontend.

## 1. Getting Started
### Prerequisites

+ .NET 8.0 SDK or higher
+ SQL Server (LocalDB, Express, or Full)
+ Visual Studio 2022 or VS Code
+ Node.js & React (for frontend)

## 2. Getting Started
### Clone the repository:
```bash
git clone https://github.com/nadaabdulghaffar/Mentora-backend.git
cd Mentora-backend
```
## 3. Installing Dependencies
### Restore NuGet packages and project dependencies:
```bash
dotnet restore
```
This command reads from all *.csproj files and installs required packages.

## 4.Database Setup & Migrations
1. Create and apply migrations:
```bash
dotnet ef migrations add InitialCreate --project ./src/Infrastructure/Mentora.Persistence --startup-project ./src/Presentation/Mentora.API
dotnet ef database update --project ./src/Infrastructure/Mentora.Persistence --startup-project ./src/Presentation/Mentora.API

```
## 5.Running the Application
You can start the API locally via:
```bash
dotnet run
```
By default, the API will run on:
```bash
http://localhost:5069
```
API documentation on :
```bash
http://localhost:5069/scalar
```