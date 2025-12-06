using ExamTwo.Models;

namespace ExamTwo.Services
{
    public interface ICoffeeMachineService
    {
        Dictionary<string, int> GetCoffees();
        Dictionary<string, int> GetCoffeePrices();
        Dictionary<int, int> GetCoinInventory();
        PurchaseResult BuyCoffee(OrderRequest request);
    }

}
