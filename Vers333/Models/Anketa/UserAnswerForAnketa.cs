namespace webapi.Models.Anketa
{
    public class UserAnswerForAnketa
    {
        public int Id { get; set; }

        public string? Content { get; set; }

        public int QuestionAnketaId { get; set; }   

        public int UserId { get;set; }

    }
}
