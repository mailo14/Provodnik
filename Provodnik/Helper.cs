using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provodnik
{
    public static class Helper
    {
        public static string FormatPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return null;
            return "8" + phone;
        }
        public static string FormatSnils(string snils)
        {
            if (string.IsNullOrWhiteSpace(snils)) return null;
            return snils.Insert(9," ").Insert(6,"-").Insert(3,"-");
        }
    }
}
