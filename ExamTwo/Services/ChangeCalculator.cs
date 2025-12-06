namespace ExamTwo.Services
{
    public class ChangeCalculator
    {
        public Dictionary<int, int>? Calculate(int amount, Dictionary<int, int> inventory)
        {
            if (amount == 0)
                return new Dictionary<int, int>();

            var result = new Dictionary<int, int>();
            int remaining = amount;

            foreach (var coin in inventory.Keys.OrderByDescending(c => c))
            {
                int need = remaining / coin;
                int use = Math.Min(need, inventory[coin]);

                if (use > 0)
                {
                    result[coin] = use;
                    remaining -= use * coin;
                }

                if (remaining == 0)
                    return result;
            }

            return null;
        }
    }

}
