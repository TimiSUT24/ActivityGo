# 🎯 ActiviGo – Activity Booking Platform

A complete booking system for sports and leisure activities, built with **ASP.NET Core (REST API, EF Core)** and **React**.  
Supports both indoor and outdoor activities with weather integration.

---

## 📚 Table of Contents

- [📌 About the Project](#-about-the-project)
- [🧱 Architecture Overview](#-architecture-overview)
- [🧩 Data Models](#-data-models)
- [🌐 API Endpoints](#-api-endpoints)
- [📊 Roadmap](#-roadmap)
- [✅ Business Rules](#-business-rules)
- [⚙️ Technologies Used](#-technologies-used)
- [📦 NuGet Packages](#-nuget-packages)
- [⚙️ Getting Started](#️-getting-started)
- [🙌 Contributors](#-contributors)
- [📄 License](#-license)

---

## 📌 About the Project

ActiviGo is a simple booking app that helps people find and reserve fun sports and leisure activities in their town.  
The system handles:

- User authentication & authorization (JWT, roles: User/Admin)
- Activity and venue management
- Session booking with capacity & double-booking protection
- Weather forecasts for outdoor activities
- Admin dashboard for CRUD operations and statistics
- Responsive React frontend for users and administrators

---

## 🧱 Architecture Overview

The project is based on **N-tier architecture** with clear separation of concerns:

- **Presentation Layer** → API Controllers  
- **Application Layer** → Services  
- **Domain Layer** → Entities & Interfaces  
- **Infrastructure Layer** → Repositories & DbContext  

**Design patterns used:**
- **Service Pattern** for business logic
- **Repository Pattern** for data access
- **Sliced Architecture** feature first layering

- 🧾 DTOs for input/output separation  
- 🧪 Validation with FluentValidation  
- 🔐 JWT authentication & role-based access  
- 📘 Swagger/OpenAPI for documentation  

---

## 🧩 Data Models

```csharp
🧱 BaseEntity
Id, CreatedAtUtc, UpdatedAtUtc

📅 ActivityOccurrence
Id, ActivityId, LocationId, StartTime, EndTime, Capacity

📝 Booking
Id, UserId, ActivityOccurrenceId, Status (Upcoming, Past, Cancelled)

🏷️ Category
Name, Description, IsActive

📍 Location
Name, Address, Latitude, Longitude, IsActive, Capacity

🏃 SportActivity
Name, Description, Category, DefaultDurationMinutes, Price, ImageUrl, IsActive

👤 User
FirstName, LastName, Email, Role (User/Admin), PasswordHash

```
## 🌐 API Endpoints

### 🔐 Authentication
| Method | Endpoint        | Description                          | Status       |
|--------|-----------------|--------------------------------------|--------------|
| POST   | `/auth/register`| Register a new user                  | ⏳ Coming Soon |
| POST   | `/auth/login`   | Authenticate and receive JWT         | ⏳ Coming Soon |
| POST   | `/auth/refresh` | Refresh JWT token                    | ⏳ Coming Soon |
| POST   | `/auth/logout`  | Logout user                          | ⏳ Coming Soon |

### 👤 User
| Method | Endpoint        | Description                          | Status       |
|--------|-----------------|--------------------------------------|--------------|
| GET    | `/user/activities`      | List activities (with optional filters)   | ⏳ Coming Soon |
| GET    | `/user/activities/{id}` | Get details of an activity                | ⏳ Coming Soon |
| POST   | `/user/bookings`        | Book an activity occurrence               | ⏳ Coming Soon |
| DELETE | `/user/bookings/{id}`   | Cancel a booking                          | ⏳ Coming Soon |
| GET    | `/user/activities`      | List activities (with optional filters)   | ⏳ Coming Soon |

### 🛠️ Admin
| Method | Endpoint                   | Description                          | Status       |
|--------|----------------------------|--------------------------------------|--------------|
| POST   | `/admin/activities`         | Create new activity                  | ⏳ Coming Soon |
| PUT    | `/admin/activities/{id}`    | Update activity                      | ⏳ Coming Soon |
| DELETE | `/admin/activities/{id}`    | Delete/disable activity              | ⏳ Coming Soon |
| POST   | `/admin/locations`          | Add new location                     | ⏳ Coming Soon |
| POST   | `/admin/occurrences`        | Create activity occurence            | ⏳ Coming Soon |
| GET    | `/admin/statistics`         | View booking statistics              | ⏳ Coming Soon | 

---

## 📊 Roadmap

Planned improvements and extra features:  
- ⏳ **Docker Compose** (API + DB packaged for easy startup)  
- ⏳ **Global Exception Middleware** (RFC 7807 standardized error responses)  
- ⏳ **Integration Tests** (WebApplicationFactory + InMemory DB)  
- ⏳ **Response Caching** (optimize frequent GET requests)  
- ⏳ **Rate Limiting** (protect sensitive endpoints like `/auth/login`)
---

## ✅ Business Rules

- ⏰ Cancellation cutoff: 2 hours before activity start (configurable)
- 👥 Capacity limits enforced per occurrence
- 🔄 Double-booking prevention (no overlapping times)
- 🌦️ Outdoor flag triggers weather forecast integration

## ⚙️ Technologies Used

- ASP.NET Core
- Entity Framework Core
- SQL Server
- React (Vite)
- React Router & Hooks
- FluentValidation
- Swagger / OpenAPI
- AutoMapper
- JWT Authentication

### 📦 NuGet Packages

This project uses the following NuGet packages:

- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.AspNetCore.Identity
- Microsoft.AspNetCore.Authentication.JwtBearer
- Swashbuckle.AspNetCore
- FluentValidation.AspNetCore
- AutoMapper.Extensions.Microsoft.DependencyInjection
  
## ⚙️ Getting Started

- IDE of choice

```bash
1. Clone the repository:
   git clone https://github.com/JonssonF/TooliRent.git
   cd TooliRent
2. Configure the database

The project uses Entity Framework Core (Code-First).
In appsettings.json you’ll find the connection string for SQL Server (adjust it to your local setup, e.g. localhost or LocalDB).

3. Apply migrations

Make sure the database is created and up to date:
dotnet ef database update --project ActiviGo.Infrastructure --startup-project ActiviGo.API

4. Run the API
dotnet run --project ActiviGo.API

5. Test in Swagger
API available at: https://localhost:5001/swagger
React frontend available at: http://localhost:5173/
```
---

## 🙌 Contributors
- 🌐[GitHub – Alexander Ek](https://github.com/evuul)
- 🌐[GitHub – Tim Petersén](https://github.com/TimiSUT24)
- 🌐[GitHub – Viggo Kristensen](https://github.com/bingodingo04)
- 🌐[GitHub – Fredrik Jonsson](https://github.com/JonssonF)

## 📄 License
MIT License – free to use, modify, and distribute.
