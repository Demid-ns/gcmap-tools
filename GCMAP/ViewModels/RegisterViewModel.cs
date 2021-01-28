using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [Display(Name = "Никнейм")]
        public string NickName { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        public string PasswordConfirm { get; set; }
    }
}
