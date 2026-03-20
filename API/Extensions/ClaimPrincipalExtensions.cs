using System;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimPrincipalExtensions
{
    public static string GetUsername(this ClaimsPrincipal user)
    {
        try
        {
            var username = user.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("Cannot get username from token");
            return username.Decrypt();
        }
        catch
        {
            throw new Exception("Invalid Token for username");
        }
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        try
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Cannot get userId from token");
            return int.Parse(userId.Decrypt());
        }
        catch
        {
            throw new Exception("Invalid Token for userId");
        }
    }
}
