using System;
using API.Common;
using API.Data;
using API.DTOs;
using API.Exceptions;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class ToDoService
{
    private readonly AppDbContext _context;

    public ToDoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ToDoDto>> GetAllAsync()
    {
        return await _context.ToDos
        .Where(t => t.IsActive == true)
        .Select(t => new ToDoDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            DateCreated = t.DateCreated,
            DateModified = t.DateModified,
            IsActive = t.IsActive
        })
        .ToListAsync();
    }

    public async Task<ToDoDto?> GetByIdAsync(int id)
    {
        return await _context.ToDos
            .Where(t => t.IsActive == true && t.Id == id)
            .Select(t => new ToDoDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                DateCreated = t.DateCreated,
                DateModified = t.DateModified,
                IsActive = t.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<ToDoDto> CreateAsync(int userId, ToDoCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var todo = new ToDo
            {
                Title = dto.Title,
                Description = dto.Description,
                DateCreated = DateTime.UtcNow,
                CreatedBy = userId,
                IsActive = true
            };

            await _context.ToDos.AddAsync(todo);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new ToDoDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                DateCreated = todo.DateCreated,
                IsActive = todo.IsActive
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ServiceResult<ToDoDto>> UpdateAsync(int userId, int id, ToDoUpdateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var result = new ServiceResult<ToDoDto>();
            var todo = await _context.ToDos.FindAsync(id);

            if (todo == null) return ServiceResult<ToDoDto>.Fail("Item not found");

            bool detailsChanged = false;
            #region VALIDATION

            #region Title
            if (!string.IsNullOrWhiteSpace(dto.Title) && dto.Title != todo.Title) detailsChanged = true;
            else if (string.IsNullOrWhiteSpace(dto.Title)) dto.Title = todo.Title;
            #endregion

            #region Description
            if (!string.IsNullOrWhiteSpace(dto.Description) && dto.Description != todo.Description) detailsChanged = true;
            else if (string.IsNullOrWhiteSpace(dto.Description)) dto.Description = todo.Description;
            #endregion

            #region IsActive
            if (dto.IsActive != null && dto.IsActive != todo.IsActive) detailsChanged = true;
            else if (dto.IsActive == null) dto.IsActive = todo.IsActive;
            #endregion

            #endregion

            if (detailsChanged)
            {
                todo.Title = dto.Title;
                todo.Description = dto.Description;
                todo.IsActive = dto.IsActive.Value;
                todo.DateModified = DateTime.UtcNow;
                todo.ModifiedBy = userId;

                _context.ToDos.Update(todo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            else
            {
                return ServiceResult<ToDoDto>.Ok("Nothing to update.");
            }

            return new ServiceResult<ToDoDto>
            {
                Success = true,
                Data = new ToDoDto
                {
                    Id = todo.Id,
                    Title = todo.Title,
                    Description = todo.Description,
                    DateCreated = todo.DateCreated,
                    DateModified = todo.DateModified,
                    IsActive = todo.IsActive
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
            var todo = await _context.ToDos.FindAsync(id);

            if (todo == null) return ServiceResult.Fail("Item not found.");

            _context.ToDos.Remove(todo);
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