using ExamTwo.Data;
using ExamTwo.Models;

namespace ExamTwo.Services
{
    public class CoffeeMachineService : ICoffeeMachineService
    {
        private readonly Database _db;

        public CoffeeMachineService(Database db)
        {
            _db = db;
        }

        public Dictionary<string, int> GetCoffees()
            => _db.keyValues;

        public Dictionary<string, int> GetCoffeePrices()
            => _db.keyValues2;

        public Dictionary<int, int> GetCoinInventory()
            => _db.keyValues3;

        public string BuyCoffee(OrderRequest request)
        {
            if (request.Order == null || request.Order.Count == 0)
                return "Ordem vacia.";

            if (request.Payment.TotalAmount <= 0)
                return "Dinero insuficiente ";

            var costoTotal = request.Order.Sum(o => _db.keyValues2.First(c => c.Key == o.Key).Value * o.Value);

            if (request.Payment.TotalAmount < costoTotal)
                return "Dinero insuficiente ";

            foreach (var cafe in request.Order)
            {
                var selected = _db.keyValues.First(c => c.Key == cafe.Key).Key;
                if (cafe.Value > _db.keyValues[selected])
                {
                    return $"No hay suficientes {selected} en la máquina.";
                }
                _db.keyValues[selected] -= cafe.Value;
            }

            var change = request.Payment.TotalAmount - costoTotal;
            string result = $"Su vuelto es de: {change} colones. Desglose:";

            foreach (var coin in _db.keyValues3.Keys.OrderByDescending(c => c))
            {
                var count = Math.Min(change / coin, _db.keyValues3[coin]);
                if (count > 0)
                {
                    result += $" {count} moneda de {coin},  ";
                    change -= coin * count;
                }
            }

            if (change > 0)
            {
                return "No hay suficiente cambio en la máquina.";
            }

            return result;
        }
    }

}
