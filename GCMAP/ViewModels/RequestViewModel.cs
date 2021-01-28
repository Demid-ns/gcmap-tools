using GCMAP.Models;
using GCMAP.Validators;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.ViewModels
{
    public class RequestViewModel
    {
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [Display(Name = "Ваш никнейм")]
        [MinLength(3, ErrorMessage = "Минимальна длинна поля \"{0}\" должна составлять 3 символа")]
        [MaxLength(20, ErrorMessage = "Максимальная длинна поля \"{0}\" должна составлять 20 символов")]
        public string NickName { get; set; }
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [Display(Name = "Описание заявки")]
        [MinLength(5, ErrorMessage = "Минимальна длинна поля \"{0}\" должна составлять 5 символов")]
        [MaxLength(350, ErrorMessage = "Максимальная длинна поля \"{0}\" должна составлять 350 символов")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [Display(Name = "Контакт (ник или телефон telegram / почтовый адрес)")]
        [MinLength(3, ErrorMessage = "Минимальна длинна поля \"{0}\" должна составлять 3 символов")]
        [MaxLength(80, ErrorMessage = "Максимальная длинна поля \"{0}\" должна составлять 80 символов")]
        public string Contact { get; set; }
        [Display(Name = "Скриншот для заявки (если требуется)")]
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg" })]
        [MaxFileSize(2 * 1024 * 1024)]
        public IFormFile Photo { get; set; }
    }
}
