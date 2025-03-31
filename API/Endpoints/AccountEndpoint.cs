using System;
using API.Common;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class AccountEndpoint
{
      public static RouteGroupBuilder MapAccountEndpoint(this WebApplication app)
      {
        var group=app.MapGroup("/api/account").WithTags("account");

       group.MapPost("/register", async (HttpContext context, UserManager<AppUser> userManager,
    [FromForm] string fullName, [FromForm] string email, [FromForm] string password,[FromForm] IFormFile? profileImage) =>
{
    Console.WriteLine($"Attempting to register user: {email}");

    var userFromDb = await userManager.FindByEmailAsync(email);
    if (userFromDb is not null)
    {
        Console.WriteLine("User already exists!");
        return Results.BadRequest(Response<string>.Failure("User already exists."));
    }

    if(profileImage is null){
        return Results.BadRequest(Response<string>.Failure("Profile Image is required"));
    }

    var picture=await FileUpload.Upload(profileImage);

    picture=$"{context.Request.Scheme}://{context.Request.Host}/uploads/{picture}";


    var user = new AppUser
    {
        Email = email,
        UserName = email,  // 👈 Required for Identity
        NormalizedEmail = email.ToUpper(), // 👈 Ensures FindByEmail works
        FullName = fullName,
        ProfileImage=picture
    };

    var result = await userManager.CreateAsync(user, password);
    
    if (!result.Succeeded)
    {
        Console.WriteLine("Registration failed!");
        foreach (var error in result.Errors)
        {
            Console.WriteLine($"Error: {error.Description}");
        }
        return Results.BadRequest(Response<string>.Failure(result.Errors.Select(x => x.Description).FirstOrDefault()!));
    }

    Console.WriteLine("User created successfully!");
    return Results.Ok(Response<string>.Success("", "User created successfully."));
}).DisableAntiforgery();


        return group;
      }
} 
