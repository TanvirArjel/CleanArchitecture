# 🔥 Attention!!

**Currently, .NET MAUI is in preview, and setting up the environment for the .NET MAUI is quite tricky and challenging. So if you would like to ignore/skip .NET MAUI for now then, simply remove the `MauiBlazorApp` and `MauiBlazorApp.WinUI` projects from the solution and build the solution. Now everything should work fine!**

# 🏃‍♂️ How to Run the Project
  1. First build the solution.
  2. Now make **`Identity.Api`, `EmployeeManagement.Api` and `MauiBlazor.WebUI`** projects as startup projects and then run it. On startup necessary databases will be created in **MSSQLLocalDB**

# Clean Architecture in ASP.NET Core
This repository contain the implementation of domain driven design and clear architecture in ASP.NET Core.

# ⚙️ Fetures
1. Domain Driven Design
2. REST API
3. API Versioning
4. Blazor Client (Web Assembly)
5. Caching with InMemory and Redis
6. Logging with Serilog
7. EF Core Repository and Cache Repository
8. Microsoft SQL Server
9. Simple and clean admin template for starter

# 📁 Folder Structures:
![SolutionFolder](https://user-images.githubusercontent.com/14342773/123045601-ea4cb300-d41c-11eb-8caf-8b7846564f28.PNG)

## 📂 src/Server:
  Will contain all the projects of the server side app and will look like as follows:
  ![ServerFolder](https://user-images.githubusercontent.com/14342773/123045708-094b4500-d41d-11eb-9db3-d8cbfb7b9a31.PNG)

### 📂 src/Server/Core:
  Core folder contains the projects related to the application core funcationalities like Domain Logic and Application Logic. This folder is the heart of the server app.
  
  ![CoreFolder](https://user-images.githubusercontent.com/14342773/123046128-88d91400-d41d-11eb-905a-d680d264f8a1.PNG)

  
#### 📝 EmployeeManagement.Domain Project: 
  This is application's **Domain Layer** which will contain:
   1. Domain entities which will be mapped to database table
   2. Domain logic,
   3. Domain repositories
   4. Value objects.

This will not depend on any other project. This is fully independent.

#### 📝 EmployeeManagement.Application Project:
  This is application's **Application Layer** which will contain:
   1. Appplication Business Logic
   2. Infrastructure repositories' interfaces i.e Cache Repository interfaces.
   3. Infrastructure services' interfaces i.e IEmailSender, ISmsSender, IExceptionLogger etc.
   4. Data Transfer Objects (Dtos)
   5. Application Custom Exception types.
  
  It will only depend on Domain project aka **Domain Layer.**
  
  ![CoreApplication](https://user-images.githubusercontent.com/14342773/123301594-a2717d00-d53d-11eb-8f74-076ff92f682d.PNG)

  
### 📂 src/Server/Infrastructure:
  This folder will contains all the project related to project's infrastuctures like Data access code, persistance and application's cross cutting concerns' intefaces implementation like IEmailSender, ISmsSender etc.
  
  ![Infrastructure](https://user-images.githubusercontent.com/14342773/123589564-37f56100-d80b-11eb-8f94-c79ea589adf8.PNG)

  
### 📂 src/Server/ApiApps:
  This folder will contain all the REST API projects which is the **PresentationLayer** of the project.
