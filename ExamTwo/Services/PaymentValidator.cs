using ExamTwo.Models;

namespace ExamTwo.Services
{
    public class PaymentValidator
    {
        private static readonly HashSet<int> AllowedCoins = new() { 500, 100, 50, 25 };
        private static readonly HashSet<int> AllowedBills = new() { 1000 };

        public PurchaseResult Validate(Payment p)
        {
            if (p == null)
                return Fail("Pago no válido.");

            if (p.Coins.Any(c => !AllowedCoins.Contains(c)))
                return Fail("Moneda no permitida.");

            if (p.Bills.Any(b => !AllowedBills.Contains(b)))
                return Fail("Solo se aceptan billetes de 1000 colones.");

            return Success();
        }

        private PurchaseResult Success() => new() { Success = true };

        private PurchaseResult Fail(string m) => new()
        {
            Success = false,
            Message = m
        };
    }

}
