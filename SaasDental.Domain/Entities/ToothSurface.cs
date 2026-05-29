using SaasDental.Domain.Common;
using SaasDental.Domain.Enums;

namespace SaasDental.Domain.Entities;

public class ToothSurface : BaseEntity
{
    public SurfaceType SurfaceType { get; private set; }

    public Guid ToothId { get; private set; }
    public Tooth Tooth { get; private set; } = null!;

    public ICollection<ClinicalFinding> Findings { get; private set; } = new List<ClinicalFinding>();

    private ToothSurface() { }

    public ToothSurface(SurfaceType surfaceType, Guid toothId)
    {
        SurfaceType = surfaceType;
        ToothId = toothId;
    }
}
