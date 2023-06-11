using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace webapi.Models.Persons
{
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? SecondName { get; set; }

        public string? ThirdName { get; set; }

        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }

        public string? PlaceOfLife { get; set; }


    }
}
