using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace Provodnik
{    
    public class UchebGruppaViewModel
    {
        public string UchebGruppa { get; set; }
        public string UchebCentr { get; set; }
        public DateTime? UchebStartDat { get; set; }
        public DateTime? UchebEndDat { get; set; }
    }

    public class ObucheniyasViewModel : System.ComponentModel.INotifyPropertyChanged//: PersonDoc
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
        }

        public ObservableCollection<UchebGruppaViewModel> List { get; set; } = new ObservableCollection<UchebGruppaViewModel>();
        
        public void RefreshList()
        {
            List.Clear();
            var db = new ProvodnikContext();
            var yearStart = DateTime.Today;
            yearStart = yearStart.AddDays(-yearStart.Day + 1).AddMonths(-yearStart.Month + 1);
            var qq = (from p in db.Persons
                 where //p.UchebStartDat > yearStart && 
                 p.UchebGruppa != null
                 orderby p.UchebStartDat descending
                 select new { p.UchebGruppa, p.UchebStartDat, p.UchebEndDat, p.UchebCentr }).Distinct()
                              .Select(pp => new UchebGruppaViewModel()
                              {
                                  UchebCentr = pp.UchebCentr,
                                  UchebGruppa = pp.UchebGruppa,
                                  UchebStartDat = pp.UchebStartDat,
                                  UchebEndDat = pp.UchebEndDat
                              });
            foreach (var q in qq)
                List.Add(q);
        }
    }

    /// <summary>
    /// Логика взаимодействия для Obucheniyas.xaml
    /// </summary>
    public partial class Obucheniyas : Window
    {
        public Obucheniyas()
        {
            InitializeComponent();
            var vm = new ObucheniyasViewModel();
            vm.RefreshList();
            DataContext = vm;            
        }


       

        

        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pvm = ((FrameworkElement)e.OriginalSource).DataContext as UchebGruppaViewModel;
            if (pvm != null)
            {
                var pw = new Obuchenie(pvm);

                if (pw.ShowDialog() == true)
                {

                    var vm = (DataContext as ObucheniyasViewModel);
                    vm.RefreshList();
                    //TODO goto if exist or add anyway and goto
                }
            }
        }

        private void PersonsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                if (PersonsListView.SelectedItem != null &&
                MessageBox.Show("Удалить группу? У участников будут очищены соотвествующие поля", "Подтверждение удаления", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {
                    var ug = PersonsListView.SelectedItem as UchebGruppaViewModel;
                    var vm = DataContext as ObucheniyasViewModel;

                    using (var db = new ProvodnikContext())
                    {
                        var toDelete = (from p in db.Persons
                                        where p.UchebGruppa == ug.UchebGruppa && p.UchebCentr == ug.UchebCentr
                                        && p.UchebStartDat == ug.UchebStartDat
                                        && p.UchebEndDat == ug.UchebEndDat
                                        select p).ToList();
                        foreach (var p in toDelete)
                        {
                            p.UchebGruppa = null;
                            p.UchebCentr = null;
                            p.UchebStartDat = null;
                            p.UchebEndDat = null;
                        }
                        db.SaveChanges();
                        vm.List.Remove(ug);
                    }
                }
        }

        private void AddFromListButton_Click(object sender, RoutedEventArgs e)
        {            
            var psw = new Obuchenie(null);

            if (psw.ShowDialog() == true)
            {                
                    var vm = DataContext as ObucheniyasViewModel;
           var pswVm=psw.DataContext as ObuchenieViewModel; 
                vm.List.Insert(0, MainWindow.Mapper.Value.Map<UchebGruppaViewModel>(pswVm));               
            }
        }
    }
}
