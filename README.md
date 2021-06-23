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

## 📂 Server Folder:
  Will contain all the projects of the server side app and will look like as follows:
  ![ServerFolder](https://user-images.githubusercontent.com/14342773/123045708-094b4500-d41d-11eb-9db3-d8cbfb7b9a31.PNG)

### 📂 Core Folder:
  Core folder contains the projects related to the application core funcationalities like Domain Logic and Application Logic. This folder is the heart of the server app.
  
  ![CoreFolder](https://user-images.githubusercontent.com/14342773/123046128-88d91400-d41d-11eb-905a-d680d264f8a1.PNG)

  
#### 📝 EmployeeManagement.Domain Project: 
  This is application's **Domain Layer** which will contain domain entities which will be mapped to database table, domain logic, domain repositories and value objects.
#### 📝 EmployeeManagement.Application Project:
  This is application's **Application Layer** which will contain application business logic, infrastructure services repositories, dtos. It will only depend on Domain project aka **Domain Layer.**
  
### 📂 Infrastructure folder:
  This folder will contains all the project related to project's infrastuctures like Data access code, persistance and application's cross cutting concern's intefaces implementation like IEmailSender, ISmsSender etc.
  
### 📂 ApiApps folder:
  This folder will contain all the REST API projects which is the **PresentationLayer** of the project.
