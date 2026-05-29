namespace SaasDental.Domain.Enums;

public enum DebtStatus
{
    Pending = 1, // Nada pagado
    Partial = 2, // Pago parcial (en cuotas)
    Paid = 3 // Pagado totalmente
}
