using System.Text;
using API.Data;
using API.Endpoints;
using API.Models;
using API.Services; // ✅ Added for TokenService
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // ✅ Added for Swagger

var builder = WebApplication.CreateBuilder(args);

// Load JWT Settings from configuration
var securityKey = builder.Configuration["JWTSetting:SecurityKey"];
if (string.IsNullOrEmpty(securityKey))
{
    throw new InvalidOperationException("SecurityKey is missing in configuration.");
}

// Add DbContext (using SQLite here, can switch to SQL Server in prod)
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlite("Data Source=chat.db"));

// Add Identity core services
builder.Services.AddIdentityCore<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddSignInManager<SignInManager<AppUser>>();

// ✅ Register custom TokenService
builder.Services.AddScoped<TokenService>();

// Configure JWT Authentication
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

// Add Authorization
builder.Services.AddAuthorization();

// Add Controllers
builder.Services.AddControllers();

// ✅ Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
/*builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chat API", Version = "v1" });
});*/

// Build the app
var app = builder.Build();

// Middleware configuration
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseHttpsRedirection();

app.UseAuthentication(); // ✅ Order matters
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

// ✅ Register custom Account endpoint (extension method)
app.MapAccountEndpoint();

app.Run();
