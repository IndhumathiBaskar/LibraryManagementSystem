


using LibraryManagementSystem.Infrastructure.Data;
using LibraryManagementSystem.Infrastructure.Repositories;
using LibraryManagementSystem.UseCases;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO;
using System;
using System.Text;
using LibraryManagementSystem.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args);


/*
appsettings.json: You've added a Jwt section to store the secret key.
JwtService.cs: The constructor now takes IConfiguration and reads the secret key from it using configuration["Jwt:SecretKey"].
Program.cs:
The hardcoded secret key is removed.
JwtService is registered without passing the key directly. The DI container will now inject IConfiguration into JwtService's constructor.
The connection string name is changed to DefaultConnection for AppDbContext.
The secret key is read from configuration in AddJwtBearer.
*/


// Explicitly load appsettings.json from the Presentation folder
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("Presentation/appsettings.json", optional: false, reloadOnChange: true);

var secretKey = builder.Configuration["Jwt:SecretKey"];

Console.WriteLine($"JWT Secret Key: {builder.Configuration["Jwt:SecretKey"]}");


if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT SecretKey is missing in appsettings.json.");
}

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

// 🔹 Configure SQL Server connection
// builder.Configuration.GetConnectionString("DefaultConnection"): This is the key change. It uses the IConfiguration object (which builder provides) to retrieve the connection string named "DefaultConnection" from appsettings.json.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔹 Configure SQL Server connection for ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// 🔹 Register services
builder.Services.AddScoped<IBookRepository,SqlBookRepository>();
builder.Services.AddScoped<LibraryService>();

// 🔹 Register AuthService 
builder.Services.AddScoped<AuthService>();

// 🔹 Register JwtService
builder.Services.AddScoped<JwtService>();


// 🔹 Enable controllers
builder.Services.AddControllers();

// 🔹 Enable Swagger for API testing
builder.Services.AddEndpointsApiExplorer();

/// <summary>
/// Configures Swagger to include JWT Bearer authentication support.
/// </summary>
/// <param name="c">The Swagger generator options.</param>
/// <remarks>
/// This code block performs the following:
///   1. Adds a security definition named "Bearer" to Swagger, describing how JWT Bearer authentication is used.
///   2. Specifies that the JWT token should be provided in the "Authorization" header.
///   3. Sets the security scheme type to HTTP with the "bearer" scheme and "JWT" format.
///   4. Adds a security requirement to apply the "Bearer" security definition to all endpoints, enabling the "Authorize" button in Swagger UI.
/// </remarks>

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",new OpenApiInfo { Title = "Library API", Version = "v1" });

    // 🔑 Add Security Definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // 🔑 Add Security Requirement
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

});

// 🔹 Enable Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// 🔹 Enable Swagger UI
//if (app.Environment.IsDevelopment()) // this will enable production
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//Enables Authentication & Authorization middleware.
app.UseAuthentication();
app.UseMiddleware<RoleAuthorizationMiddleware>(); // 🔹 Add Authorization Middleware
app.UseAuthorization();



app.MapControllers();
app.Run();