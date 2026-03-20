using System;
using API.Data;
using API.DTOs;
using API.DTOs.User;
using API.Exceptions;
using API.Models;
using API.Utilities;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDetailDto>> GetAllAsync()
    {
        return await _context.Users
        .Where(u => u.IsActive == true)
        .Select(u => new UserDetailDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            IsActive = u.IsActive,
            CreatedBy = u.CreatedBy,
            DateCreated = u.DateCreated,
            ModifiedBy = u.ModifiedBy,
            DateModified = u.DateModified
        })
        .ToListAsync();
    }

    public async Task<UserDetailDto?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Where(t => t.IsActive == true && t.Id == id)
              .Select(u => new UserDetailDto
              {
                  Id = u.Id,
                  Username = u.Username,
                  Email = u.Email,
                  IsActive = u.IsActive,
                  CreatedBy = u.CreatedBy,
                  DateCreated = u.DateCreated,
                  ModifiedBy = u.ModifiedBy,
                  DateModified = u.DateModified
              })
            .FirstOrDefaultAsync();
    }

    public async Task<UserDetailDto> CreateAsync(int userId, UserCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            CryptoUtils _cryptoUtils = new CryptoUtils();
            PasswordUtils _passwordUtils = new PasswordUtils();
            var (isStrong, message) = _passwordUtils.CheckStrength(dto.password);

            if (!isStrong) throw new ValidationException(message);

            string passwordSalt = _cryptoUtils.GenerateSalt();
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                DateCreated = DateTime.UtcNow,
                Password = _cryptoUtils.HashPassword(dto.password, passwordSalt),
                IsActive = true
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new UserDetailDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedBy = userId,
                DateCreated = user.DateCreated,
                IsActive = user.IsActive
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<UserDetailDto> UpdateAsync(int userId, int id, UserUpdateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                throw new NotFoundException("Item not found");

            user.Username = dto.Username;
            user.Email = dto.Email;
            user.IsActive = dto.IsActive;
            user.ModifiedBy = userId;
            user.DateModified = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new UserDetailDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                ModifiedBy = userId,
                DateModified = user.DateModified,
                CreatedBy = user.CreatedBy,
                DateCreated = user.DateCreated,
                IsActive = user.IsActive
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(int userId, int id)
    {
        if (userId == id) throw new ValidationException("Invalid user");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                throw new NotFoundException("Item not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
