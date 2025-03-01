using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provodnik
{
    public class TrudoustroistvoShortViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Depo { get; set; }
        public int Kolvo { get; set; }
    }
}
