using System;
using API.Common;
using API.Data;
using API.DTOs;
using API.DTOs.ErrorLog;
using API.Exceptions;
using API.Models;
using Microsoft.EntityFrameworkCore;
namespace API.Services;

public class ErrorLogService
{
    private readonly AppDbContext _context;

    public ErrorLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(ErrorLogCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var errorLog = new ErrorLog
            {
                UserId = dto.UserId,
                Description = dto.Description,
                ClientIpAddress = dto.ClientIpAddress,
                DateCreated = dto.DateCreated
            };

            await _context.ErrorLogs.AddAsync(errorLog);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
