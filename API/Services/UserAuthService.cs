using System;
using API.Data;
using API.DTOs.User;
using API.Exceptions;
using API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UserAuthService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public UserAuthService(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<UserDto> SignInAsync(UserSigninDto dto)
    {
        CryptoUtils _crypto = new CryptoUtils();

        var user = await _context.Users
           .Where(u => u.Username == dto.Username)
           .Select(u => new UserDto
           {
               Id = u.Id,
               Username = u.Username,
               Email = u.Email,
               IsActive = u.IsActive,
               Password = u.Password,
               PasswordSalt = u.PasswordSalt
           })
           .FirstOrDefaultAsync();

        if (user == null)
            throw new ValidationException("Item not found.");

        if (user.IsActive == false)
            throw new ValidationException("Invalid user.");

        if (user.Password != _crypto.HashPassword(dto.Password, user.PasswordSalt))
            throw new ValidationException("Invalid password.");

        user.Token = _tokenService.CreateToken(user);

        return user;
    }
}
