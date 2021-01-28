using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.Models
{
    //Модель новости
    public class News
    {
        public int Id { get; set; }
        public string Theme { get; set; }
        public Photo Photo { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
    }
}
