using GCMAP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.ViewModels
{
    public class AcceptRequestViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Никнейм")]
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        public string NickName { get; set; }
        [Display(Name = "Описание заявки")]
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        public string Description { get; set; }
        [Display(Name = "Контакт")]
        [Required(ErrorMessage = "Поле \"{0}\" обязательно для заполнения.")]
        public string Contact { get; set; }
        public Photo Photo { get; set; }
    }
}
