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
    public partial class TrudoustroistvoView : Window
    {
        public TrudoustroistvoView()
        {
            InitializeComponent();
         //   this.DataContext = new PersonViewModel(new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);//TrudoustroistvoViewModel
        }

        private void AddFromListButton_Click(object sender, RoutedEventArgs e)
        {
            var vm= this.DataContext as TrudoustroistvoViewModel;
            var psw=new PersonsView(1);
            var pswVm = psw.DataContext as PersonsViewModel;

            pswVm.FindCommand.Execute(null);//psw.FindButton_Click(null,null);
            if (psw.ShowDialog() == true)
            {
                var db = new ProvodnikContext();
                foreach (var per in psw.vm.PersonList.Where(pp => pp.IsSelected))
                {
                    var id = per.Id;

                    if (vm.Persons.FirstOrDefault(pp => pp.PersonId == id) != null) continue;

                    var alreadyIncludedPeriod = (from t in db.Trudoustroistva join tp in db.TrudoustroistvoPersons on t.Id equals tp.TrudoustroistvoId
                                           where tp.PersonId == id && t.StartDate.Year == DateTime.Now.Year
                                           select t).FirstOrDefault();
                    if (alreadyIncludedPeriod != null)
                    {
                        MessageBox.Show($"Боец {per.Fio} уже распределен в {alreadyIncludedPeriod.Depo} {alreadyIncludedPeriod.StartDate.ToString("dd.MM.yyyy")}-{alreadyIncludedPeriod.EndDate.ToString("dd.MM.yyyy")}");
                        continue;
                    }

                    var np = MainWindow.Mapper.Value.Map<TrudoustroistvoPersonViewModel>(db.Persons.First(pp => pp.Id == id));

                     np.TrudoustroistvoViewModel = vm;
                    vm.Persons.Add(np);
                    
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as TrudoustroistvoViewModel;
            if (!vm.Validator.IsValid)
                MessageBox.Show(string.Join(Environment.NewLine, vm.Validator.ValidationMessages));

            vm.Kolvo = vm.Persons.Count;

            var db = new ProvodnikContext();

            Trudoustroistvo p;
            if (vm.Id.HasValue)
            {
                p = db.Trudoustroistva.Single(pp => pp.Id == vm.Id.Value);

                MainWindow.Mapper.Value.Map(vm, p);
                var currents = vm.Persons.Select(pp => pp.PersonId).ToList();
                var toDelete = (from pd in db.TrudoustroistvoPersons
                                where pd.TrudoustroistvoId == p.Id && !currents.Contains(pd.PersonId)
                                select pd);
                var personsToDeleteIds = toDelete.Select(x => x.PersonId).ToList();
                db.TrudoustroistvoPersons.RemoveRange(toDelete);
                db.SaveChanges();
            }
            else
            {
                p = new Trudoustroistvo();
                MainWindow.Mapper.Value.Map(vm, p);
                db.Trudoustroistva.Add(p);
                db.SaveChanges();
            }

            foreach (var d in vm.Persons)
            {
                TrudoustroistvoPerson pd;
                if (d.Id == null)
                {
                    db.TrudoustroistvoPersons.Add(pd = new TrudoustroistvoPerson() { TrudoustroistvoId = p.Id, PersonId = d.PersonId });
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
                    var vm = this.DataContext as TrudoustroistvoViewModel;
                    var p = PersonsListView.SelectedItem as TrudoustroistvoPersonViewModel;
                    vm.Persons.Remove(p);                    
                }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //var vm = this.DataContext as TrudoustroistvoViewModel;
            //vm.IsChanged = false;
            DialogResult = false;
        }

        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pe = ((FrameworkElement)e.OriginalSource).DataContext as TrudoustroistvoPersonViewModel;
            if (pe != null)
            {
                var vm = this.DataContext as TrudoustroistvoViewModel;
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

        private async void ExcelButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = (DataContext as ObuchenieViewModel);
            new Reporter().ExportToExcel(vm.Persons.Select(pp => pp.Id).ToList());
        }
    }
}
