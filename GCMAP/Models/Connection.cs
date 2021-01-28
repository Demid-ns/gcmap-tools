using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.Models
{
    //Модель заявки на подключение
    public class Connection
    {
        public int Id { get; set; }
        public string StationName { get; set; }
        public int X { get; set; }
        public int Z { get; set; }
        public string NickName { get; set; }
        public string Contact { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public bool Accepted { get; set; }
    }
}
