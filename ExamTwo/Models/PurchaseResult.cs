namespace ExamTwo.Models
{
    public class PurchaseResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalChange { get; set; }
        public Dictionary<int, int> ChangeBreakdown { get; set; } = new();
    }
}


