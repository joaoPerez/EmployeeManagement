using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Infrastructure.Data;
using EmployeeManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EmployeeManagement.Domain.Entities;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
          .AllowAnyOrigin()    // Allow requests from any origin. Adjust this for production.
          .AllowAnyMethod()    // Allow GET, POST, PUT, DELETE, OPTIONS, etc.
          .AllowAnyHeader();   // Allow any header.
    });
});

// Configure logging.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.WebHost.UseUrls("http://0.0.0.0:80");

var connectionString = builder.Configuration.GetConnectionString("EmployeeDatabase");
builder.Services.AddDbContext<EmployeeContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddControllers();

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("JWT Key is not configured.")))
    };
});


// Enable Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Employee Management API", Version = "v1" });

    // Define the BearerAuth scheme that's in use
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                        Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Add a global security requirement which uses the defined scheme
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme 
            {
                Reference = new OpenApiReference 
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});


var app = builder.Build();

app.UseCors("AllowAll");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<EmployeeContext>();
        
        if (context.Database.EnsureCreated())
        {
            Console.WriteLine("Database created successfully.");
        }
        else
        {
            Console.WriteLine("Database already exists.");
        }

        if (!context.Employees.Any(e => e.Email == "admin@example.com"))
        {
            var employeeService = services.GetRequiredService<IEmployeeService>();

            var result = await employeeService.CreateEmployeeAsync(
                firstName: "Admin",
                lastName: "User",
                email: "admin@example.com",
                documentNumber: "ADMIN001",
                phones: ["1234567890"],
                role: Role.Director,
                password: "adminpassword",
                birthDate: new DateTime(1980, 1, 1),
                managerId: null,
                creatorRole: Role.Director
            );

            if (!result.IsSuccess)
            {
                var errors = string.Join(", ", result.Errors);
                Console.WriteLine("Failed to seed admin user: " + errors);
            }
            else
            {
                Console.WriteLine("Default admin user seeded successfully.");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred seeding the DB: " + ex.Message);
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Management API V1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();