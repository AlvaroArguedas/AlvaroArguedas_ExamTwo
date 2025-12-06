namespace ExamTwo.Models
{
    public class Payment
    {
        public List<int> Coins { get; set; } = new List<int>();
        public List<int> Bills { get; set; } = new List<int>();
        public int TotalAmount
        {
            get
            {
                int total = 0;

                total += Coins.Sum();
                total += Bills.Sum();

                return total;
            }
        }
    }
}
