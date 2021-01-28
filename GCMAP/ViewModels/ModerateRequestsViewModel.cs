using GCMAP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GCMAP.ViewModels
{
    public class ModerateRequestsViewModel
    {
        public IEnumerable<Request> Requests { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
