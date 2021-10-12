<!---
# 🔥 Attention!!

**Currently, .NET MAUI is in preview, and setting up the environment for the .NET MAUI is quite tricky and challenging. So if you would like to ignore/skip .NET MAUI for now then, simply remove the `MauiBlazorApp` and `MauiBlazorApp.WinUI` projects from the solution and build the solution. Now everything should work fine!**
-->

# 🏃‍♂️ How to Run the Project
  1. First make sure that you have **.NET 6.0** and **Visual Studio 2022** are installed.
  2. Now open the solution with VS 2022 and build the solution to make sure that there is no error.
  3. Now make **`Identity.Api`, `EmployeeManagement.Api` and `BlazorWasmApp`** projects as startup projects and then run it. On startup necessary databases will be created in **MSSQLLocalDB**

# Clean Architecture in ASP.NET Core
This repository contains the implementation of Domain Driven Design and Clear Architecture in ASP.NET Core.

# ⚙️ Fetures
1. Domain Driven Design
2. CQRS
3. REST API
4. API Versioning
5. Blazor Client (Web Assembly)
6. Caching with InMemory and Redis
7. Logging with Serilog
8. EF Core Repository and Cache Repository
9. Microsoft SQL Server
10. Simple and clean admin template for starter

# 📁 Folder Structures:
![Solution](https://user-images.githubusercontent.com/14342773/136995798-70c684d1-cea8-4b86-b45f-c768a6fd4265.PNG)

## 📂 src/ServerApp:
  Will contain all the projects of the server side app and will look like as follows:
  ![ServerFolder](https://user-images.githubusercontent.com/14342773/123045708-094b4500-d41d-11eb-9db3-d8cbfb7b9a31.PNG)

### 📂 src/ServerApp/Core:
  Core folder contains the projects related to the application core funcationalities like Domain Logic and Application Logic. This folder is the heart of the server app.
  
  ![CoreFolder](https://user-images.githubusercontent.com/14342773/123046128-88d91400-d41d-11eb-905a-d680d264f8a1.PNG)

  
#### 📝 EmployeeManagement.Domain Project: 
  This is application's **Domain Layer** which will contain:
   1. Domain entities which will be mapped to database table
   2. Domain logic,
   3. Domain repositories
   4. Value objects.
   5. Domain Exceptions

This will not depend on any other project. This is fully independent.

#### 📝 EmployeeManagement.Application Project:
  This is application's **Application Layer** which will contain:
   1. Appplication Logic
   2. Infrastructure repositories' interfaces i.e Cache Repository interfaces.
   3. Infrastructure services' interfaces i.e IEmailSender, ISmsSender, IExceptionLogger etc.
   4. Data Transfer Objects (Dtos)
   5. Command and Queries
  
  It will only depend on Domain project aka **Domain Layer.**
  
  ![Application](https://user-images.githubusercontent.com/14342773/136792482-c61660a2-af4e-4b85-940c-b1370b7e96f9.PNG)

  
### 📂 src/ServerApp/Infrastructure:
  This folder will contains all the project related to project's infrastuctures like Data access code, persistance and application's cross cutting concerns' intefaces implementation like IEmailSender, ISmsSender etc.
  
  ![Infrastructure](https://user-images.githubusercontent.com/14342773/123589564-37f56100-d80b-11eb-8f94-c79ea589adf8.PNG)

  
### 📂 src/ServerApp/Presentation:
  This folder will contain all the REST API projects which is the **PresentationLayer** of the project.
