using Microsoft.EntityFrameworkCore;
using SaasDental.Application.Common.Interfaces;
using SaasDental.Domain.Entities;
using SaasDental.Domain.Enums;
using SaasDental.Infrastructure.Persistence;

namespace SaasDental.Infrastructure.Repositories;

public class ClinicalRepository : IClinicalRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ClinicalRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClinicalHistory?> GetHistoryByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClinicalHistories
            .FirstOrDefaultAsync(ch => ch.PatientId == patientId, cancellationToken);
    }

    public async Task<Odontogram?> GetOdontogramByIdAsync(Guid odontogramId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Odontograms
            .Include(o => o.Teeth)
                .ThenInclude(t => t.Surfaces)
            .Include(o => o.Teeth)
                .ThenInclude(t => t.ToothLevelFindings)
            .FirstOrDefaultAsync(o => o.Id == odontogramId, cancellationToken);
    }

    public async Task<Odontogram?> GetInitialOdontogramAsync(Guid clinicalHistoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Odontograms
            .Include(o => o.Teeth)
                .ThenInclude(t => t.Surfaces)
            .Include(o => o.Teeth)
                .ThenInclude(t => t.ToothLevelFindings)
            .FirstOrDefaultAsync(o => o.ClinicalHistoryId == clinicalHistoryId && o.VersionType == OdontogramVersionType.Initial, cancellationToken);
    }

    public async Task AddHistoryAsync(ClinicalHistory history, CancellationToken cancellationToken = default)
    {
        await _dbContext.ClinicalHistories.AddAsync(history, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateHistoryAsync(ClinicalHistory history, CancellationToken cancellationToken = default)
    {
        _dbContext.ClinicalHistories.Update(history);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddOdontogramAsync(Odontogram odontogram, CancellationToken cancellationToken = default)
    {
        await _dbContext.Odontograms.AddAsync(odontogram, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddClinicalFindingAsync(ClinicalFinding finding, CancellationToken cancellationToken = default)
    {
        await _dbContext.ClinicalFindings.AddAsync(finding, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
