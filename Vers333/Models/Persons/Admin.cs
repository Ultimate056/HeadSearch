namespace webapi.Models.Persons
{
    public class Admin
    {
        public int Id { get; set; }

        public string? Email { get; set; }   

        public string? FirstName { get; set; }   

        public string? SecondName { get; set; }

        public string? ThirdName { get; set; }

        public string? Password { get; set; }

        public int PostId { get; set; } 
    }
}
