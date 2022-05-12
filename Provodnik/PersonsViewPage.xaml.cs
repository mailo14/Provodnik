using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Provodnik.PersonViewModel;

namespace Provodnik
{
    /// <summary>
    /// Логика взаимодействия для PersonsView.xaml
    /// </summary>
    public partial class PersonsViewPage : Page
    {
        public PersonsViewModel vm;
        int? groupId;
        public PersonsViewPage()
        {
            InitializeComponent();
            vm = new PersonsViewModel();
            DataContext = vm;
            //  this.groupId = groupId;
            //  if (!groupId.HasValue)
            AddToGroupButton.Visibility = Visibility.Collapsed;

        }
        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).IsDropDownOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var pw = new PersonView();
            pw.DataContext = new PersonViewModel(null);// new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);
            if (pw.ShowDialog() == true)
            {
                vm.PersonSearch = pw.vm.Fio; //vm.RefreshPersonList();

                //TODO goto if exist or add anyway and goto
                vm.InitCollectionsForCombo();
            }



        }


        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }
        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var p = ((FrameworkElement)e.OriginalSource).DataContext as PersonShortViewModel;
            if (p != null)
            {
                var pw = new PersonView();
                var db = new ProvodnikContext();
                var pvm = new PersonViewModel(p.Id);
                pw.DataContext = pvm;

                if (pw.ShowDialog() == true)
                {
                    var ind = vm.PersonList.IndexOf(p);
                    vm.PersonList.RemoveAt(ind);

                    p = MainWindow.Mapper.Value.Map<PersonShortViewModel>(new ProvodnikContext().Persons.First(pp => pp.Id == p.Id));
                    vm.PersonList.Insert(ind, p);
                    //vm.RefreshPersonList();
                    //TODO goto if exist or add anyway and goto
                    vm.InitCollectionsForCombo();

                    Helper.SetPersonShortIndexes(PersonsListView);
                }
            }
        }

        private void AddToGroupButton_Click(object sender, RoutedEventArgs e)
        {
            // if (vm.PersonList.Where(pp => pp.IsSelected).Any())
            //     DialogResult = true;
        }

        private void PersonsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                if (PersonsListView.SelectedItem != null &&
                MessageBox.Show("Удалить?", "Подтверждение удаления", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    var g = PersonsListView.SelectedItem as PersonShortViewModel;
                    vm.PersonList.Remove(g);
                    var db = new ProvodnikContext();
                    db.Persons.Remove(db.Persons.First(pp => pp.Id == g.Id));
                    db.SaveChanges();
                }
        }

        private async void ExcelButton_Click(object sender, RoutedEventArgs e)
        {
            new Reporter().ExportToExcel(vm.PersonList.Select(pp => pp.Id).ToList());
        }

        private void PersonsListView_Sorting(object sender, DataGridSortingEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                //runs after sorting is done
                Helper.SetPersonShortIndexes(PersonsListView);
            }, null);
        }

        private void BadgesButton_Click(object sender, RoutedEventArgs e)
        {
            new Reporter().Badges(vm.PersonList.ToList());
        }

        private void SpravkiPSOButton_Click(object sender, RoutedEventArgs e)
        {
            var db = new ProvodnikContext();

            var ids = vm.PersonList.Select(pp => pp.Id).ToList();
            var idsWithSpravka = new HashSet<int>(from p in db.Persons
                                                  where ids.Contains(p.Id)
                                                  join pd in db.PersonDocs on p.Id equals pd.PersonId
                                                  where pd.DocTypeId == DocConsts.СправкаРСО && pd.FileName != null
                                                  select p.Id);

            var persons = vm.PersonList.Where(p => !idsWithSpravka.Contains(p.Id));
            if (!persons.Any())
            {
                MessageBox.Show("Справки есть у всех");
                return;
            }

            excelReport excel = new excelReport();
            excel.Init("Справки РСО.xltx", $"Справки РСО { DateTime.Now.Ticks}.xlsx");//,visible:true);//, otchetDir: otchetDir);

            int ri = 1;
            foreach (var r in persons)
            {
                ri++;
                excel.cell[ri, 1].value2 = ri - 1;
                excel.cell[ri, 2].value2 = r.Fio;
                //excel.cell[ri, 3].value2 = "АО \"ФПК\"";
                excel.cell[ri, 4].value2 = r.BirthDat?.ToString("dd.MM.yyyy");
                excel.cell[ri, 5].value2 = r.UchZavedenie;
                excel.cell[ri, 6].value2 = r.UchFac;
                excel.cell[ri, 7].value2 = r.Inn;
                excel.cell[ri, 8].value2 = Helper.FormatSnils(r.Snils);
                //excel.cell[ri, 9].value2 = "01.04.2021 - 31.12.2021 г.";
            }
            excel.setAllBorders(excel.get_Range("A1", "I" + ri));
            excel.myExcel.Visible = true;
        }

        private void VoronkaButton_Click(object sender, RoutedEventArgs e)
        {
            excelReport excel = new excelReport();
            excel.Init("Воронка.xltx", $"Воронка { DateTime.Today.ToString("dd.MM.yyyy")}.xlsx");//,visible:true);//, otchetDir: otchetDir);

            int ri = 1;
            var db = new ProvodnikContext();

            var ids = vm.PersonList.Select(pp => pp.Id).ToList();
            var qq = from p in db.Persons
                     where ids.Contains(p.Id)
                     select new
                     {
                         p.UchebGruppa,
                         seliObuch = p.UchebGruppa != null,//UchebStartDat.HasValue,
                         vibilObuch = p.UchebGruppa != null && p.IsVibil, //p.VibilPrichina == "выбыл с обучения",
                         ostObuch = p.UchebGruppa != null && !p.IsVibil,//p.VibilPrichina != "выбыл с обучения",
                         p.IsExamen,
                         p.IsExamenFailed,
                         oshibokSvidet = p.IsSertificatError,
                         poluchenoSvidet = p.SertificatDat.HasValue,//db.PersonDocs.Any(x=>x.PersonId==p.Id && x.DocTypeId==DocConsts.СвидетельствоПрофессии && x.FileName!=null),
                         zakazanoNapr = p.IsNaprMedZakazano,
                         gotovoNapr = p.IsNaprMedPolucheno,
                         vishel = p.IsNaprMedVidano,
                         goden = p.IsMedKomm,
                         neGoden = p.VibilPrichina == "не допущен медкомиссией", //"не годен",

                         p.IsNovichok
                     };

            int startCol = 2;

            foreach (var r in qq.GroupBy(x => x.UchebGruppa).OrderBy(x => x.Key).Where(x => x.Key != null))
            {
                ri++;
                excel.cell[ri, startCol + 2].value2 = r.Key;
                excel.cell[ri, startCol + 3].value2 = r.Count();
                excel.cell[ri, startCol + 4].value2 = r.Count(x => x.vibilObuch);
                excel.cell[ri, startCol + 5].value2 = r.Count(x => x.ostObuch);
                excel.cell[ri, startCol + 6].value2 = r.Count(x => x.IsExamen);
                excel.cell[ri, startCol + 7].value2 = r.Count(x => x.IsExamenFailed);
                excel.cell[ri, startCol + 8].value2 = r.Count(x => x.oshibokSvidet);
                excel.cell[ri, startCol + 9].value2 = r.Count(x => x.poluchenoSvidet);
            }

            foreach (var r in qq.GroupBy(x => x.IsNovichok).OrderByDescending(x => x.Key))
            {
                ri++;
                excel.cell[ri, startCol + 2].value2 = r.Key ? "Всего новички" : "Старики";
                excel.cell[ri, startCol + 3].value2 = r.Count(x => x.seliObuch);
                excel.cell[ri, startCol + 4].value2 = r.Count(x => x.vibilObuch);
                excel.cell[ri, startCol + 5].value2 = r.Count(x => x.ostObuch);
                excel.cell[ri, startCol + 6].value2 = r.Count(x => x.IsExamen);
                excel.cell[ri, startCol + 7].value2 = r.Count(x => x.IsExamenFailed);
                excel.cell[ri, startCol + 8].value2 = r.Count(x => x.oshibokSvidet);
                excel.cell[ri, startCol + 9].value2 = r.Count(x => x.poluchenoSvidet);
            }

            excel.cell[2, startCol + 10].value2 = qq.Count(x => x.zakazanoNapr);
            excel.cell[2, startCol + 11].value2 = qq.Count(x => x.gotovoNapr);
            excel.cell[2, startCol + 12].value2 = qq.Count(x => x.vishel);
            excel.cell[2, startCol + 13].value2 = qq.Count(x => x.goden);
            excel.cell[2, startCol + 14].value2 = qq.Count(x => x.neGoden);

            ri++;
            excel.cell[ri, startCol + 2].value2 = "ИТОГО:";
            excel.cell[ri, startCol + 3].Formula = excel.cell[ri, startCol + 9].Formula = "=R[-1]C+R[-2]C";
            for (int i = 10; i <= 14; i++)
            {
                excel.mySheet.Range[excel.cell[2, startCol + i], excel.cell[ri - 1, startCol + i]].Merge();
                excel.cell[ri, startCol + i].Formula = "=R2C";
            }
            excel.cell[2, startCol + 1].value2 = ids.Count;//.Formula = $"=R{ri}C{startCol +3}";

            excel.setAllBorders(excel.get_Range("A1", "P" + ri));
            excel.myExcel.Visible = true;
        }

        private void ScansButton_Click(object sender, RoutedEventArgs e)
        {

            excelReport excel = new excelReport();
            excel.Init("Сканы.xltx", $"Сканы.xlsx");//,visible:true);//, otchetDir: otchetDir);

            int ri = 1;
            var db = new ProvodnikContext();

            var ids = vm.PersonList.Select(pp => pp.Id).ToList();
            var qq = from p in db.Persons
                     join pd in db.PersonDocs on p.Id equals pd.PersonId
                     //join d in db.DocTypes on 
                     where ids.Contains(p.Id)
                     group pd by p into g
                     select new
                     {
                         g.Key.Fio,
                         g.Key.Otryad,
                         scans = g.Where(pp=> pp.FileName != null).Select(pp => pp.DocTypeId)
                     };

            var dts = db.DocTypes.ToList();
            int c = 2;
            foreach (var dt in dts)
            {
                c++;
                excel.cell[1, c].value2 = dt.Description;
            }
            foreach (var r in qq.OrderBy(pp => pp.Fio))
            {
                ri++;
                excel.cell[ri, 1].value2 = r.Fio;
                excel.cell[ri, 2].value2 = r.Otryad;

                var set = new HashSet<int>(r.scans);
                c = 2;
                foreach (var dt in dts)
                {
                    c++;
                    excel.cell[ri, c].value2 = set.Contains(dt.Id) ? 1 : 0;
                }
            }

            excel.setAllBorders(excel.mySheet.Range[excel.cell[1, 1], excel.cell[ri, 2 + dts.Count]]);
            excel.myExcel.Visible = true;
        }
    }
}
