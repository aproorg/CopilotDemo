# CopilotDemo Todo API

A clean layered architecture REST API for managing todo items with API key authentication.

## Architecture

The solution follows a three-layer architecture:

### 1. CopilotDemo.Core (Application/Domain Layer)
- **Entities**: Domain models (`TodoItem`)
- **Interfaces**: Service and repository contracts (`ITodoService`, `ITodoRepository`)
- **Services**: Business logic implementation (`TodoService`)
- **Dependencies**: None

### 2. CopilotDemo.Repository (Data Access Layer)
- **Data**: EF Core DbContext configuration
- **Repositories**: Data access implementations (`TodoRepository`)
- **Dependencies**: Core layer, Entity Framework Core, SQL Server provider

### 3. CopilotDemo.Web (Presentation Layer)
- **Controllers**: REST API endpoints (`TodoController`)
- **Middleware**: API key authentication
- **Configuration**: Dependency injection, API key options
- **Dependencies**: Core and Repository layers

## Features

- RESTful API for Todo management
- API Key authentication via custom middleware
- Swagger/OpenAPI documentation
- Entity Framework Core with SQL Server
- Dependency Injection
- Clean separation of concerns

## Prerequisites

- .NET 10 SDK
- SQL Server or SQL Server LocalDB

## Setup

### 1. Configure the API Key

Create or update `appsettings.secrets.json` in the `CopilotDemo.Web` folder:

```json
{
  "ApiKey": {
    "Key": "your-secret-api-key-here"
  }
}
```

**Important**: This file is excluded from git via `.gitignore` to keep your API key secure.

### 2. Configure Database Connection

Update the connection string in `appsettings.json` if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CopilotDemoTodoDb;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

### 3. Run Migrations

```bash
# From the solution root directory
dotnet ef migrations add InitialCreate --project CopilotDemo.Repository --startup-project CopilotDemo.Web
dotnet ef database update --project CopilotDemo.Repository --startup-project CopilotDemo.Web
```

### 4. Run the API

```bash
dotnet run --project CopilotDemo.Web
```

The API will be available at:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5001`
- Swagger UI: `https://localhost:7001/swagger`

## API Endpoints

All endpoints require the `X-Api-Key` header with your configured API key.

### Get All Todos
```
GET /api/todo
```

### Get Todo by ID
```
GET /api/todo/{id}
```

### Create Todo
```
POST /api/todo
Content-Type: application/json

{
  "title": "Buy groceries",
  "description": "Milk, eggs, bread"
}
```

### Update Todo
```
PUT /api/todo/{id}
Content-Type: application/json

{
  "title": "Buy groceries",
  "description": "Milk, eggs, bread, butter",
  "isCompleted": false
}
```

### Delete Todo
```
DELETE /api/todo/{id}
```

### Mark Todo as Completed
```
POST /api/todo/{id}/complete
```

## Using the API with Authentication

### cURL Example
```bash
curl -H "X-Api-Key: your-secret-api-key-here" https://localhost:7001/api/todo
```

### Swagger UI
1. Navigate to `https://localhost:7001/swagger`
2. Click the "Authorize" button
3. Enter your API key in the value field
4. Click "Authorize"
5. All requests will now include the API key automatically

### Postman
1. Create a new request
2. In the Headers tab, add:
   - Key: `X-Api-Key`
   - Value: `your-secret-api-key-here`

## Security Notes

- The API key is stored in `appsettings.secrets.json` which is excluded from version control
- For production, consider using:
  - Azure Key Vault
  - Environment variables
  - User Secrets (for development)
  - More sophisticated authentication (JWT, OAuth2, etc.)
- HTTPS is enforced in production environments
- Swagger endpoints bypass authentication for documentation access

## Project Structure

```
CopilotDemo/
??? CopilotDemo.Core/
?   ??? Entities/
?   ?   ??? TodoItem.cs
?   ??? Interfaces/
?   ?   ??? ITodoRepository.cs
?   ?   ??? ITodoService.cs
?   ??? Services/
?       ??? TodoService.cs
??? CopilotDemo.Repository/
?   ??? Data/
?   ?   ??? ApplicationDbContext.cs
?   ??? Repositories/
?       ??? TodoRepository.cs
??? CopilotDemo.Web/
    ??? Configuration/
    ?   ??? ApiKeyOptions.cs
    ??? Controllers/
    ?   ??? TodoController.cs
    ??? Middleware/
    ?   ??? ApiKeyAuthenticationMiddleware.cs
    ??? Properties/
    ?   ??? launchSettings.json
    ??? appsettings.json
    ??? appsettings.secrets.json (not in git)
    ??? Program.cs
```

## Development

### Adding New Features

1. Define domain models in **Core/Entities**
2. Define interfaces in **Core/Interfaces**
3. Implement business logic in **Core/Services**
4. Implement data access in **Repository/Repositories**
5. Add controllers in **Web/Controllers**
6. Register services in **Web/Program.cs**

### Running Tests

```bash
dotnet test
```

## License

This is a demo project for educational purposes.
