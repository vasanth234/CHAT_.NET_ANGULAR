using System;
using API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ApDbContext:IdentityDbContext<AppUser>
{
    public ApDbContext(DbContextOptions<ApDbContext>options):base(options)
    {

    }
}