using System.Text.Json.Serialization;

namespace GroupsTask_API.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GroupId { get; set; }
        [JsonIgnore]
        public Group? Group { get; set; }
        public decimal Balance { get; set; }
    }
}
