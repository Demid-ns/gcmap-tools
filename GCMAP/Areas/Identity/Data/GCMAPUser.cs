using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GCMAP.Areas.Identity.Data
{
    public class GCMAPUser : IdentityUser
    {
        //Дополнительное свойство пользователя - никнейм
        public string NickName { get; set; }
    }
}
