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
Id, CreatedAtUtc, UpdatedAtUtc, RowVersion

📅 ActivityOccurrence
StartUtc, EndUtc, CapacityOverride, ActivityId, PlaceId, PriceOverride
→ Navigation: Activity, Place, Bookings
→ Computed: EffectiveCapacity

📝 Booking
UserId, ActivityOccurrenceId, Status (Booked/Cancelled), BookedAtUtc, CancelledAtUtc
→ Navigation: User, ActivityOccurrence

🏷️ Category
Name, Description, IsActive
→ Navigation: Activities

📍 Place
Name, Address, Latitude, Longitude, Environment (Indoor/Outdoor), Capacity, IsActive
→ Navigation: Occurrences

🏃 SportActivity
Name, Description, Environment (Indoor/Outdoor), DefaultDurationMinutes, Price, ImageUrl, IsActive, CategoryId
→ Navigation: Category, Occurrences

👤 User (IdentityUser)
Firstname, Lastname, Email, Roles, RefreshTokens
→ Navigation: Bookings, RefreshTokens

🔁 RefreshToken
UserId, TokenHash, CreatedAtUtc, ExpiresAtUtc, RevokedAtUtc, ReplacedByTokenHash, CreatedByIp, RevokedByIp
→ Navigation: User

```
## 🌐 API Endpoints

### 🔐 Authentication
| Method | Endpoint             | Description                                   | Status |
|--------|----------------------|-----------------------------------------------|---------|
| POST   | `/api/auth/register` | Register a new user (assigns role 'User')     | ✅ Implemented |
| POST   | `/api/auth/login`    | Login and receive JWT + refresh cookie        | ✅ Implemented |
| POST   | `/api/auth/refresh`  | Refresh access token via refresh token        | ✅ Implemented |
| POST   | `/api/auth/logout`   | Logout and revoke all refresh tokens          | ✅ Implemented |
| GET    | `/api/auth/me`       | Get current authenticated user info           | ✅ Implemented |

---

### 🎯 Activities
| Method | Endpoint                 | Description                                     | Status |
|--------|--------------------------|-------------------------------------------------|---------|
| GET    | `/api/activity`          | List all activities (optionally include inactive) | ✅ Implemented |
| GET    | `/api/activity/{id}`     | Get activity by ID                              | ✅ Implemented |
| POST   | `/api/activity`          | Create a new activity                           | ✅ Implemented |
| PUT    | `/api/activity/{id}`     | Update an activity                              | ✅ Implemented |
| DELETE | `/api/activity/{id}`     | Delete (deactivate) an activity                 | ✅ Implemented |

---

### 🗓️ Activity Occurrences
| Method | Endpoint                               | Description                                      | Status |
|--------|----------------------------------------|--------------------------------------------------|---------|
| POST   | `/api/activityoccurrence`              | Create a new occurrence                          | ✅ Implemented |
| GET    | `/api/activityoccurrence`              | Get all occurrences                              | ✅ Implemented |
| GET    | `/api/activityoccurrence/{id}`         | Get occurrence by ID                             | ✅ Implemented |
| PUT    | `/api/activityoccurrence`              | Update an occurrence                             | ✅ Implemented |
| DELETE | `/api/activityoccurrence/{id}`         | Delete an occurrence                             | ✅ Implemented |
| GET    | `/api/activityoccurrence/with-weather` | Get all occurrences with weather data (by date range) | ✅ Implemented |

---

### 📍 Places
| Method | Endpoint                          | Description                              | Status |
|--------|-----------------------------------|------------------------------------------|---------|
| GET    | `/api/place`                      | List all places                          | ✅ Implemented |
| GET    | `/api/place/{id}`                 | Get place by ID                          | ✅ Implemented |
| POST   | `/api/place`                      | Create a new place (Admin only)          | ✅ Implemented |
| PUT    | `/api/place/{id}`                 | Update a place (Admin only)              | ✅ Implemented |
| DELETE | `/api/place/{id}`                 | Delete a place (Admin only)              | ✅ Implemented |
| PATCH  | `/api/place/{id}/active/{isActive}` | Activate or deactivate a place (Admin only) | ✅ Implemented |

---

### 📘 Bookings
| Method | Endpoint                | Description                                 | Status |
|--------|--------------------------|---------------------------------------------|---------|
| GET    | `/api/booking/me`        | Get all bookings for the logged-in user     | ✅ Implemented |
| GET    | `/api/booking/{id}`      | Get booking by ID (for current user)        | ✅ Implemented |
| POST   | `/api/booking`           | Create a new booking                        | ✅ Implemented |
| DELETE | `/api/booking/{id}`      | Cancel a booking (soft delete)              | ✅ Implemented |

---

### 📊 Admin Statistics
| Method | Endpoint                        | Description                                  | Status |
|--------|----------------------------------|----------------------------------------------|---------|
| GET    | `/api/statistics/summary`        | Summary overview (KPI)                       | ✅ Implemented |
| GET    | `/api/statistics/bookings-per-day` | Bookings count per day                       | ✅ Implemented |
| GET    | `/api/statistics/revenue-per-day`  | Revenue per day (Completed only)             | ✅ Implemented |
| GET    | `/api/statistics/top-activities`   | Top activities by bookings                   | ✅ Implemented |
| GET    | `/api/statistics/top-places`       | Top places by bookings                       | ✅ Implemented |
| GET    | `/api/statistics/bookings-by-category` | Bookings grouped by category             | ✅ Implemented |

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
