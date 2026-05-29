namespace SaasDental.Domain.Enums;

public enum PaymentMethod
{
    Cash = 1, // Efectivo
    CreditCard = 2, // Tarjeta de Crédito
    DebitCard = 3, // Tarjeta de Débito
    BankTransfer = 4, // Transferencia Bancaria
    Other = 5 // Otros (Billeteras digitales, etc)
}
