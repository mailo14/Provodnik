﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class PersonsView : Window
    {
        public PersonsViewModel vm;
        int? groupId;
        public PersonsView(int? groupId)
        {
            InitializeComponent();
            vm= new PersonsViewModel();
            DataContext = vm;
            this.groupId = groupId;
            if (!groupId.HasValue)
                AddToGroupButton.Visibility = Visibility.Collapsed;
        }
        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).IsDropDownOpen = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var pw=new PersonView();
            pw.DataContext = new PersonViewModel(null);// new ProvodnikContext().Persons.FirstOrDefault()==null?(int?)null:1);
            if (pw.ShowDialog() == true)
            {
                vm.PersonSearch = pw.vm.Fio;//run find, should be last                vm.RefreshPersonList();

                //TODO goto if exist or add anyway and goto
                vm.InitCollectionsForCombo();
            }



        }

       /* public void FindButton_Click(object sender, RoutedEventArgs e)
        {
            vm.PersonSearch = null;
            vm.RefreshPersonList();
        }

       public void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            vm.ReadyOnly = null;
            vm.PersonSearch = null;
            vm.RefreshPersonList();
        }*/
        protected void HandleDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
        }
        void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var p = ((FrameworkElement)e.OriginalSource).DataContext as PersonShortViewModel;
            if (p!= null)
            {
                var pw = new PersonView();
                var db = new ProvodnikContext();
                var pvm =new PersonViewModel(p.Id);              
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
                MainWindow.p.CheckAlarms();
            }
        }

        private void AddToGroupButton_Click(object sender, RoutedEventArgs e)
        {
            if (vm.PersonList.Where(pp => pp.IsSelected).Any())
                DialogResult = true;
        }

        private void PersonsListView_KeyUp(object sender, KeyEventArgs e)
        {
            
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
