using ExamTwo.Data;
using ExamTwo.Models;

namespace ExamTwo.Services
{
    public class OrderValidator
    {
        private readonly Database _db;

        public OrderValidator(Database db)
        {
            _db = db;
        }

        public PurchaseResult Validate(OrderRequest req)
        {
            if (req == null || req.Items == null || req.Items.Count == 0)
                return Fail("La orden está vacía.");

            foreach (var item in req.Items)
            {
                if (!_db.keyValues.ContainsKey(item.Type))
                    return Fail($"Café inválido: {item.Type}");

                if (item.Quantity <= 0)
                    return Fail("Cantidad inválida.");

                if (_db.keyValues[item.Type] < item.Quantity)
                    return Fail($"No hay suficiente stock de {item.Type}.");
            }

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
