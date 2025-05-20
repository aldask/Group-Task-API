namespace GroupsTask_API.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int PayerId { get; set; }
        public Member Payer { get; set; }

        public decimal Amount { get; set; }

        public List<TransactionSplit> Splits { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
