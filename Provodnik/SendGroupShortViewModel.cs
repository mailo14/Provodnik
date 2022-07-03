using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provodnik
{
    public class SendGroupShortViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? OtprDat { get; set; }
        public string City { get; set; }
        public string Depo { get; set; }
    }
}
