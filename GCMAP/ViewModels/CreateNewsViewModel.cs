using GCMAP.Validators;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.ViewModels
{
    public class CreateNewsViewModel
    {
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [MinLength(5, ErrorMessage = "Минимальна длинна поля \"{0}\" должна составлять 5 символов")]
        [MaxLength(50, ErrorMessage = "Максимальная длинна поля \"{0}\" должна составлять 50 символов")]
        [Display(Name = "Тема")]
        public string Theme { get; set; }
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [DataType(DataType.Upload)]
        [Display(Name = "Фотография")]
        [AllowedExtensions(new string[] { ".png", ".jpg", ".jpeg" })]
        [MaxFileSize(2 * 1024 * 1024)]
        public IFormFile Photo { get; set; }
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [MinLength(5, ErrorMessage = "Минимальна длинна поля \"{0}\" должна составлять 5 символов")]
        [MaxLength(350, ErrorMessage = "Максимальная длинна поля \"{0}\" должна составлять 350 символов")]
        [Display(Name = "Описание")]
        public string Description { get; set; }
    }
}
