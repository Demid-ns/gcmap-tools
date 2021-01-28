using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.ViewModels
{
    public class AcceptConnectionViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Никнейм")]
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        public string NickName { get; set; }
        [Display(Name = "Название станции")]
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        public string StationName { get; set; }
        [Display(Name = "Координата X")]
        public int X { get; set; }
        [Display(Name = "Координата Z")]
        public int Z { get; set; }
        [Display(Name = "Контакт")]
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        public string Contact { get; set; }
    }
}
