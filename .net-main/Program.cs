using Data.Context;
using Data.Repositories;
using Data.Repositories.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Configurations.Mediator.Interfaces;
using Services.Interfaces;
using Services.Services;
using System.Text;
using AccessControl.Seeding;
using AccessControl.Middlewares;
using Domain.Config;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

#region SMTP Email Client
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
#endregion

#region Application Insights (Telemetry)
builder.Services.AddApplicationInsightsTelemetry(); // Enables telemetry for monitoring
#endregion

#region Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(); // Checks database connectivity

builder.Services.AddHealthChecksUI(setup =>
{
    setup.SetHeaderText("AccessControl - Health Check UI");
})
.AddInMemoryStorage(); // In-memory history for health check results
#endregion

#region Identity & Authentication
// Adds ASP.NET Core Identity with EF Core
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT authentication setup
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(cfg =>
{
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("OnlyCompanyEmail", policy =>
        policy.Requirements.Add(new EmailDomainRequirement("@access.com")));
});
#endregion

#region Dependency Injection - Services & Repositories
// Services
builder.Services.AddScoped<IDoorService, DoorService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccessMediator, AccessMediator>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Repositories
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<IDoorRepository, DoorRepository>();

// Custom auth handler
builder.Services.AddSingleton<IAuthorizationHandler, EmailDomainHandler>();
#endregion

#region Database (In-Memory)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("accessControl")); // Temporary in-memory DB
#endregion

#region Swagger (OpenAPI)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AccessControl API", Version = "v1" });

    // Adds JWT auth to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
#endregion

#region MVC Controllers & Endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
#endregion

var app = builder.Build();

#region Global Middlewares (Exception Handling, Timing, Swagger, etc.)
app.UseGlobalExceptionMiddleware(); // Global exception handler

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AccessControl API v1");
    c.RoutePrefix = string.Empty; // Swagger at root
});

app.UseHttpsRedirection();

app.UseRouting(); // Required before auth middlewares

app.UseRequestTiming(); // Logs request duration

app.UseAuthentication();
app.UseAuthorization();
#endregion

#region Health Checks Endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
    endpoints.MapHealthChecksUI(options =>
    {
        options.UIPath = "/health-ui";
    });
});
#endregion

#region Seed Admin User on Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureDeletedAsync(); 
    await dbContext.Database.EnsureCreatedAsync(); 

    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentitySeeder.SeedAsync(userManager, roleManager);
}

#endregion

app.Run();


public partial class Program { }
