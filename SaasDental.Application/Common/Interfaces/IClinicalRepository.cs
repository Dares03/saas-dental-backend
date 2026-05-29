using SaasDental.Domain.Entities;

namespace SaasDental.Application.Common.Interfaces;

public interface IClinicalRepository
{
    Task<ClinicalHistory?> GetHistoryByPatientIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<Odontogram?> GetOdontogramByIdAsync(Guid odontogramId, CancellationToken cancellationToken = default);
    Task<Odontogram?> GetInitialOdontogramAsync(Guid clinicalHistoryId, CancellationToken cancellationToken = default);
    
    Task AddHistoryAsync(ClinicalHistory history, CancellationToken cancellationToken = default);
    Task UpdateHistoryAsync(ClinicalHistory history, CancellationToken cancellationToken = default);
    
    Task AddOdontogramAsync(Odontogram odontogram, CancellationToken cancellationToken = default);
    Task AddClinicalFindingAsync(ClinicalFinding finding, CancellationToken cancellationToken = default);
    
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
