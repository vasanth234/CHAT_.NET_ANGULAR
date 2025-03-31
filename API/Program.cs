using System.Text;
using API.Data;
using API.Endpoints;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Load JWT Settings
var securityKey = builder.Configuration["JWTSetting:SecurityKey"];
if (string.IsNullOrEmpty(securityKey))
{
    throw new InvalidOperationException("SecurityKey is missing in configuration.");
}

// Add DB Context
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlite("Data Source=chat.db"));

// Add Identity
builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Configure Authentication
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.SaveToken = true;
    opt.RequireHttpsMetadata = false;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers(); // Ensure controllers work properly
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection(); // Temporarily disable if causing issues
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers(); // Ensure controllers are mapped

Console.WriteLine("Registering AccountEndpoint...");
app.MapAccountEndpoint();
Console.WriteLine("AccountEndpoint registered.");

app.Run();
