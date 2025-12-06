namespace ExamTwo.Models
{
    public class OrderRequest
    {
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public Payment Payment { get; set; } = new Payment();
    }

}
