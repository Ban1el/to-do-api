using System;
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

    public async Task<ToDoDto> CreateAsync(ToDoCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var todo = new ToDo
            {
                Title = dto.Title,
                Description = dto.Description,
                DateCreated = DateTime.UtcNow,
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
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<ToDoDto> UpdateAsync(int id, ToDoUpdateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var todo = await _context.ToDos.FindAsync(id);

            if (todo == null)
                throw new NotFoundException("Item not found");

            // update the fields
            todo.Title = dto.Title;
            todo.Description = dto.Description;
            todo.IsActive = dto.IsActive;
            todo.DateModified = DateTime.UtcNow;

            _context.ToDos.Update(todo);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new ToDoDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                DateCreated = todo.DateCreated,
                DateModified = todo.DateModified,
                IsActive = todo.IsActive
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var todo = await _context.ToDos.FindAsync(id);

            if (todo == null)
                throw new NotFoundException("Item not found");

            _context.ToDos.Remove(todo);
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