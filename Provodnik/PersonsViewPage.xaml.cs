﻿using FluentFTP;
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
                    p.Parent = vm;
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
            {
                List<PersonShortViewModel> list =null;

                if (vm.IsPersonsSelected == false)
                {
                    if (PersonsListView.SelectedItem != null)
                    {
                        list = new List<PersonShortViewModel> { PersonsListView.SelectedItem as PersonShortViewModel };
                    }
                }
                else
                {
                    list = vm.PersonList.Where(x => x.IsSelected).ToList();
                }

                if (list != null) { 
                    var listToAsk = list.Select(x => x.Fio).Take(5).ToList();
                    if (list.Count > 5) listToAsk.Add($"... (всего {list.Count})");
                    var message = "Удалить следующих бойцов:"
                        + Environment.NewLine+ Environment.NewLine
                        + string.Join(Environment.NewLine, listToAsk)
                        + "?";

                    if (MessageBox.Show(message, "Подтверждение удаления", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                    {
                        foreach (var g in list)
                        {
                            vm.PersonList.Remove(g);
                            new Repository().DeletePerson(g.Id);
                        }
                    }
                }
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
                excel.cell[ri, 2].value2 = "Новосибирское РО";
                var fio = new StringHelper().ParseFio(r.Fio);
                excel.cell[ri, 3].value2 = fio[0];
                excel.cell[ri, 4].value2 = fio[1];
                excel.cell[ri, 5].value2 = fio[2];
                excel.cell[ri, 6].value2 = r.BirthDat?.ToString("dd.MM.yyyy"); 
                excel.cell[ri, 7].value2 =r.UchZavedenie;
                excel.cell[ri, 8].value2 = r.UchFac; 
                excel.cell[ri, 9].value2 = r.Inn;
                excel.cell[ri, 10].value2 = Helper.FormatSnils(r.Snils);
                excel.cell[ri, 11].value2 = "АО \"ФПК\"";
                excel.cell[ri, 12].value2 = new DateTime(DateTime.Today.Year,4,1).ToString("dd.MM.yyyy");
                excel.cell[ri, 13].value2 = new DateTime(DateTime.Today.Year, 12, 31).ToString("dd.MM.yyyy");
            }
            excel.setAllBorders(excel.get_Range("A1", "M" + ri));
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
                         p.UchebStartDat,
                         seliObuch = p.UchebGruppa != null,//UchebStartDat.HasValue,
                         vibilObuch = p.UchebGruppa != null && p.IsVibil, //p.VibilPrichina == "выбыл с обучения",
                         ostObuch = p.UchebGruppa != null && !p.IsVibil,//p.VibilPrichina != "выбыл с обучения",
                         p.IsExamen,
                         //p.IsExamenFailed,
                         //oshibokSvidet = p.IsSertificatError,
                         poluchenoSvidet = p.SertificatDat.HasValue,//db.PersonDocs.Any(x=>x.PersonId==p.Id && x.DocTypeId==DocConsts.СвидетельствоПрофессии && x.FileName!=null),
                         zakazanoNapr = p.IsNaprMedZakazano,
                         gotovoNapr = p.IsNaprMedPolucheno,
                         vishel = p.IsNaprMedVidano,
                         goden = p.IsMedKomm ,
                         neGoden =p.IsMedKommNeGoden, //p.VibilPrichina == "не допущен медкомиссией", //"не годен",
                         trudoustroen=p.IsTrudoustroen,
                         p.IsNovichok ,
                         medKnizkaPoluchena= p.IsSanKnizka || p.IsSvoyaSanKnizka,
                         medKnizkaZakazana= p.SanKnizkaDat.HasValue || p.IsSvoyaSanKnizka

                     };

            int startCol = 2;

            foreach (var r in qq
                .Where(p=>p.UchebStartDat != null && p.UchebStartDat.Value.Year >= DateTime.Today.Year)
                .GroupBy(x => x.UchebGruppa).OrderBy(x => x.Key).Where(x => x.Key != null))
            {
                ri++;
                excel.cell[ri, startCol + 2].value2 = r.Key;
                excel.cell[ri, startCol + 3].value2 = r.Count();
                excel.cell[ri, startCol + 4].value2 = r.Count(x => x.vibilObuch);
                excel.cell[ri, startCol + 5].value2 = r.Count(x => x.ostObuch);
                excel.cell[ri, startCol + 6].value2 = r.Count(x => x.IsExamen);
                excel.cell[ri, startCol + 7].value2 = r.Count(x => x.poluchenoSvidet);
            }

            foreach (var r in qq.GroupBy(x => x.IsNovichok).OrderByDescending(x => x.Key))
            {
                ri++;
                excel.cell[ri, startCol + 2].value2 = r.Key ? "Всего новички" : "Старики";
                excel.cell[ri, startCol + 3].value2 = r.Key ? r.Count(x => x.seliObuch):r.Count();
                excel.cell[ri, startCol + 4].value2 = r.Count(x => x.vibilObuch);
                excel.cell[ri, startCol + 5].value2 = r.Key ? r.Count(x => x.ostObuch) : r.Count();
                excel.cell[ri, startCol + 6].value2 = r.Count(x => x.IsExamen);
                excel.cell[ri, startCol + 7].value2 = r.Count(x => x.poluchenoSvidet);
            }

            excel.cell[2, startCol + 8].value2 = qq.Count(x => x.medKnizkaZakazana);
            excel.cell[2, startCol + 9].value2 = qq.Count(x => x.medKnizkaPoluchena);

            excel.cell[2, startCol + 10].value2 = qq.Count(x => x.zakazanoNapr);
            excel.cell[2, startCol + 11].value2 = qq.Count(x => x.gotovoNapr);
            excel.cell[2, startCol + 12].value2 = qq.Count(x => x.vishel);
            excel.cell[2, startCol + 13].value2 = qq.Count(x => x.goden);
            excel.cell[2, startCol + 14].value2 = qq.Count(x => x.neGoden);
            excel.cell[2, startCol + 15].value2 = qq.Count(x => x.trudoustroen);

            ri++;
            excel.cell[ri, startCol + 2].value2 = "ИТОГО:";
            for (int i = 3; i <= 7; i++)
                excel.cell[ri, startCol + i].Formula= "=R[-1]C+R[-2]C";
            for (int i = 8; i <= 15; i++)
            {
                excel.mySheet.Range[excel.cell[2, startCol + i], excel.cell[ri - 1, startCol + i]].Merge();
                excel.cell[ri, startCol + i].Formula = "=R2C";
            }
            excel.cell[2, startCol + 1].value2 = ids.Count;//.Formula = $"=R{ri}C{startCol +3}";

            excel.setAllBorders(excel.get_Range("A1", "Q" + ri));
            excel.myExcel.Visible = true;
        }

        private void ObuchenieButton_Click(object sender, RoutedEventArgs e)
        {
            excelReport excel = new excelReport();
            excel.Init("Таблица обучение.xltx", $"Обучение { DateTime.Today.Year}.xlsx");//,visible:true);//, otchetDir: otchetDir);

            int ri = 3;
            var db = new ProvodnikContext();

            var ids = vm.PersonList.Select(pp => pp.Id).ToList();
            var qq = db.Persons.Where(p => ids.Contains(p.Id) && p.UchebEndDat.HasValue && p.UchebEndDat.Value.Year == DateTime.Today.Year)
                .GroupBy(p => new { p.UchebGruppa, p.UchebStartDat, p.UchebEndDat })
                .Select(x => new { x.Key.UchebGruppa, x.Key.UchebStartDat, x.Key.UchebEndDat, kolvo = x.Count() }).OrderBy(x=>x.UchebEndDat);
                                 
            foreach (var r in qq)
            {
                ri++;
                excel.cell[ri, 1].value2 = ri - 3;
                excel.cell[ri, 2].value2 = r.UchebGruppa;
                excel.cell[ri, 3].value2 = r.UchebStartDat?.ToString("dd.MM.yyyy");
                excel.cell[ri, 4].value2 = r.UchebEndDat?.ToString("dd.MM.yyyy");
                excel.cell[ri, 5].value2 = r.kolvo;
            }

            ri++;
            excel.cell[ri,1].value2 = "Всего:"; excel.mySheet.Range[excel.cell[ri, 1], excel.cell[ri , 4]].Merge();
            excel.cell[ri, 5].Formula =  "=SUM(R4C:R[-1]C)";
            
            excel.setAllBorders(excel.get_Range("A4", "E" + ri));
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
