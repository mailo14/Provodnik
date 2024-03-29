﻿using System;
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
    public class ExamensViewModel : System.ComponentModel.INotifyPropertyChanged//: PersonDoc
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
            var qq = (from pp in db.Persons where pp.ExamenDat == dat orderby pp.Fio select pp);
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
                                    where pd.ExamenDat == dat && !currents.Contains(pd.Id)
                                    select pd).ToList();
                    if (toDelete.Any())
                    {
                        MessageBox.Show("Данные о обучении будут очищены у удаленных: "
                            + Environment.NewLine + string.Join(Environment.NewLine, toDelete.Select(pp => pp.Fio)));
                        foreach (var pd in toDelete)
                        {
                            pd.ExamenDat = null;
                            pd.IsExamen = false;
                            foreach (var pdo in (from pdo in db.PersonDocs
                                                 where pdo.PersonId == pd.Id && pdo.DocTypeId == DocConsts.СвидетельствоПрофессии
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
                                where pd.ExamenDat != dat && currents.Contains(pd.Id)
                                select pd).ToList();
                    foreach (var p in news)
                    {
                        foreach (var pdo in (from pdo in db.PersonDocs
                                             where pdo.PersonId == p.Id && pdo.DocTypeId == DocConsts.СвидетельствоПрофессии
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
                        pe.ExamenDat = dat;
                        //pe.IsPraktika = p.IsPraktika;
                        pe.IsExamen = p.IsExamen;
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
                np.IsExamen = false;
                Persons.Add(np);
            }
            IsChanged = true;
        }
    }

    /// <summary>
    /// Логика взаимодействия для Examens.xaml
    /// </summary>
    public partial class Examens : Window
    {
        public Examens()
        {
            InitializeComponent();
            //GroupCalendar.SelectedDate = DateTime.Today;
            var vm = new ExamensViewModel();
            DataContext = vm;
            vm.SelectedDate= DateTime.Today;
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

                    var vm = (DataContext as ExamensViewModel);
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
                    (DataContext as ExamensViewModel).Persons.Remove(p);
                    (DataContext as ExamensViewModel).IsChanged = true;
                    Helper.SetPersonShortIndexes(PersonsListView);
                }
        }

        private void AddFromListButton_Click(object sender, RoutedEventArgs e)
        {            
            var psw = new PersonsView(1);
            var pswVm=psw.DataContext as PersonsViewModel; pswVm.ReadyOnly = null;
            pswVm.ExamenExist = false;
            pswVm.PraktikaExist =true;
            pswVm.SanknizkaExist = pswVm.MedKommExist = true;
            pswVm.ExceptVibil = true;
 pswVm.PersonSearch = null; //run find, should be last            pswVm.FindCommand.Execute(null);//psw.FindButton_Click(null, null);    


            if (psw.ShowDialog() == true)
            {
                (DataContext as ExamensViewModel).AddPersons(psw.vm.PersonList.Where(pp => pp.IsSelected).Select(pp => pp.Id));
                Helper.SetPersonShortIndexes(PersonsListView);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ExamensViewModel).Save();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ExamensViewModel).ReloadPersons();
        }

        

        private void GroupCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        private void PersonsListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            (DataContext as ExamensViewModel).IsChanged = true;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = (DataContext as ExamensViewModel);
            if (vm.IsChanged)
                switch (MessageBox.Show("Сохранить изменения?", "", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes: vm.Save(); break;
                    case MessageBoxResult.Cancel: e.Cancel=true; break;
                }
        }
        private async void ExcelButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = (DataContext as ExamensViewModel);
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
