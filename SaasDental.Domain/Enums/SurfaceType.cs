namespace SaasDental.Domain.Enums;

public enum SurfaceType
{
    None = 0, // When finding affects the whole tooth, not a specific surface
    Oclusal = 1,
    Mesial = 2,
    Distal = 3,
    Vestibular = 4,
    Palatina = 5, // Or Lingual
    Lingual = 6 // Added for explicit clarity, though Palatina/Lingual are functionally the same face depending on maxilla/mandible
}
