using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provodnik
{
public    class Repository
    {
        public List<string> GetPersonCities()
        {
            return new ProvodnikContext().Persons.Select(pp => pp.Gorod).Distinct()
                .Union(new List<string> { "Адлер", "Москва", "Санкт-Петербург", "Новороссийск","Новосибирск" }).Distinct().OrderBy(pp => pp).ToList();
        }
        public List<string> GetCities()
        {
            return new ProvodnikContext().Persons.Select(pp => pp.Gorod).Where(pp=>!pp.Contains(",") && !pp.Contains("Москва-")).Distinct()
                .Union(new List<string> { "Адлер", "Москва", "Санкт-Петербург", "Новороссийск","Новосибирск" }).Distinct().OrderBy(pp => pp).ToList();
        }
        public List<string> GetPeresadSts()
        {
            return new ProvodnikContext().SendGroups.Select(pp => pp.PeresadSt).Distinct()
                .Union(new List<string> { "Екатеринбург", "Москва"  }).Distinct().OrderBy(pp => pp).ToList();
        }
        public List<string> GetDepos(string city=null)
        {
            var mosDepos=new List<string> { "М – Николаевка","М – Киевская","М – Ярославская" };
            var spbDepos= new List<string> { "Санкт-Петербург ВЧ8" };
            switch (city)
            {
                case null: return mosDepos.Union(spbDepos).Union(new[] { "Адлер", "Новороссийск", "Новосибирск-Главный" }).OrderBy(x=>x).ToList();
                //case "Адлер": return new List<string> { };break;
                case "Москва": return mosDepos;
                case "Санкт-Петербург": return spbDepos;
                case "Новосибирск": return new List<string> { "Новосибирск-Главный" }; 
            }
            return new List<string>();
        }
        public List<string> GetDeposMed()
        {
            return new ProvodnikContext().MedKomZayavki.Select(x=>x.Depo).ToList()
                .Union(GetDepos())
                .Distinct().OrderBy(pp => pp).ToList();
        }

        public class DepoLabels
        {
            public string DepoRod { get; set; }
            public string Filial { get;  set; }
            public string Sp { get;  set; }
        }
        public DepoLabels GetDepoLabels(string city, string depo)
        {
            var ret = new DepoLabels();

            if (city == "Адлер") { ret.DepoRod = "Вагонного участка Адлер Северо-Кавказского филиала АО «ФПК»"; ret.Filial = "Северо-Кавказский филиал АО «ФПК"; ret.Sp = "вагонный участок Адлер"; }
            if (city == "Санкт-Петербург") { ret.DepoRod = "Вагонного участка Санкт-Петербург-Московский Северо-Западного филиала АО «ФПК»"; ret.Filial = "Северо-Западный филиал АО «ФПК"; ret.Sp = "вагонный участок Санкт-Петербург-Московский"; }
            if (city == "Новороссийск") { ret.DepoRod = "Пассажирского вагонного депо Новороссийск Северо-Кавказского филиала АО «ФПК»"; ret.Filial = "Северо-Кавказский филиал АО «ФПК"; ret.Sp = "вагонный участок Новороссийск"; }
            if (city == "Новосибирск") { ret.DepoRod = "Вагонного участка Новосибирск-Главный Западно-Сибирского филиала АО «ФПК»"; ret.Filial = "Западно-Сибирский филиал АО «ФПК"; ret.Sp = "вагонный участок Новосибирск-Главный"; }

            if (depo == "М – Николаевка") { ret.DepoRod = "Пассажирского вагонного депо Николаевка Московского филиала АО «ФПК»"; ret.Filial = "Московский филиал АО «ФПК"; ret.Sp = "депо Николаевка"; }
            if (depo == "М – Киевская") { ret.DepoRod = "Пассажирского вагонного депо Москва-Киевская Московского филиала АО «ФПК»"; ret.Filial = "Московский филиал АО «ФПК"; ret.Sp = "депо Москва-Киевская"; }
            if (depo == "М – Ярославская") { ret.DepoRod = "Пассажирского вагонного депо Москва-Ярославская Московского филиала АО «ФПК»"; ret.Filial = "Московский филиал АО «ФПК"; ret.Sp = "депо Москва-Ярославская"; }

            return ret;
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
            return new List<string> { UchFormaConsts.Ochnaya, "ЗАОЧНАЯ", "АКАДЕМ", "ОЧНО-ЗАОЧНАЯ", UchFormaConsts.Zakonchil, UchFormaConsts.NeUchitsa};
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


    public class UchFormaConsts
    {
        public const string Ochnaya = "ОЧНАЯ";
        //, "ЗАОЧНАЯ", "АКАДЕМ", "ОЧНО-ЗАОЧНАЯ", 
        public const string Zakonchil = "закончил";
        public const string NeUchitsa = "не учится";
    }
}
