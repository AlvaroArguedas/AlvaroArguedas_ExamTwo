using NUnit.Framework;
using ExamTwo.Services;
using ExamTwo.Data;
using ExamTwo.Models;

namespace ExamTwo.Tests
{
    public class CoffeeMachineServiceTests
    {
        private CoffeeMachineService _service;
        private Database _db;

        [SetUp]
        public void Setup()
        {
            _db = new Database();
            _service = new CoffeeMachineService(_db);
        }

        [Test]
        public void BuyCoffee_ShouldFail_WhenRequestIsNull()
        {
            var result = _service.BuyCoffee(null);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("La orden está vacía.", result.Message);
        }

        [Test]
        public void BuyCoffee_ShouldFail_WhenQuantityExceedsStock()
        {
            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 999 }
        },
                Payment = new Payment
                {
                    Coins = new List<int> { 500, 500 },
                    Bills = new List<int> { }
                }
            };

            var result = _service.BuyCoffee(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("No hay suficiente stock de Americano.", result.Message);
        }

        [Test]
        public void BuyCoffee_ShouldFail_WhenPaymentIsInsufficient()
        {
            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 1 }
        },
                Payment = new Payment
                {
                    Coins = new List<int> { 100 },
                    Bills = new List<int> { }
                }
            };

            var result = _service.BuyCoffee(request);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Dinero insuficiente.", result.Message);
        }

        [Test]
        public void BuyCoffee_ShouldSucceed_WhenExactPayment()
        {
            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 1 }
        },
                Payment = new Payment
                {
                    Coins = new List<int> { 500, 100, 100, 100, 100, 50 },
                    Bills = new List<int> { }
                }
            };

            var result = _service.BuyCoffee(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, result.TotalChange);
        }

        [Test]
        public void BuyCoffee_ShouldReturnCorrectChange()
        {
            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 1 }
        },
                Payment = new Payment
                {
                    Bills = new List<int> { 1000 },
                    Coins = new List<int> { 500 }
                }
            };

            var result = _service.BuyCoffee(request);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(550, result.TotalChange);
            Assert.AreEqual(1, result.ChangeBreakdown[500]);
            Assert.AreEqual(0, result.ChangeBreakdown.GetValueOrDefault(100));
        }

        [Test]
        public void GetCoffees_ShouldReturnAllCoffeesWithCorrectStock()
        {
            var coffees = _service.GetCoffees();
            Assert.AreEqual(10, coffees["Americano"]);
            Assert.AreEqual(8, coffees["Cappuccino"]);
            Assert.AreEqual(10, coffees["Lates"]);
            Assert.AreEqual(15, coffees["Mocaccino"]);
        }

        [Test]
        public void GetCoffeePrices_ShouldReturnCorrectPrices()
        {
            var prices = _service.GetCoffeePrices();

            Assert.AreEqual(950, prices["Americano"]);
            Assert.AreEqual(1200, prices["Cappuccino"]);
            Assert.AreEqual(1350, prices["Lates"]);
            Assert.AreEqual(1500, prices["Mocaccino"]);
        }

        [Test]
        public void BuyCoffee_ShouldFail_WhenQuantityIsZero()
        {
            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 0 }
        },
                Payment = new Payment
                {
                    Bills = new List<int> { 1000 },
                    Coins = new List<int> { 500 }
                }
            };

            var result = _service.BuyCoffee(request);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Cantidad inválida.", result.Message);
        }


        [Test]
        public void BuyCoffee_ShouldFail_WhenCoinNotAllowed()
        {
            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 1 }
        },
                Payment = new Payment
                {
                    Coins = new List<int> { 10 },
                    Bills = new List<int>{ }
                }
            };
            var result = _service.BuyCoffee(request);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Moneda no permitida.", result.Message);
        }

        [Test]
        public void BuyCoffee_ShouldFail_WhenBillNotAllowed()
        {
            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 1 }
        },
                Payment = new Payment
                {
                    Coins = new List<int>{ },
                    Bills = new List<int> { 2000 }
                }
            };
            var result = _service.BuyCoffee(request);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Solo se aceptan billetes de 1000 colones.", result.Message);
        }

        [Test]
        public void Payment_ShouldCalculateTotalAmountCorrectly()
        {
            var payment = new Payment
            {
                Coins = new List<int> { 500, 100 },
                Bills = new List<int> { 1000 }
            };

            Assert.AreEqual(1600, payment.TotalAmount);
        }

        [Test]
        public void BuyCoffee_ShouldFail_WhenNoCoinsInMachine()
        {
            _db.keyValues3.Clear();

            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 1 }
        },
                Payment = new Payment
                {
                    Coins = new List<int> { 500, 500 },
                    Bills = new List<int>{ }
                }
            };

            var result = _service.BuyCoffee(request);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Fuera de servicio", result.Message);
        }

        [Test]
        public void BuyCoffee_ShouldFail_WhenCannotGiveExactChange()
        {
            _db.keyValues3 = new Dictionary<int, int> { { 100, 1 } };

            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 1 }
        },
                Payment = new Payment
                {
                    Coins = new List<int> { }, 
                    Bills = new List<int> { 1000 }
                }
            };
            var result = _service.BuyCoffee(request);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Fallo al realizar la compra.", result.Message);
        }

        [Test]
        public void BuyCoffee_ShouldUpdateCoffeeStock()
        {
            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 2 }
        },
                Payment = new Payment
                {
                    Coins = new List<int> { 500, 500, 500, 500 },
                    Bills = new List<int>{ }
                }
            };

            _service.BuyCoffee(request);
            var coffees = _service.GetCoffees();
            Assert.AreEqual(8, coffees["Americano"]);
        }

        [Test]
        public void BuyCoffee_ShouldFail_WhenCoffeeOutOfStock()
        {
            var coffees = _service.GetCoffees();
            coffees["Americano"] = 0;

            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 1 }
        },
                Payment = new Payment
                {
                    Coins = new List<int> { 500, 500 },
                    Bills = new List<int>{ }
                }
            };
            var result = _service.BuyCoffee(request);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("No hay suficiente stock de Americano.", result.Message);
        }

        [Test]
        public void BuyCoffee_ShouldSucceed_WhenBuyingMultipleCoffees()
        {
            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 2 },
            new OrderItem { Type = "Cappuccino", Quantity = 3 }
        },
                Payment = new Payment
                {
                    Coins = new List<int> { 500 },
                    Bills = new List<int> { 1000, 1000, 1000, 1000, 1000 } 
                }
            };

            var result = _service.BuyCoffee(request);

          

            int expectedCost = 2 * 950 + 3 * 1200;
            int expectedChange = request.Payment.TotalAmount - expectedCost;
            Assert.AreEqual(expectedChange, result.TotalChange);
        }
        [Test]
        public void BuyCoffee_ShouldSucceed_WhenBuyingExactStock()
        {
            var coffees = _service.GetCoffees();
            int currentStock = coffees["Americano"];

            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = currentStock }
        },
                Payment = new Payment
                {
                    Coins = new List<int> { 500, 500, 500, 500, 500 },
                    Bills = new List<int> { 1000, 1000, 1000, 1000, 1000, 1000, 1000 },
                }
            };

            var result = _service.BuyCoffee(request);
            Assert.IsTrue(result.Success);

            coffees = _service.GetCoffees();
            Assert.AreEqual(0, coffees["Americano"]);
        }

        [Test]
        public void BuyCoffee_ShouldReturnCorrectChangeBreakdownWithMultipleCoins()
        {
            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 1 }
        },
                Payment = new Payment
                {
                    Coins = new List<int> { 500, 500, 100, 50 },
                    Bills = new List<int> { 1000 }
                }
            };

            var result = _service.BuyCoffee(request);
            Assert.IsTrue(result.Success);

            int totalPaid = request.Payment.TotalAmount;
            int expectedChange = totalPaid - 950;

            Assert.AreEqual(expectedChange, result.TotalChange);

            
            int breakdownSum = result.ChangeBreakdown.Sum(kv => kv.Key * kv.Value);
            Assert.AreEqual(expectedChange, breakdownSum);
        }



        [Test]
        public void BuyCoffee_ShouldFail_WhenSecondPaymentIsInsufficient()
        {
            var request = new OrderRequest
            {
                Items = new List<OrderItem>
        {
            new OrderItem { Type = "Americano", Quantity = 1 }
        },
                Payment = new Payment
                {
                    Bills = new List<int> { 1000 },
                    Coins = new List<int> { } 
                }
            };

            var result = _service.BuyCoffee(request);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(50, result.TotalChange);

            request.Payment = new Payment { Coins = new List<int> { 500 }, Bills = new List<int>() };
            result = _service.BuyCoffee(request);
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Dinero insuficiente.", result.Message);
        }

    }
}
