using System.Text;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var JwtSetting=builder.Configuration.GetSection("JWTSetting");


builder.Services.AddDbContext<ApDbContext>(x=>x.UseSqlite("Data Source=chat.db"));//add DbContext name as ApDbContext to connect with database
builder.Services.AddIdentityCore<AppUser>()//add User class name AppUser into it and interacting with AppDbContext
.AddEntityFrameworkStores<ApDbContext>()
.AddDefaultTokenProviders();



builder.Services.AddAuthentication(opt=>{
    opt.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme=JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opt=>{
    opt.SaveToken=true;
    opt.RequireHttpsMetadata=false;
    opt.TokenValidationParameters=new TokenValidationParameters
    {
        ValidateIssuerSigningKey=true,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSetting.GetSection("SecurityKey").Value!)),
        ValidateIssuer=false,
        ValidateAudience=false
    };
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.Run();


