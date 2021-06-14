# Clean Architecture in ASP.NET Core
This repository contain the implementation of domain driven design and clear architecture in ASP.NET Core.

# Fetures
1. Domain Driven Design
2. REST API
3. Blazor Client (Web Assembly)
5. Caching with InMemory and Redis
6. Logging with Serilog
7. EF Core Repository and Cache Repository
8. Microsoft SQL Server
9. Simple and clean admin template for starter

# Folder Structures
## Server Folder:
  Will contain all the projects of the server side app.
### Core Folder:
  Core folder contains the projects related to the application core funcationalities like Domain Logic and Application Logic. This folder is the heart of the server app.
  
#### EmployeeManagement.Domain Project: 
  This is application's **Domain Layer** which will contain domain entities which will be mapped to database table, domain logic, domain repositories and value objects.
#### EmployeeManagement.Application Project:
  This is application's **Application Layer** which will contain application business logic, infrastructure services repositories, dtos. It will only depend on Domain project aka **Domain Layer.**
  
  more to follow.....
