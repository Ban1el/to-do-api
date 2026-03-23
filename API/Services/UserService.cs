using System;
using API.Common;
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

    public async Task<ServiceResult<UserDetailDto>> CreateAsync(int userId, UserCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var result = new ServiceResult<UserDetailDto>();

            bool usernameExists = await _context.Users
                .AnyAsync(u => u.Username == dto.Username);

            if (usernameExists) return ServiceResult<UserDetailDto>.Fail("Username already exists");

            bool emailExists = await _context.Users
               .AnyAsync(u => u.Email == dto.Email);

            if (emailExists) return ServiceResult<UserDetailDto>.Fail("Email already exists");

            CryptoUtils _cryptoUtils = new CryptoUtils();
            PasswordUtils _passwordUtils = new PasswordUtils();
            var (isStrong, message) = _passwordUtils.CheckStrength(dto.password);

            if (!isStrong) return ServiceResult<UserDetailDto>.Fail(message);

            string passwordSalt = _cryptoUtils.GenerateSalt();
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                Password = _cryptoUtils.HashPassword(dto.password, passwordSalt),
                PasswordSalt = passwordSalt,
                IsActive = true
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new ServiceResult<UserDetailDto>
            {
                Success = true,
                Data = new UserDetailDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedBy = user.CreatedBy,
                    DateCreated = user.DateCreated,
                    ModifiedBy = user.ModifiedBy,
                    DateModified = user.DateModified,
                    IsActive = user.IsActive
                }
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ServiceResult<UserDetailDto>> UpdateAsync(int userId, int id, UserUpdateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            bool detailsChanged = false;
            var result = new ServiceResult<UserDetailDto>();
            var user = await _context.Users.FindAsync(id);
            CryptoUtils _cryptoUtils = new CryptoUtils();
            PasswordUtils _passwordUtils = new PasswordUtils();

            if (user == null) return ServiceResult<UserDetailDto>.Fail("Item not found.");
            #region VALIDATION

            #region Username
            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.Username) detailsChanged = true;
            else if (string.IsNullOrWhiteSpace(dto.Username)) dto.Username = user.Username;
            #endregion

            #region Email
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email) detailsChanged = true;
            else if (string.IsNullOrWhiteSpace(dto.Email)) dto.Email = user.Email;
            #endregion

            #region Password
            string newPasswordHashed = _cryptoUtils.HashPassword(dto.NewPassword, user.PasswordSalt);

            if (!string.IsNullOrWhiteSpace(dto.NewPassword) && newPasswordHashed != user.Password)
            {
                if (string.IsNullOrWhiteSpace(dto.CurrentPassword)) return ServiceResult<UserDetailDto>.Fail("Enter current password to update password.");

                string currentPasswordHashed = _cryptoUtils.HashPassword(dto.CurrentPassword, user.PasswordSalt);

                if (currentPasswordHashed != user.Password) return ServiceResult<UserDetailDto>.Fail("Invalid current password.");

                var (isStrong, message) = _passwordUtils.CheckStrength(dto.NewPassword);
                if (!isStrong) return ServiceResult<UserDetailDto>.Fail(message);

                detailsChanged = true;
            }
            else if (string.IsNullOrWhiteSpace(dto.NewPassword)) dto.NewPassword = user.Password;
            #endregion

            #region IsActive
            if (dto.IsActive != null && dto.IsActive != user.IsActive)
            {
                if (userId == user.Id) return ServiceResult<UserDetailDto>.Fail("You cannot change your own active status while logged in.");

                detailsChanged = true;
            }
            else if (dto.IsActive == null) dto.IsActive = user.IsActive;
            #endregion

            #endregion

            if (detailsChanged)
            {
                user.Username = dto.Username;
                user.Password = newPasswordHashed;
                user.Email = dto.Email;
                user.IsActive = dto.IsActive.Value;
                user.ModifiedBy = userId;
                user.DateModified = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            else
            {
                return ServiceResult<UserDetailDto>.Ok("Nothing to update.");
            }

            return new ServiceResult<UserDetailDto>
            {
                Success = true,
                Data = new UserDetailDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    ModifiedBy = userId,
                    DateModified = user.DateModified,
                    CreatedBy = user.CreatedBy,
                    DateCreated = user.DateCreated,
                    IsActive = user.IsActive
                }
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ServiceResult> DeleteAsync(int userId, int id)
    {
        if (userId == id) return ServiceResult.Fail("Invalid user.");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return ServiceResult.Fail("Item not found.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ServiceResult.Ok();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
