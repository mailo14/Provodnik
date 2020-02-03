using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

        public static void SetPersonShortIndexes(DataGrid personsListView)
        {
            int ind = 0;
            foreach (PersonShortViewModel p in personsListView.Items)
            {
                p.Index = ++ind;
            }
        }

        public static void SetPersonShortIndexes(IEnumerable<PersonShortViewModel> personList)
        {int ind = 0;
            foreach (PersonShortViewModel p in personList)
            {
                p.Index = ++ind;
            }
        }
    }
}
