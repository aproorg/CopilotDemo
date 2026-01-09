using CopilotDemo.Core.Entities;
using CopilotDemo.Core.Interfaces;
using CopilotDemo.Core.Services;
using CopilotDemo.Repository.Data;
using CopilotDemo.Repository.Repositories;
using CopilotDemo.Web.Configuration;
using CopilotDemo.Web.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Load secrets configuration
builder.Configuration.AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true);

// Configure API Key options
builder.Services.Configure<ApiKeyOptions>(
    builder.Configuration.GetSection(ApiKeyOptions.SectionName));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

  // Add API Key to Swagger UI
  c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
  {
    Description = "API Key needed to access the endpoints. X-Api-Key: {your_api_key}",
    In = ParameterLocation.Header,
    Name = "X-Api-Key",
    Type = SecuritySchemeType.ApiKey
  });

  c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
  options.UseInMemoryDatabase("CopilotDemoDb");

});

// Register application services
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

// Seed the in-memory database for demo purposes
using (var scope = app.Services.CreateScope())
{
  var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

  // Ensure the database is created
  context.Database.EnsureCreated();

  // Only seed if no data exists
  if (!context.TodoItems.Any())
  {
    context.TodoItems.AddRange(
        new TodoItem
        {
          Id = 1,
          Title = "Sample Task 1",
          Description = "This is a sample task for demo purposes",
          IsCompleted = false,
          CreatedAt = DateTime.UtcNow
        },
        new TodoItem
        {
          Id = 2,
          Title = "Sample Task 2",
          Description = "This is another sample task",
          IsCompleted = true,
          CreatedAt = DateTime.UtcNow.AddHours(-1)
        },
        new TodoItem
        {
          Id = 3,
          Title = "Demo Task 3",
          Description = "Complete the API demonstration",
          IsCompleted = false,
          CreatedAt = DateTime.UtcNow.AddMinutes(-30)
        }
    );

    context.SaveChanges();
  }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c =>
  {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
  });
}

app.UseHttpsRedirection();

// Add API Key authentication middleware
app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
