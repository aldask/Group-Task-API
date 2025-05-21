namespace GroupsTask_API.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public decimal Balance { get; set; }
    }
}