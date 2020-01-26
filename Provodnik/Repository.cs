using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provodnik
{
public    class Repository
    {
        public List<string> GetCities()
        {
            return new List<string> { "Адлер", "Москва","Санкт-Петербург" };
        }
        public List<string> GetDepos(string city)
        {
            switch (city)
            {
                case "Адлер": return new List<string> { "Адлер ВЧ" };break;
                case "Москва": return new List<string> { "Москва ВЧ" }; break;
                case "Санкт-Петербург": return new List<string> { "Санкт-Петербург ВЧ" }; break;
            }
            return new List<string>();
        }

        public List<string> GetRegOtdeleniya()
        {
            return new List<string> { "Новосибирское РО" };
        }

        public List<string> GetUchZavedeniya()
        {
            var items = new ProvodnikContext().Persons.Select(pp => pp.UchZavedenie).Distinct().OrderBy(pp => pp).ToList();
            items.Remove("не учится");
            items.Insert(0, "не учится");
            return items;// new List<string> { "не учится", "СГУПС" };
        }

        public List<string> GetUchFormas(string uchZavedenie)
        {
            if (uchZavedenie == "не учится") return new List<string> { "не учится" };
            return new List<string> { "ОЧНАЯ", "ЗАОЧНАЯ", "АКАДЕМ", "ОЧНО-ЗАОЧНАЯ" };
        }

        public List<string> GetGrazdanstva()
        {
            return new List<string> { "РФ","КЗ" };
        }

        public List<string> GetUchebCentri()
        {
            return new ProvodnikContext().Persons.Select(pp => pp.UchebCentr).Distinct().OrderBy(pp => pp).ToList();
            //return new List<string> { "СГУПС" };
        }

        public List<string> GetUchFacs(string uchZavedenie)
        {
            if (uchZavedenie == "не учится") return new List<string> { };
            return new List<string> { "УПП", "др"};
        }

        public List<string> GetMarshruts()
        {
            return new ProvodnikContext().SendGroups.Select(pp => pp.Marshrut).Distinct().OrderBy(pp => pp).ToList();
        }

        internal List<string> GetOtryadi()
        {
            return new ProvodnikContext().Persons.Select(pp => pp.Otryad).Distinct().ToList();
        }
    }
}
