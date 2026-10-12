# Taxi Booking Management System

A project built with **C#**, **ASP.NET Web Forms (.NET Framework)**, **ADO.NET**, and **Microsoft SQL Server Express**.

## Features

- Customer registration and login
- Password hashing using PBKDF2
- Admin login and protected admin dashboard
- Pickup and drop-off selection from saved locations
- Taxi assignment based on location and availability
- Trip distance and fare calculation
- Database access using parameterized SQL queries

> Features may still be under development. Refer to the source code for the current implementation.

## Requirements

Install on Windows:

1. **Visual Studio Community** with the **ASP.NET and web development** workload.
2. The .NET Framework targeting pack matching the version in `TaxiBookingSystem.csproj` (the project has been developed with .NET Framework 4.8/4.8.1 tooling).
3. **SQL Server Express**.
4. **SQL Server Management Studio (SSMS)** to run the database script.
5. **Git** (optional, if cloning from a terminal).

## Get the project

Replace the URL below with your actual GitHub repository URL:

```bash
git clone https://github.com/YOUR-USERNAME/TaxiBookingSystem.git
cd TaxiBookingSystem
```

Open `TaxiBookingSystem.slnx` in Visual Studio. If your Visual Studio version does not support `.slnx`, update Visual Studio or open `TaxiBookingSystem.csproj` directly.

## Set up the database

1. Start SQL Server Express.
2. Open SSMS and connect to your local instance, commonly `.\SQLEXPRESS`, using Windows Authentication.
3. Open `TaxiBookingDb_Setup.sql` in SSMS.
4. Execute the script.
5. Refresh **Databases** in Object Explorer and verify that `TaxiBookingDb` exists.

The script creates the tables and inserts illustrative sample locations and taxis. Coordinates are demo data, not live GPS information. Run the script against a new local development database; back up any existing database before changing it.

## Configure the connection string

In `TaxiBookingSystem/Web.config`, check the `TaxiBookingDb` connection string. The development configuration is:

```xml
<connectionStrings>
  <add name="TaxiBookingDb"
       connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=TaxiBookingDb;Integrated Security=True;TrustServerCertificate=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

If your SQL Server instance has a different name, update `Data Source`. With Windows Authentication, your Windows account must have permission to access the database.

## Restore and run

1. Open the solution/project in Visual Studio.
2. Allow NuGet packages to restore. If needed, right-click the solution and choose **Restore NuGet Packages** (the menu wording can vary by Visual Studio version).
3. Select **Build > Build Solution**.
4. Set `TaxiBookingSystem` as the startup project if prompted.
5. Press **F5** or click **IIS Express**.
6. Register a test customer and verify login and booking.
7. Create an admin account using the project's approved local setup procedure. Do not assume the database script creates an admin account; it intentionally does not insert a default password.

## Main project files

```text
TaxiBookingSystem/
├── AdminLogin.aspx
├── AdminDashboard.aspx
├── BookTaxi.aspx
├── CustomerDashBoard.aspx
├── Default.aspx
├── Login.aspx
├── Register.aspx
├── Services/
│   ├── BookingService.cs
│   └── PasswordHelper.cs
├── Web.config
└── TaxiBookingSystem.csproj

TaxiBookingDb_Setup.sql
README.md
```

File names and folders may change as the project develops.

## Security notes

- Do not commit passwords, API keys, or production connection strings to GitHub.
- Use only fictional test customer data in development.
- Do not publish temporary admin setup pages with the deployed application.
- This is an educational project. Review authentication, authorization, validation, error handling, and deployment configuration before using it outside a local development environment.
