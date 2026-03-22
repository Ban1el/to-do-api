using System;
using API.Data;
using API.DTOs.AudiTrail;
using API.Models;

namespace API.Services;

public class AuditTrailService
{
    private readonly AppDbContext _context;

    public AuditTrailService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(AuditTrailCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var auditTrail = new AuditTrail
            {
                UserId = dto.UserId,
                Module = dto.Module,
                Action = dto.Action,
                Method = dto.Method,
                Description = dto.Description,
                Data = dto.Data,
                ClientIpAddress = dto.ClientIpAddress,
                IsRequest = dto.IsRequest,
                Path = dto.Path,
                RefId = dto.RefId,
                DateCreated = dto.DateCreated
            };

            await _context.AuditTrails.AddAsync(auditTrail);
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
