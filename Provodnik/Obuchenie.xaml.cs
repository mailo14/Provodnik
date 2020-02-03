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
    public class ObuchenieViewModel : System.ComponentModel.INotifyPropertyChanged//: PersonDoc
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(prop));
        }

        bool _IsChanged;
        public bool IsChanged
        {
            get => _IsChanged;
            set
            {
                _IsChanged = value;
                OnPropertyChanged();
            }
        }

        string _UchebGruppa;
        public string UchebGruppa
        {
            get => _UchebGruppa;
            set
            {
                _UchebGruppa = value;
                OnPropertyChanged();
            }
        }

        string _UchebCentr;
        public string UchebCentr
        {
            get => _UchebCentr;
            set
            {
                _UchebCentr = value;
                OnPropertyChanged();
            }
        }

        DateTime? _UchebStartDat;
        public DateTime? UchebStartDat
        {
            get => _UchebStartDat;
            set
            {
                _UchebStartDat = value;
                OnPropertyChanged();
            }
        }

        DateTime? _UchebEndDat;
        private UchebGruppaViewModel BaseUchebGruppa;


        public List<string> UchebCentri { get; set; }
        public ObuchenieViewModel(UchebGruppaViewModel ug)
        {
            BaseUchebGruppa = ug;

            UchebCentri = new Repository().GetUchebCentri();
        }

        public DateTime? UchebEndDat
        {
            get => _UchebEndDat;
            set
            {
                _UchebEndDat = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<PersonShortViewModel> Persons { get; set; } = new ObservableCollection<PersonShortViewModel>();

        public void ReloadPersons()
        {
            Persons.Clear();

            var db = new ProvodnikContext();
            var qq = (from p in db.Persons
                      where p.UchebGruppa == UchebGruppa && p.UchebCentr == UchebCentr
                      && p.UchebStartDat == UchebStartDat
                      && p.UchebEndDat == UchebEndDat
                      orderby p.Fio
                      select p).ToList();
            foreach (var q in qq)
                Persons.Add(MainWindow.Mapper.Value.Map<PersonShortViewModel>(q));

            Helper.SetPersonShortIndexes(Persons);

            IsChanged = false;
        }

        public void Save()
        {
            if (UchebGruppa == null || UchebCentr == null || UchebStartDat == null || UchebEndDat == null)
            {
                MessageBox.Show("Данные группы не заполнены!");
                return;
            }
            if (Persons.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одного участника!");
                return;
            }


            if (BaseUchebGruppa != null)
            {
                var currents = Persons.Select(pp => pp.Id).ToList();
                using (var db = new ProvodnikContext())
                {
                    var toDelete = (from p in db.Persons
                                    where p.UchebGruppa == BaseUchebGruppa.UchebGruppa && p.UchebCentr == BaseUchebGruppa.UchebCentr
                                    && p.UchebStartDat == BaseUchebGruppa.UchebStartDat
                                    && p.UchebEndDat == BaseUchebGruppa.UchebEndDat
                                    && !currents.Contains(p.Id)
                                    select p).ToList();
                    if (toDelete.Any())
                    {
                        MessageBox.Show("Данные о обучении будут очищены у удаленных: "
                            + Environment.NewLine + string.Join(Environment.NewLine, toDelete.Select(pp => pp.Fio)));
                        foreach (var p in toDelete)
                        {
                            p.UchebGruppa = null;
                            p.UchebCentr = null;
                            p.UchebStartDat = null;
                            p.UchebEndDat = null;
                        }
                        db.SaveChanges();
                    }
                }
            }

            using (var db = new ProvodnikContext())
            {
                foreach (var p in Persons)
                {
                    var pe = db.Persons.First(pp => pp.Id == p.Id);
                    pe.UchebGruppa = UchebGruppa;
                    pe.UchebCentr = UchebCentr;
                    pe.UchebStartDat = UchebStartDat;
                    pe.UchebEndDat = UchebEndDat;
                }
                db.SaveChanges();
            }
            IsChanged = false;
        }

        public void AddPersons(IEnumerable<int> ids)
        {
            var db = new ProvodnikContext();
            foreach (var id in ids)
            {
                if (Persons.FirstOrDefault(pp => pp.Id == id) != null) continue;

                var np = MainWindow.Mapper.Value.Map<PersonShortViewModel>(db.Persons.First(pp => pp.Id == id));
                Persons.Add(np);
            }
            IsChanged = true;
        }
    }

    /// <summary>
    /// Логика взаимодействия для Obuchenie.xaml
    /// </summary>
    public partial class Obuchenie : Window
    {
        public Obuchenie(UchebGruppaViewModel ug)
        {
            InitializeComponent();

            var vm = new ObuchenieViewModel(ug);
            if (ug != null)
            {
                vm.UchebGruppa = ug.UchebGruppa;
                vm.UchebCentr = ug.UchebCentr;
                vm.UchebStartDat = ug.UchebStartDat;
                vm.UchebEndDat = ug.UchebEndDat;

                vm.ReloadPersons();
            }
            DataContext = vm;
        }

        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pe = ((FrameworkElement)e.OriginalSource).DataContext as PersonShortViewModel;
            if (pe != null)
            {
                var pw = new PersonView();
                var db = new ProvodnikContext();

                var pvm =new PersonViewModel(pe.Id);
                pw.DataContext = pvm;
                if (pw.ShowDialog() == true)
                {

                    var ppp = new ProvodnikContext().Persons.First(pp => pp.Id == pe.Id);
                    var tmp = MainWindow.Mapper.Value.Map(ppp, pe);

                    var vm = (DataContext as ObuchenieViewModel);
                    var ind = vm.Persons.IndexOf(pe);
                    vm.Persons.RemoveAt(ind);
                    vm.Persons.Insert(ind, pe);
                    //vm.RefreshPersonList();
                    //TODO goto if exist or add anyway and goto
                }
            }
        }

        private void PersonsListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                if (PersonsListView.SelectedItem != null &&
                MessageBox.Show("Удалить?", "Подтверждение удаления", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                {                    
                    var p = PersonsListView.SelectedItem as PersonShortViewModel;
                    (DataContext as ObuchenieViewModel).Persons.Remove(p);
                    (DataContext as ObuchenieViewModel).IsChanged = true;
                }
        }

        private void AddFromListButton_Click(object sender, RoutedEventArgs e)
        {            
            var psw = new PersonsView(1);
            var pswVm=psw.DataContext as PersonsViewModel; pswVm.ReadyOnly = null;
            pswVm.ExamenExist = false;
            pswVm.PraktikaExist =false;
            pswVm.ExceptVibil = true;
 pswVm.PersonSearch = null; //run find, should be last            pswVm.FindCommand.Execute(null);//psw.FindButton_Click(null, null);    


            if (psw.ShowDialog() == true)
            {
                (DataContext as ObuchenieViewModel).AddPersons(psw.vm.PersonList.Where(pp => pp.IsSelected).Select(pp => pp.Id));
               
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ObuchenieViewModel).Save();
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ObuchenieViewModel).ReloadPersons();
            DialogResult = true;
        }
                


        private void PersonsListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as ObuchenieViewModel).IsChanged = true;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = (DataContext as ObuchenieViewModel);
            if (vm.IsChanged)
                switch (MessageBox.Show("Сохранить изменения?", "", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes: vm.Save(); break;
                    case MessageBoxResult.Cancel: e.Cancel=true; break;
                }
        }
        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).IsDropDownOpen = true;
        }

        private async void ExcelButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = (DataContext as ObuchenieViewModel);
            new Reporter().ExportToExcel(vm.Persons.Select(pp => pp.Id).ToList());
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
