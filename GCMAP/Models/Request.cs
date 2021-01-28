using GCMAP.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.Models
{
    //Модель заявки
    public class Request
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string Description { get; set; }
        public string Contact { get; set; }
        //Тип заявки
        public RequestType RequestType { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public bool Accepted { get; set; }
        public Photo Photo { get; set; }
    }
}
