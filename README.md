<!---
# 🔥 Attention!!

**Currently, .NET MAUI is in preview, and setting up the environment for the .NET MAUI is quite tricky and challenging. So if you would like to ignore/skip .NET MAUI for now then, simply remove the `MauiBlazorApp` and `MauiBlazorApp.WinUI` projects from the solution and build the solution. Now everything should work fine!**
-->

# 🏃‍♂️ How to Run the Project
  1. First make sure that you have **.NET 7.0** and **Visual Studio 2022** are installed.
  2. Now open the solution with VS 2022 and build the solution to make sure that there is no error.
  3. Now make **`CleanHr.Api` and `ClearHr.Blazor`** projects as startup projects and then run it. On startup necessary databases will be created in **MSSQLLocalDB**
  4. (Optional) To enable distributed tracing with Grafana Tempo, run: `docker compose -f src/ServerApp/Presentation/CleanHr.Api/docker-compose.yml up -d`
     - Access Grafana at: http://localhost:3000
     - Traces will be automatically sent to Tempo and visualized in Grafana
     - See `src/ServerApp/Presentation/CleanHr.Api/Observability/OBSERVABILITY.md` for detailed documentation

# Clean Architecture in ASP.NET Core
This repository contains the implementation of Domain Driven Design and Clear Architecture in ASP.NET Core.

# ⚙️ Fetures
1. Domain Driven Design
2. CQRS
3. REST API
4. API Versioning
5. Blazor Client (Web Assembly)
6. Caching with InMemory and Redis
7. Distributed Tracing with OpenTelemetry and Grafana Tempo
8. EF Core Repository and Cache Repository
9. Microsoft SQL Server
10. Simple and clean admin template for starter

# 📁 Folder Structures:
![image](https://user-images.githubusercontent.com/14342773/188484576-458676e4-a912-4b35-89f9-af062e1f343e.png)

## 📂 src/ServerApp:
  Will contain all the projects of the server side app and will look like as follows:
  ![tempsnip](https://user-images.githubusercontent.com/14342773/188484945-2b5bb4a8-37e0-416b-93e7-87a56fb9f20d.png)

### 📂 src/ServerApp/Core:
  Core folder contains the projects related to the application's core funcationalities like Domain Logic and Application Logic. This folder is called the heart of the server app.
  
  ![tempsnip](https://user-images.githubusercontent.com/14342773/188485084-9b67457b-aeed-4003-9e6f-d0d1e1865073.png)

  
#### 📝 EmployeeManagement.Domain Project: 
  This is application's **Domain Layer** which will contain:
   1. Domain entities and aggregate roots which will be mapped to database table
   2. Domain logic,
   3. Domain repositories
   4. Value objects.
   5. Domain Exceptions

This will not depend on any other project. This is fully independent.

![tempsnip](https://user-images.githubusercontent.com/14342773/188485592-9a95589c-792e-422d-85ca-88b7eb91941b.png)

#### 📝 EmployeeManagement.Application Project:
  This is application's **Application Layer** which will contain:
   1. Appplication Logic
   2. Infrastructure repositories' interfaces i.e Cache Repository interfaces.
   3. Infrastructure services' interfaces i.e IEmailSender, ISmsSender, IApplicationLogger etc.
   4. Data Transfer Objects (Dtos)
   5. Command and Queries
  
  It will only depend on Domain project aka **Domain Layer.**
  
  ![tempsnip](https://user-images.githubusercontent.com/14342773/188486030-ea017c5e-2a31-4013-b634-a89d8f07ef15.png)

  
### 📂 src/ServerApp/Infrastructure:
  This folder will contains all the project related to project's infrastuctures like Data access code, persistance and application's cross cutting concerns' intefaces implementation like IEmailSender, ISmsSender etc.
  
  ![tempsnip](https://user-images.githubusercontent.com/14342773/188486351-ace161da-2a5c-45f7-95d1-319ba166df4e.png)

  
### 📂 src/ServerApp/Presentation:
  This folder will contain the REST API projects which is the **PresentationLayer** of the project.
  
  ![tempsnip](https://user-images.githubusercontent.com/14342773/188486653-289f5002-66ed-4641-9e13-f9a8e162fd57.png)

