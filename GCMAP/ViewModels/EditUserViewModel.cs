using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.ViewModels
{
    public class EditUserViewModel
    {
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [Display(Name = "Никнейм")]
        public string NickName { get; set; }

        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
