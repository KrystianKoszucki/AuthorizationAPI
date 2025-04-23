# Authorization API (.NET 6)


# Table of Contents

- [About the Project](#about-the-project)
	- [Technologies Used](#technologies-used)
- [Features](#features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
- [Running the API with Docker](#running-the-api-with-docker)
- [API Documentation](#api-documentation)
- [Running Tests Locally](#running-tests-locally)


# About the Project

A secure and modular Authorization API built with **.NET 6**, providing authentication and role-based access control with possibility of banning users. Secured endpoints using policy-based and role-based authorization, ensuring granular access control using **JWT** standards. The API is designed to be scalable, testable, and container-ready with **Docker**, featuring automated test execution prior to deployment.

## Technologies Used

-   .NET 6
    
-   ASP.NET Core Identity
    
-   Entity Framework Core
    
-   JWT
    
-   xUnit
    
-   Docker
    
-   Swagger

-   SQL Server
 
-   SQLite

# Features

-   🔑 JWT-based authentication
    
-   👥 Role-based and policy-based authorization
    
-   👤 User registration, login, CRUD operations on user account, banning users
    
-   ✅ Secure endpoint protection
    
-   🧪 Integration and unit testing with xUnit
    
-   🐳 Dockerized with test-first startup logic
    
-   🧰 Swagger for API documentation
    
-   🗃️ EF Core for database access

-   🖥️ + ⚡ SQLServer + SQLite


## Getting Started

### Prerequisites

-   [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
    
-   [Docker](https://www.docker.com/)

## Running the API with Docker

This project uses Docker to build, test, and run the API. It follows a **test-first approach**:

> 🧪 The container will first execute all tests.  
> ✅ If tests pass, the API starts.  
> ❌ If tests fail, the container exits.

#### To run:

`docker build --rm --no-cache -t authorization-api .`

`docker run -d -p 44383:80 --name authorization-api-container authorization-api`

Then API container is working in the background. To check if it's working, just open in the browser:

`http://localhost:44383/swagger/index.html` 

To stop the container, use:

`docker stop authorization-api-container`

## API Documentation

After running the app, you can explore the API via **Swagger UI** at:
`https://localhost:44383/swagger/index.html`


## Running Tests Locally

If you would like to run tests outside Docker, use:
`dotnet test`
