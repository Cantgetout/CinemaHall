# CinemaHub

CinemaHub is a full-stack web application built with ASP.NET Core MVC for managing cinema operations. 
It provides a platform for browsing movies, exploring actor filmographies, and viewing screening schedules, along with a complete administrative backend for staff.
The project was created as third-year final project for my ITKARIERA course, thus there are some areas of the application where more work should be applied but for the 
grading system this project is perfect.

## Features

* **Movie & Actor Catalog:** Browse movies, view synopses, and explore actor details and filmographies.
* **Screening Management:** View and manage movie screenings assigned to specific cinema halls and times.
* **Role-Based Access Control:**
  * **Administrators:** Manage user accounts, assign roles, and maintain system access.
  * **Moderators:** Perform CRUD (Create, Read, Update, Delete) operations on the movie catalog, actor database, and screening schedules.
  * **Users:** Register, log in, and browse the public catalog.
* **Automated Data Seeding:** Includes a database seeder that automatically populates the system with required roles, a master administrator account, and dummy data for testing purposes.

## Tech Stack

* **Framework:** ASP.NET Core 9.0 MVC
* **Language:** C#
* **Database:** MySQL, Entity Framework Core (Pomelo.EntityFrameworkCore.MySql)
* **Authentication:** ASP.NET Core Identity
* **Frontend:** HTML5, CSS3, Bootstrap 5, Razor Pages
* **Testing:** NUnit, Entity Framework In-Memory Database

## Prerequisites

To run this project locally, you will need:
* [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* MySQL Server (local or remote)
* Visual Studio 2022, JetBrains Rider, or VS Code

## Getting Started

### 1. Clone the repository
```bash
git clone https://github.com/Cantgetout/CinemaHall.git
cd CinemaHall/CinemaHub
```

### 2. Configure the Database
Update the `DefaultConnection` string in `appsettings.json` to point to your local MySQL instance:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CinemaHub;Uid=your_user;Pwd=your_password;"
}
```

### 3. Initialize User Secrets (Local Configuration)
To securely run the database seeder on your local machine, initialize the .NET Secret Manager and set the default passwords:
```bash
dotnet user-secrets init
dotnet user-secrets set "SeederConfiguration:AdminPassword" "YourAdminPassword123!"
dotnet user-secrets set "SeederConfiguration:DummyUserPassword" "YourDummyPassword123!"
```

### 4. Apply Database Migrations
Run the following command to create the database schema and apply all migrations:
```bash
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```
The application will be available at `https://localhost:7013` (or the port specified in your `launchSettings.json`).

## Testing

The project includes a dedicated unit testing suite (`CinemaHub_Tests`) that validates the core business logic and service layers using an In-Memory database.

To execute the tests, navigate to the test project directory and run:
```bash
cd ../CinemaHub_Tests
dotnet test
```

## Project Structure

* **CinemaHub/Controllers:** Handles incoming HTTP requests and routes them to the appropriate services.
* **CinemaHub/Services:** Contains the core business logic and database interaction interfaces (`IMovieService`, `IActorService`, etc.).
* **CinemaHub/Data/Models:** Entity Framework data models mapping to the MySQL database.
* **CinemaHub/Data/Models/ViewModels:** Strongly typed models used to pass data safely between the controllers and Razor views.
* **CinemaHub_Tests:** NUnit test project containing the service layer unit tests.
