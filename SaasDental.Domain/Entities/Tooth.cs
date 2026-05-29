using SaasDental.Domain.Common;

namespace SaasDental.Domain.Entities;

public class Tooth : BaseEntity
{
    public int ToothNumber { get; private set; } // FDI format: 11-48, 51-85

    public Guid OdontogramId { get; private set; }
    public Odontogram Odontogram { get; private set; } = null!;

    public ICollection<ToothSurface> Surfaces { get; private set; } = new List<ToothSurface>();
    public ICollection<ClinicalFinding> ToothLevelFindings { get; private set; } = new List<ClinicalFinding>();

    private Tooth() { }

    public Tooth(int toothNumber, Guid odontogramId)
    {
        ToothNumber = toothNumber;
        OdontogramId = odontogramId;
    }
}
