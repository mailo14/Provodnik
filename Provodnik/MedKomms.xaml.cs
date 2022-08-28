using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
    public class MedKommsViewModel : System.ComponentModel.INotifyPropertyChanged//: PersonDoc
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

        public ObservableCollection<PersonShortViewModel> Persons { get; set; } = new ObservableCollection<PersonShortViewModel>();

        DateTime? _SelectedDate;
        public DateTime? SelectedDate
        {
            get => _SelectedDate;
            set
            {
                if (_SelectedDate != null && IsChanged)
                    switch (MessageBox.Show("Сохранить изменения?", "", MessageBoxButton.YesNoCancel))
                    {
                        case MessageBoxResult.Yes: Save(); break;
                        case MessageBoxResult.Cancel: return;
                    }
                _SelectedDate = value;
                ReloadPersons();
                OnPropertyChanged();
            }
        }

        public void ReloadPersons()
        {
            var dat = SelectedDate.Value;
            Persons.Clear();
            var db = new ProvodnikContext();
            var qq = (from pp in db.Persons where pp.NaprMedVidanoDat == dat orderby pp.Fio select pp);
            foreach (var q in qq)
                Persons.Add(MainWindow.Mapper.Value.Map<PersonShortViewModel>(q));

            Helper.SetPersonShortIndexes(Persons);

            IsChanged = false;
        }

        public void Save()
        {
            try
            {
                var dat = SelectedDate.Value;

                var currents = Persons.Select(pp => pp.Id).ToList();
                using (var db = new ProvodnikContext())
                {
                    var toDelete = (from pd in db.Persons
                                    where pd.NaprMedVidanoDat == dat && !currents.Contains(pd.Id)
                                    select pd).ToList();
                    if (toDelete.Any())
                    {
                        MessageBox.Show("Данные о мед.комиссии будут очищены у удаленных: "
                            + Environment.NewLine + string.Join(Environment.NewLine, toDelete.Select(pp => pp.Fio)));
                        foreach (var pd in toDelete)
                        {
                            pd.NaprMedVidanoDat = null;
                            //pd.IsMedKomm = false; TODO clear scans and FillMessagesAndAlls

                            foreach (var pdo in (from pdo in db.PersonDocs
                                                 where pdo.PersonId == pd.Id && (pdo.DocTypeId == DocConsts.ЗаключениеВЭК || pdo.DocTypeId == DocConsts.ЗаключениеВЭК2)//>= 6 && pdo.DocTypeId <= 8
                                                 select pdo))
                                pdo.FileName = null;
                            db.SaveChanges();

                            var pvm = new PersonViewModel(pd.Id, false);
                            pvm.FillMessagesAndAlls(pd);
                            db.SaveChanges();
                        }
                    }
                }

                using (var db = new ProvodnikContext())
                {
                    var news = (from pd in db.Persons
                                where pd.NaprMedVidanoDat != dat && currents.Contains(pd.Id)
                                select pd).ToList();
                    foreach (var p in news)
                    {
                        foreach (var pdo in (from pdo in db.PersonDocs
                                             where pdo.PersonId == p.Id && (pdo.DocTypeId == DocConsts.ЗаключениеВЭК || pdo.DocTypeId == DocConsts.ЗаключениеВЭК2)//>= 6 && pdo.DocTypeId <= 8
                                             select pdo))
                            pdo.FileName = null;
                        db.SaveChanges();
                    }
                }

                using (var db = new ProvodnikContext())
                {
                    foreach (var p in Persons)
                    {
                        var pe = db.Persons.First(pp => pp.Id == p.Id);
                        pe.NaprMedVidanoDat = dat;
                        //pe.IsMedKomm = p.IsMedKomm;
                        pe.IsNaprMedVidano = p.IsNaprMedVidano;
                        //pe.IsPsih = p.IsPsih;
                        //if (!pe.IsPsih && pe.PsihDat.HasValue) pe.PsihDat = null;
                        //pe.IsPsihZabral = p.IsPsihZabral;
                        db.SaveChanges();

                        var pvm = new PersonViewModel(pe.Id, false);
                        pvm.FillMessagesAndAlls(pe);
                        db.SaveChanges();
                    }
                }
                IsChanged = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении" + Environment.NewLine + ex.Message);
            }
        }

        public void AddPersons(IEnumerable<int> ids)
        {
            var db = new ProvodnikContext();
            foreach (var id in ids)
            {
                if (Persons.FirstOrDefault(pp => pp.Id == id) != null) continue;

                var np = MainWindow.Mapper.Value.Map<PersonShortViewModel>(db.Persons.First(pp => pp.Id == id));
                //np.IsMedKomm = false; TODO clear scans and FillMessagesAndAlls?
                Persons.Add(np);
            }
            IsChanged = true;
        }

    }

    /// <summary>
    /// Логика взаимодействия для MedKomms.xaml
    /// </summary>
    public partial class MedKomms : Window
    {
        public MedKomms()
        {
            InitializeComponent();
            //GroupCalendar.SelectedDate = DateTime.Today;
            var vm = new MedKommsViewModel();
            DataContext = vm;
            vm.SelectedDate = DateTime.Today;
        }






        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var pe = ((FrameworkElement)e.OriginalSource).DataContext as PersonShortViewModel;
            if (pe != null)
            {
                var pw = new PersonView();
                var db = new ProvodnikContext();

                var pvm = new PersonViewModel(pe.Id);
                pw.DataContext = pvm;
                if (pw.ShowDialog() == true)
                {

                    var ppp = new ProvodnikContext().Persons.First(pp => pp.Id == pe.Id);
                    var tmp = MainWindow.Mapper.Value.Map(ppp, pe);

                    var vm = (DataContext as MedKommsViewModel);
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
                    (DataContext as MedKommsViewModel).Persons.Remove(p);
                    (DataContext as MedKommsViewModel).IsChanged = true;
                    Helper.SetPersonShortIndexes(PersonsListView);
                }
        }

        private void AddFromListButton_Click(object sender, RoutedEventArgs e)
        {
            var psw = new PersonsView(1);
            var pswVm = psw.DataContext as PersonsViewModel; pswVm.ReadyOnly = null;
            pswVm.SanknizkaExist = true;
            pswVm.MedKommExist = false;
            pswVm.ExceptVibil = true;
            pswVm.PersonSearch = null; //run find, should be last            pswVm.FindCommand.Execute(null);//psw.FindButton_Click(null, null);    


            if (psw.ShowDialog() == true)
            {
                (DataContext as MedKommsViewModel).AddPersons(psw.vm.PersonList.Where(pp => pp.IsSelected).Select(pp => pp.Id));
                Helper.SetPersonShortIndexes(PersonsListView);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MedKommsViewModel).Save();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as MedKommsViewModel).ReloadPersons();
        }


        private void PersonsListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as MedKommsViewModel).IsChanged = true;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = (DataContext as MedKommsViewModel);
            if (vm.IsChanged)
                switch (MessageBox.Show("Сохранить изменения?", "", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes: vm.Save(); break;
                    case MessageBoxResult.Cancel: e.Cancel = true; break;
                }
        }
        private async void ExcelButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = (DataContext as MedKommsViewModel);
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

        private void NapravleniyaButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = (DataContext as MedKommsViewModel);
            var noBirths = vm.Persons.Where(pp => !pp.BirthDat.HasValue).Select(pp => pp.Fio).ToList();
            if (noBirths.Any() &&
                MessageBox.Show("У следующих бойцов не заполнена дата рождения:"
                + Environment.NewLine + string.Join(", ", noBirths)
                + Environment.NewLine + "Продолжить?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            new Reporter().NapravleniyaMed(vm.Persons.ToList());

        }
        private void VedomostButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MedKommsViewModel;
            var db = new ProvodnikContext();
            if (vm.Persons.Count == 0) return;


            var path = (string.Format("{0}\\_шаблоны\\" + "Ведомость мед.комисии.dotx", AppDomain.CurrentDomain.BaseDirectory));
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application { Visible = false };
            Microsoft.Office.Interop.Word.Document aDoc = wordApp.Documents.Open(path, ReadOnly: false, Visible: false);
            aDoc.Activate();

            object missing = Missing.Value;

            Microsoft.Office.Interop.Word.Range range = aDoc.Content;
            range.Find.ClearFormatting();

            range.Find.Execute(FindText: "{дата}", ReplaceWith: vm.SelectedDate?.ToString("dd.MM.yyyy"), Replace: Microsoft.Office.Interop.Word.WdReplace.wdReplaceAll);


            var table = aDoc.Tables[1];
            int iRow = 2;

            foreach (var p in vm.Persons)
            {
                if (iRow > 2) table.Rows.Add(ref missing);
                table.Cell(iRow, 1).Range.Text = (iRow - 1).ToString() + '.';
                table.Cell(iRow, 2).Range.Text = p.Fio;
                table.Cell(iRow, 3).Range.Text = p.BirthDat?.ToString("dd.MM.yyyy");

                iRow++;
            }
            wordApp.Visible = true;
        }
    }
}
