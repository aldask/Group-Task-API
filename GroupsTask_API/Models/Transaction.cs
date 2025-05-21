public class Transaction
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public int PayerId { get; set; }
    public decimal Amount { get; set; }
    public ICollection<TransactionSplit> Splits { get; set; } = new List<TransactionSplit>();
    public DateTime CreatedAt { get; set; }
}