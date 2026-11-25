using CommunityGuard.Api.Data;
using CommunityGuard.Domain.Entities;
using CommunityGuard.Domain.Exceptions;
using CommunityGuard.Shared.Common;
using CommunityGuard.Shared.Dtos;
using CommunityGuard.Shared.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CommunityGuard.Api.Services;

public class IncidentService : IIncidentService
{
    private readonly AppDbContext _context;

    public IncidentService(AppDbContext context) => _context = context;

    public async Task<Guid> CreateAsync(CreateIncidentRequest request, Guid userId)
    {
        // 1. Use the Domain factory method to create the entity.
        // This automatically enforces the validation rules from Phase 1.
        var incident = Incident.Report(
            request.Title, request.Description, request.Sector, request.Priority, userId);

        // 2. Add to DbContext and save.
        _context.Incidents.Add(incident);
        await _context.SaveChangesAsync();

        return incident.Id;
    }

    public async Task<PagedResult<IncidentDto>> GetPagedAsync(int page, int pageSize, Guid userId, bool isController)
    {
        // Start with the base query
        var query = _context.Incidents.AsQueryable();

        // RBAC RULE: If they are NOT a Controller, only show incidents assigned to them
        if (!isController)
        {
            query = query.Where(i => i.AssignedToUserId == userId);
        }

        // Apply ordering and paging
        var finalQuery = query
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new IncidentDto(
                i.Id, i.Title, i.Description, i.Sector, i.Priority, i.Status, i.CreatedAt));

        return await finalQuery.ToPagedResultAsync(page, pageSize);
    }

    public async Task UpdateStatusAsync(Guid incidentId, UpdateIncidentStatusRequest request, Guid userId)
    {
        // 1. Find the incident.
        var incident = await _context.Incidents.FindAsync(incidentId);
        if (incident == null) throw new DomainException("Incident not found.");

        // 2. Call the Domain state machine.
        // If the transition is illegal (e.g., Closed -> Reported), this throws a DomainException.
        incident.TransitionTo(request.NewStatus, userId, request.Note);

        // 3. Save changes.
        await _context.SaveChangesAsync();
    }

    public async Task<IncidentDetailDto?> GetByIdAsync(Guid id)
    {
        var incident = await _context.Incidents
            .Include(i => i.History)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        if (incident == null) return null;

        return new IncidentDetailDto(
            incident.Id,
            incident.Title,
            incident.Description,
            incident.Sector,
            incident.Priority,
            incident.Status,
            incident.CreatedAt,
            incident.History
                .OrderBy(h => h.ChangedAt)
                .Select(h => new IncidentHistoryDto(h.Status, h.ChangedAt, h.ChangedByUserId, h.Note))
                .ToList()
        );
    }
}