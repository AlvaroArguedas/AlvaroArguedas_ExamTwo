using ExamTwo.Data;
using ExamTwo.Models;

namespace ExamTwo.Services
{
    public class CoffeeMachineService : ICoffeeMachineService
    {
        private readonly Database _db;
        private readonly PaymentValidator _paymentValidator;
        private readonly OrderValidator _orderValidator;
        private readonly ChangeCalculator _changeCalculator;

        public CoffeeMachineService(Database db)
        {
            _db = db;
            _paymentValidator = new PaymentValidator();
            _orderValidator = new OrderValidator(db);
            _changeCalculator = new ChangeCalculator();
        }

        public Dictionary<string, int> GetCoffees() => _db.keyValues;

        public Dictionary<string, int> GetCoffeePrices() => _db.keyValues2;

        public Dictionary<int, int> GetCoinInventory() => _db.keyValues3;

        public PurchaseResult BuyCoffee(OrderRequest request)
        {
            var orderCheck = _orderValidator.Validate(request);
            if (!orderCheck.Success) return orderCheck;

            if (_db.keyValues3.Values.All(q => q == 0))
                return Fail("Fuera de servicio");

            var paymentCheck = _paymentValidator.Validate(request.Payment);
            if (!paymentCheck.Success) return paymentCheck;

            int totalCost = request.Items.Sum(i => _db.keyValues2[i.Type] * i.Quantity);
            int totalPaid = request.Payment.TotalAmount;

            if (totalPaid < totalCost)
                return Fail("Dinero insuficiente.");

            int changeNeeded = totalPaid - totalCost;

            var change = _changeCalculator.Calculate(changeNeeded, _db.keyValues3);
            if (changeNeeded > 0 && change == null)
                return Fail("Fallo al realizar la compra.");

            foreach (var i in request.Items)
                _db.keyValues[i.Type] -= i.Quantity;

            if (change != null)
                foreach (var c in change)
                    _db.keyValues3[c.Key] -= c.Value;

            return new PurchaseResult
            {
                Success = true,
                Message = "Compra realizada con éxito.",
                TotalChange = changeNeeded,
                ChangeBreakdown = change ?? new Dictionary<int, int>()
            };
        }

        private PurchaseResult Fail(string m) => new()
        {
            Success = false,
            Message = m
        };

        
    }
}
