using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static Provodnik.PersonViewModel;

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

            snils = snils.Trim();

            if (snils.Length != 11)
                return snils;

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

        public static List<DocType> GetDocuments(string Pol, string Grazdanstvo, string UchZavedenie)
        {var db = new ProvodnikContext();

            var Documents = db.DocTypes.Where(pp => pp.IsObyazat).OrderBy(pp=>pp.OrderId).ToList();


            if (Pol == "мужской")
            {
                if (!Documents.Where(pp => pp.Id >= 12 && pp.Id <= 14).Any())
                    foreach (var d in new ProvodnikContext().DocTypes.Where(pp => pp.Id >= 12 && pp.Id <= 14))
                        Documents.Add( d);
            }
            else
                    if (Pol == "женский")
            {
                foreach (var d in Documents.Where(pp => pp.Id >= 12 && pp.Id <= 14).ToList())
                    Documents.Remove(d);
            }


            var docIds = new int[] { 17, 18, 19, 20 };
            if (Grazdanstvo == "КЗ")
            {
                if (!Documents.Where(pp => docIds.Contains(pp.Id)).Any())
                    foreach (var d in new ProvodnikContext().DocTypes.Where(pp => docIds.Contains(pp.Id)))
                        Documents.Add(d);

                Documents.Remove(Documents.FirstOrDefault(x => x.Id == DocConsts.Прописка));
            }
            else
            {
                foreach (var d in Documents.Where(pp => docIds.Contains(pp.Id)).ToList())
                    Documents.Remove(d);

                if (!Documents.Any(x => x.Id == DocConsts.Прописка))
                {
                    var propiskaDoc = new ProvodnikContext().DocTypes.First(x => x.Id == DocConsts.Прописка);
                    Documents.Add(propiskaDoc);
                    //Documents.Insert(Documents.Any() ? 1 : 0, new PersonDocViewModel() { Description = propiskaDoc.Description, Id = propiskaDoc.Id, Bitmap = new System.Windows.Controls.Image() });
                }
            }


            //UchFormas.Clear(); foreach (var u in repository.GetUchFormas(UchZavedenie)) UchFormas.Add(u);
            //if (UchFormas.Count == 1) UchForma = UchFormas[0];
            //UchFacs.Clear(); foreach (var u in repository.GetUchFacs(UchZavedenie)) UchFacs.Add(u);

            if (!string.IsNullOrWhiteSpace(UchZavedenie) && UchZavedenie != RepoConsts.NoUchZavedenie)
            {
                if (!Documents.Where(pp => pp.Id == 15).Any())
                    foreach (var d in new ProvodnikContext().DocTypes.Where(pp => pp.Id == 15))
                        Documents.Add(d);
            }
            else
            {
                foreach (var d in Documents.Where(pp => pp.Id == 15).ToList())
                    Documents.Remove(d);
            }

            return Documents;
        }
        public static void RestorePersonDocuments(PersonViewModel p,bool ask=false,bool clear=false)
        {
            if (clear) p.Documents.Clear();

            var target=GetDocuments(p.Pol, p.Grazdanstvo, p.UchZavedenie);
            var ids=target.Select(x => x.Id).ToList();
            var toDelete=p.Documents.Where(x => target.All(_=>_.Id!=x.DocTypeId));
            var toAdd= target.Where(x => p.Documents.All(_ => _.DocTypeId!=x.Id)).ToList();

            string msg = "";
            if (toDelete.Any())
                msg += "Удалено: " + string.Join(", ", toDelete.Select(x => x.Description)) + Environment.NewLine;
            if (toAdd.Any())
                msg += "Добавлено: " + string.Join(", ", toAdd.Select(x => x.Description));

            if (msg==string.Empty)
            { return; }

            if (ask && MessageBox.Show(msg,"", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
            {
                return;
            }

            var db = new ProvodnikContext();

            var propiskaDoc = toAdd.FirstOrDefault(x => x.Id == DocConsts.Прописка);
            if (propiskaDoc != null && !p.Documents.Any(x => x.DocTypeId == DocConsts.Прописка))
            {                
                p.Documents.Insert(p.Documents.Any() ? 1 : 0, new PersonDocViewModel() { Description = propiskaDoc.Description, DocTypeId = propiskaDoc.Id, Bitmap = new System.Windows.Controls.Image() });
                toAdd.Remove(propiskaDoc);
            }

            foreach (var d in toAdd)
                p.Documents.Add(new PersonDocViewModel() { Description = d.Description, DocTypeId = d.Id, Bitmap = new System.Windows.Controls.Image() });

            foreach (var d in toDelete.ToList())
                p.Documents.Remove(d);

            var dtOrders = db.DocTypes.ToDictionary(x => x.Id, x => x.OrderId);

            p.Documents = new ObservableCollection<PersonDocViewModel>(p.Documents.OrderBy(x => dtOrders[x.DocTypeId]));
        }

        public static int? GetVozrast(DateTime? birthDat, DateTime? onDate=null)
        {
            if (!birthDat.HasValue) return null;

            if (!onDate.HasValue) onDate=DateTime.Today;
            var result = onDate.Value.Year- birthDat.Value.Year;
            if (birthDat.Value.AddYears(result) > onDate.Value) result--;

            return result;
        }

    }
}
