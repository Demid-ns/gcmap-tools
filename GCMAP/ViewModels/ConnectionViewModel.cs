using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.ViewModels
{
    public class ConnectionViewModel
    {
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [Display(Name = "Ваш никнейм")]
        [MinLength(3, ErrorMessage = "Минимальна длинна поля \"{0}\" должна составлять 3 символа")]
        [MaxLength(20, ErrorMessage = "Максимальная длинна поля \"{0}\" должна составлять 20 символов")]
        public string NickName { get; set; }
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [Display(Name = "Название Вашей станции")]
        [MinLength(3, ErrorMessage = "Минимальна длинна поля \"{0}\" должна составлять 3 символов")]
        [MaxLength(20, ErrorMessage = "Максимальная длинна поля \"{0}\" должна составлять 20 символов")]
        public string StationName { get; set; }
        [Display(Name = "Координата X Вашей станции")]
        [Range(-11516, -5243, ErrorMessage = "Значение координаты X может быть от -5243 до 11516")]
        public int X { get; set; }
        [Display(Name = "Координата Z Вашей станции")]
        [Range(3458, 7616, ErrorMessage = "Значение координаты Z может быть от 3458 до 7616")]
        public int Z { get; set; }
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        [Display(Name = "Контакт (ник или телефон telegram / почтовый адрес)")]
        [MinLength(3, ErrorMessage = "Минимальна длинна поля \"{0}\" должна составлять 3 символов")]
        [MaxLength(80, ErrorMessage = "Максимальная длинна поля \"{0}\" должна составлять 80 символов")]
        public string Contact { get; set; }
    }
}
