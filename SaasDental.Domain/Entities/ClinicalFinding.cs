using SaasDental.Domain.Common;
using SaasDental.Domain.Enums;

namespace SaasDental.Domain.Entities;

public class ClinicalFinding : BaseEntity
{
    public string FindingType { get; private set; } = string.Empty; // e.g., Caries, Corona, Fractura
    public FindingColor Color { get; private set; }
    public string Nomenclature { get; private set; } = string.Empty; // e.g., CC, AM, TC, DES

    // Optional relationships. A finding can be at the Tooth level (e.g. absent tooth) or Surface level (e.g. caries on Oclusal)
    public Guid? ToothId { get; private set; }
    public Tooth? Tooth { get; private set; }

    public Guid? ToothSurfaceId { get; private set; }
    public ToothSurface? ToothSurface { get; private set; }

    private ClinicalFinding() { }

    // Constructor for Tooth-level findings (e.g., Extracted, Crown)
    public ClinicalFinding(string findingType, FindingColor color, string nomenclature, Guid toothId)
    {
        FindingType = findingType;
        Color = color;
        Nomenclature = nomenclature;
        ToothId = toothId;
    }

    // Constructor for Surface-level findings (e.g., Caries, Restoration on specific face)
    public ClinicalFinding(string findingType, FindingColor color, string nomenclature, Guid toothId, Guid toothSurfaceId)
    {
        FindingType = findingType;
        Color = color;
        Nomenclature = nomenclature;
        ToothId = toothId; // Keep reference to the tooth for easier querying
        ToothSurfaceId = toothSurfaceId;
    }

    public void UpdateColor(FindingColor newColor)
    {
        // Changing from Red to Blue means the treatment was completed
        Color = newColor;
        UpdatedAt = DateTime.UtcNow;
    }
}
