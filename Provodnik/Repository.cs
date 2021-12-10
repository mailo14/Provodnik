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
            return new List<string> { "Адлер", "Москва", "Санкт-Петербург", "Новороссийск" };
        }
        public List<string> GetPeresadSts()
        {
            return new List<string> { "Екатеринбург", "Москва"  };
        }
        public List<string> GetDepos(string city)
        {
            switch (city)
            {
                //case "Адлер": return new List<string> { };break;
                case "Москва": return new List<string> { "М – Николаевка","М – Киевская","М – Ярославская" }; break;
                case "Санкт-Петербург": return new List<string> { "Санкт-Петербург ВЧ8" }; break;
            }
            return new List<string>();
        }

        public string GetF6Depo(string city, string depo)
        {
            if (city == "Адлер") return "Вагонного участка Адлер Северо-Кавказского филиала АО «ФПК»";
            if (city == "Санкт-Петербург") return "Санкт-Петербург-Московский Северо-Западного филиала АО «ФПК»";
            if (city == "Новороссийск") return "Северо-Кавказского филиала АО «ФПК»";

            if (depo == "М – Николаевка") return "Вагонного участка Москва-Николаевка Московского филиала АО «ФПК»";
            if (depo == "М – Киевская") return "Вагонного участка Москва-Киевская Московского филиала АО «ФПК»";
            if (depo == "М – Ярославская") return "Вагонного участка Москва-Ярославская Московского филиала АО «ФПК»";

            return "";
        }

        public List<string> GetRegOtdeleniya()
        {
            return new List<string> { "Новосибирское РО" };
        }

        public List<string> GetUchZavedeniya()
        {
            var items = new ProvodnikContext().Persons.Select(pp => pp.UchZavedenie).Distinct().OrderBy(pp => pp).ToList();
            items.Remove(RepoConsts.NoUchZavedenie);
            items.Insert(0, RepoConsts.NoUchZavedenie);
            return items;// new List<string> { "не учится", "СГУПС" };
        }

        public List<string> GetUchFacs()
        {
            var items = new ProvodnikContext().Persons.Select(pp => pp.UchFac).Distinct().OrderBy(pp => pp).ToList();
            return items;
        }

        public List<string> GetUchFormas(string uchZavedenie)
        {
            if (uchZavedenie == RepoConsts.NoUchZavedenie) return new List<string> { RepoConsts.NoUchZavedenie };
            return new List<string> { "ОЧНАЯ", "ЗАОЧНАЯ", "АКАДЕМ", "ОЧНО-ЗАОЧНАЯ" };
        }

        public string GetCurSezon()
        {
            return ((DateTime.Today.Month < 10) ? DateTime.Today.Year : DateTime.Today.Year + 1).ToString();
        }

        public List<string> GetSezons()
        {
            var ret = new ProvodnikContext().Persons.Select(pp => pp.Sezon).Distinct().OrderByDescending(pp => pp).ToList();
            var cur = GetCurSezon();//.ToString("dd.MM.yyyy");

            if (!ret.Any() || ret[0] != cur)
                ret.Insert(0, cur);
            return ret;
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

        public List<string> GetMedKommDats()
        {
            var yearStart = DateTime.Today;
            yearStart = yearStart.AddDays(-yearStart.Day + 1).AddMonths(-yearStart.Month + 1);
            
            var qq = (from pp in new ProvodnikContext().Persons
                      where pp.MedKommDat >=yearStart select pp.MedKommDat.Value)
                      .Distinct().OrderByDescending(pp=>pp).ToList().Select(pp=>pp.ToString("dd.MM.yyyy")).ToList();
            qq.Insert(0, RepoConsts.NoMedKommDat);

            return qq;
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
            qq.Insert(0, RepoConsts.NoObuchenie);

            return qq;
        }

        public List<string> GetUchFacs(string uchZavedenie)
        {
            if (uchZavedenie == RepoConsts.NoUchZavedenie) return new List<string> { };
            return new List<string> { "УПП", "др"};
        }

        public List<string> GetMarshruts()
        {
            var r = new ProvodnikContext().SendGroups.Select(pp => pp.Marshrut)
                .Union(new string[] { "Новосибирск – Москва – Новосибирск", "Новосибирск – Санкт-Петербург – Новосибирск", "Новосибирск – Адлер – Новосибирск", "Новосибирск – Новороссийск – Новосибирск" })
                .Distinct().OrderBy(pp => pp).ToList();
            return r;
        }

        public List<string> GetOtryadi()
        {
            return new ProvodnikContext().Persons.Select(pp => pp.Otryad).Distinct().ToList();
        }

        public int GetAlarmsCount()
        {
            using (var db = new ProvodnikContext())
            { var dat = DateTime.Today.AddDays(-10);
                var medNaprQuery = from p in db.Persons
                                   where !p.IsMedKomm && p.NaprMedVidanoDat != null && p.NaprMedVidanoDat.Value < dat
                                   select 1;
                var docPrinesetKQuery = from p in db.Persons
                                        join pd in db.PersonDocs on p.Id equals pd.PersonId
                                        where pd.PrinesetK < DateTime.Today
                                        select 1;
                return (medNaprQuery.Union(docPrinesetKQuery)).Count();
            }
        }
        public List<AlarmViewModel> GetAlarms()
        {
            using (var db = new ProvodnikContext())
            {
                var dat = DateTime.Today.AddDays(-10);
                var medNaprQuery = from p in db.Persons
                                   where !p.IsMedKomm && p.NaprMedVidanoDat != null && p.NaprMedVidanoDat.Value < dat
                                   //orderby pd.PrinesetK, p.Fio
                                   select new { p.Id, p.Fio, p.Phone, p.BirthDat, Description="Медкомиссия не пройдена",  PrinesetK= p.NaprMedVidanoDat };
                var docPrinesetKQuery = from p in db.Persons
                                        join pd in db.PersonDocs on p.Id equals pd.PersonId
                                        join pdt in db.DocTypes on pd.DocTypeId equals pdt.Id
                                        where pd.PrinesetK < DateTime.Today
                                        //orderby pd.PrinesetK, p.Fio
                                        select new { p.Id, p.Fio, p.Phone, p.BirthDat, pdt.Description, pd.PrinesetK };
                return (medNaprQuery.Union(docPrinesetKQuery)) .OrderBy(x=>new { x.PrinesetK,x.Fio }).ToList()
                        .Select(p=> new AlarmViewModel(p.Id, p.Fio, p.Phone, p.BirthDat, p.Description, p.PrinesetK)).ToList();
            }
        }
    }
    public class AlarmViewModel
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

    public class RepoConsts
    {
        public const string NoMedKommDat = "(не вышел)";
        public const string NoObuchenie = "(нет)";
        public const string NoUchZavedenie = "не учится";
        public const string NoSezons = "(все)";
    }
    }
