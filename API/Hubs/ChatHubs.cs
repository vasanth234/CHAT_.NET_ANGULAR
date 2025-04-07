using System;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR; // for Hub
using Microsoft.AspNetCore.Identity; // for UserManager<T>


namespace API.Hubs;


[Authorize]
public class ChatHub(UserManager<AppUser> userManager,AppDbContext context) : Hub
{
   public static readonly ConcurrentDictionary<string,OnlineUserDto>
   onlineUsers=new();
}
