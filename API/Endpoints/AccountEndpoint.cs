using System;
using API.Common;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using API.DTOs;


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

     group.MapPost("/login",async(UserManager<AppUser> userManager,
     TokenService tokenService,LoginDTO dto)=>{
        if(dto is null){
            return Results.BadRequest(Response<string>.Failure("Invaild Login Details"));
        }

        var user=await userManager.FindByEmailAsync(dto.Email);

        if(user is null){
            return Results.BadRequest(Response<string>.Failure("User not found"));

        }

        var result=await userManager.CheckPasswordAsync(user!,dto.Password);
        if(!result){
            return Results.BadRequest(Response<string>.Failure("Invalid password"));
        }

        var token=tokenService.GenerateToken(user.Id,user.UserName!);
        return Results.Ok(Response<string>.Success(token,"Login successfully"));

     });


        return group;
      }
} 
