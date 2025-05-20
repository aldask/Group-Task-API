namespace GroupsTask_API.Models
{
    public class TransactionSplit
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public int MemberId { get; set; }
        public Member Member { get; set; }

        public decimal Amount { get; set; }
    }
}
