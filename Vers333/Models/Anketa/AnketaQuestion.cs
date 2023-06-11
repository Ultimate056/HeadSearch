namespace webapi.Models.Anketa
{
    public class AnketaQuestion
    {
        public int Id { get; set; }

        public string? Content { get; set; }

        public int AnketaId { get; set; }
    }
}
