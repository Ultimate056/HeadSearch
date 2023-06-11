namespace webapi.Models.Tests
{
    public class Result
    {
        public int Id { get; set; } 

        public int TestId { get; set; }

        public string? ResultContent { get; set; }

        public string? ClosedTestTime { get; set; }
        public string? OpenTestTime { get; set; }

        public string? Status { get; set; }

        public int UserId { get; set; }
    }
}
