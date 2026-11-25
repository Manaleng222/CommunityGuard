using CommunityGuard.Api.Data;
using CommunityGuard.Api.Models;
using CommunityGuard.Api.Services;
using CommunityGuard.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. DATABASE (SQLITE) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

// --- 2. IDENTITY & AUTH ---
builder.Services.AddIdentityCore<ApplicationUser>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
})
.AddRoles<IdentityRole<Guid>>()
.AddEntityFrameworkStores<AppDbContext>()
.AddSignInManager();

builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IResourceService, ResourceService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 3. MIDDLEWARE PIPELINE ---
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --- 4. SEED ROLES & ADMIN USER (Runs on startup) ---
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // 1. Create the Roles
    if (!await roleManager.RoleExistsAsync("Controller"))
        await roleManager.CreateAsync(new IdentityRole<Guid>("Controller"));

    if (!await roleManager.RoleExistsAsync("Responder"))
        await roleManager.CreateAsync(new IdentityRole<Guid>("Responder"));

    // 2. Create the Default Admin User
    var adminEmail = "admin@communityguard.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    // Only create if the user doesn't already exist
    if (adminUser == null)
    {
        var newAdmin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "System Administrator",
            EmailConfirmed = true // CRITICAL: Prevents login failure
        };

        // Create the user with the password "Admin123!"
        var createResult = await userManager.CreateAsync(newAdmin, "Admin123!");

        // If successful, assign them the "Controller" role
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdmin, "Controller");
        }
    }
}

app.Run();


public partial class Program { }