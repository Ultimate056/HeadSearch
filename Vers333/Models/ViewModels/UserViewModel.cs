using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Vers333.Models.ViewModels
{
    public class UserViewModel
    {
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        [Required(ErrorMessage = "Не указана почта")]
        [Remote(action: "CheckEmail", controller: "Main", ErrorMessage = "Email уже используется")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Не указано имя")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        public string? SecondName { get; set; }

        public string? ThirdName { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [RegularExpression("[a-zA-Z0-9]+", ErrorMessage ="Кириллица запрещена!")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string? PasswordAgain { get; set; }

        [Required(ErrorMessage = "Не указан телефон")]
        [RegularExpression(@"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$", ErrorMessage = "Телефон в неправильном формате!")]
        public string? PhoneNumber { get; set; }


        [Required(ErrorMessage = "Не указано место проживания")]
        public string? PlaceOfLife { get; set; }
    }
}
