using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.Models
{
    //Модель карты
    public class Map
    {
        public int Id { get; set; }
        public Photo Photo { get; set; }
        public string Uploader { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
    }
}
