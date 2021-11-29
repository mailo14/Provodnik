using FluentFTP;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace Provodnik
{    
    /// <summary>
    /// Логика взаимодействия для PersonView.xaml
    /// </summary>
    public partial class MedKomZayavkaView : Window
    {
        public MedKomZayavkaView()
        {
            InitializeComponent();
         //   this.DataContext = new PersonViewModel(new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);//MedKomZayavkaViewModel
        }

        public  void Napravleniya()//DateTime start, DateTime end, ProgressBar progress)
        {
            var vm = DataContext as MedKomZayavkaViewModel;

            excelReport excel = new excelReport();
            excel.Init("Направление мед.xltx", $"Направление мед { DateTime.Now.Ticks}.xlsx");//,visible:true);//, otchetDir: otchetDir);

            int ri = 1;
            var db = new ProvodnikContext();            
             
            foreach (var r in vm.Persons)
            {
                ri++;
                excel.cell[ri, 1].value2 =ri-1;
                excel.cell[ri,2].value2 = vm.Dat?.ToString("dd.MM.yyyy");
                excel.cell[ri, 3].value2 = "ЛВЧ - 7 Новосибирск";
                excel.cell[ri,4].value2 = "Новосибирское РО";
                excel.cell[ri, 5].value2 = r.Fio;
                excel.cell[ri, 6].value2 = r.BirthDat?.ToString("dd.MM.yyyy");
                excel.cell[ri, 7].value2 = "ЧУЗ «КБ «РЖД-Медицина» г. Новосибирск»";
                excel.cell[ri, 8].value2 = "г. Новосибирск, ул. Сибирская, 21";
            }
            excel.setAllBorders(excel.get_Range("A1", "H" + ri));
            excel.myExcel.Visible = true;
        }

        private void AddFromListButton_Click(object sender, RoutedEventArgs e)
        {
            var vm= this.DataContext as MedKomZayavkaViewModel;
            var psw=new PersonsView(1);
            var pswVm = psw.DataContext as PersonsViewModel;

            pswVm.FindCommand.Execute(null);//psw.FindButton_Click(null,null);
            if (psw.ShowDialog() == true)
            {
                var db = new ProvodnikContext();
                foreach (var id in psw.vm.PersonList.Where(pp => pp.IsSelected).Select(pp=>pp.Id))
                {

                    if (vm.Persons.FirstOrDefault(pp => pp.PersonId == id) != null) continue;

                    var np = MainWindow.Mapper.Value.Map<MedKomZayavkaPersonViewModel>(db.Persons.First(pp => pp.Id == id));

                     np.MedKomZayavkaViewModel = vm;
                    vm.Persons.Add(np);
                    
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var vm= this.DataContext as MedKomZayavkaViewModel;
            if (!vm.Validator.IsValid)
                MessageBox.Show(string.Join(Environment.NewLine, vm.Validator.ValidationMessages));

            var db = new ProvodnikContext();

            MedKomZayavka p;
            if (vm.Id.HasValue)
            {
                p = db.MedKomZayavki.Single(pp => pp.Id == vm.Id.Value);

                MainWindow.Mapper.Value.Map(vm, p);
                var currents = vm.Persons.Select(pp => pp.PersonId).ToList();
                var toDelete = (from pd in db.MedKomZayavkaPersons
                                where pd.MedKomZayavkaId== p.Id && !currents.Contains(pd.PersonId)
                                select pd);
                db.MedKomZayavkaPersons.RemoveRange(toDelete);
                db.SaveChanges();
            }
            else
            { p = new MedKomZayavka();
                MainWindow.Mapper.Value.Map(vm, p);
                db.MedKomZayavki.Add( p);
                db.SaveChanges();
            }

            foreach (var d in vm.Persons)
            {
                MedKomZayavkaPerson pd;
                if (d.Id==null)
                {                    
                    db.MedKomZayavkaPersons.Add(pd = new MedKomZayavkaPerson() {MedKomZayavkaId=p.Id,PersonId=d.PersonId});
                    db.SaveChanges();
                }
            }
            DialogResult = true;

        }


        private void PersonsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                if (PersonsListView.SelectedItem != null &&
                MessageBox.Show("Удалить?", "Подтверждение удаления", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    var vm = this.DataContext as MedKomZayavkaViewModel;
                    var p = PersonsListView.SelectedItem as MedKomZayavkaPersonViewModel;
                    vm.Persons.Remove(p);                    
                }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //var vm = this.DataContext as MedKomZayavkaViewModel;
            //vm.IsChanged = false;
            DialogResult = false;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            Napravleniya();
        }

        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pe = ((FrameworkElement)e.OriginalSource).DataContext as MedKomZayavkaPersonViewModel;
            if (pe != null)
            {
                var vm = this.DataContext as MedKomZayavkaViewModel;
                var pw = new PersonView();
                var db = new ProvodnikContext();
              var pvm=  new PersonViewModel(pe.PersonId);
                pw.DataContext = pvm;
                if (pw.ShowDialog() == true)
                {

                    var ppp = new ProvodnikContext().Persons.First(pp => pp.Id == pe.PersonId);
                    var tmp=    MainWindow.Mapper.Value.Map(ppp, pe);

                    var ind = vm.Persons.IndexOf(pe);
                    vm.Persons.RemoveAt(ind);
                    vm.Persons.Insert(ind, pe);
                    //vm.RefreshPersonList();
                    //TODO goto if exist or add anyway and goto
                }
            }
        }

        private void PersonsListView_Sorting(object sender, DataGridSortingEventArgs e)
        {
            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                //runs after sorting is done
                Helper.SetPersonShortIndexes(PersonsListView);
            }, null);
        }
    }
}
