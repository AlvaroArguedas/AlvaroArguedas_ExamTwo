using ExamTwo.Data;
using ExamTwo.Models;
using ExamTwo.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExamTwo.Controllers
{
   
    public class CoffeeMachineController : Controller
    {
        private readonly ICoffeeMachineService _service;

        public CoffeeMachineController(ICoffeeMachineService service)
        {
            _service = service;
        }

        [HttpGet("getCoffees")]
        public IActionResult GetCoffees() => Ok(_service.GetCoffees());

        [HttpGet("getCoffeePricesInCents")]
        public IActionResult GetPrices() => Ok(_service.GetCoffeePrices());

        [HttpGet("getQuantity")]
        public IActionResult GetCoinInventory() => Ok(_service.GetCoinInventory());

        [HttpPost("buyCoffee")]
        public IActionResult BuyCoffee([FromBody] OrderRequest request)
        {
            var result = _service.BuyCoffee(request);
            return Ok(result);
        }
    }

}
