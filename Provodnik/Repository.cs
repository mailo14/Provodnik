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

        public List<string> GetObucheniyas()
        {
            var yearStart = DateTime.Today;
            yearStart = yearStart.AddDays(-yearStart.Day + 1).AddMonths(-yearStart.Month + 1);
            var qq = (from p in new ProvodnikContext().Persons
                      where p.UchebStartDat > yearStart && 
                      p.UchebGruppa != null
                      orderby p.UchebGruppa descending
                      select p.UchebGruppa).Distinct().ToList();
            qq.Insert(0, "(нет)");

            return qq;
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

        public List<string> GetOtryadi()
        {
            return new ProvodnikContext().Persons.Select(pp => pp.Otryad).Distinct().ToList();
        }

        public int GetAlarmsCount()
        {
            using (var db = new ProvodnikContext())
            {
                return (from p in db.Persons
                         join pd in db.PersonDocs on p.Id equals pd.PersonId                         
                         where pd.PrinesetK <= DateTime.Today
                         select 1                         ).Count();
            }
        }
        public List<AlarmViewModel> GetAlarms()
        {
            using (var db = new ProvodnikContext())
            {
                return (from p in db.Persons
                        join pd in db.PersonDocs on p.Id equals pd.PersonId
                        join pdt in db.DocTypes on pd.DocTypeId equals pdt.Id
                        where pd.PrinesetK < DateTime.Today
                        orderby pd.PrinesetK, p.Fio
                        select new {p.Id, p.Fio, p.Phone, p.BirthDat, pdt.Description, pd.PrinesetK }).ToList()
                        .Select(p=> new AlarmViewModel(p.Id, p.Fio, p.Phone, p.BirthDat, p.Description, p.PrinesetK)).ToList();
            }
        }

    } public class AlarmViewModel
        {
            public AlarmViewModel(int id, string fio, string phone, DateTime? birthDat, string description, DateTime? prinesetK)
            {
                Id = id;
                Fio = fio;
                Phone = phone;
                BirthDat = birthDat;
                DocType = description;
                PrinesetK = prinesetK;
            }

            public int Id { get; set; }
            public string Fio { get; set; }
            public string Phone { get; set; }
            public string  DocType { get; set; }
           public DateTime? BirthDat { get; set; }
           public DateTime? PrinesetK { get; set; }
        }
}
