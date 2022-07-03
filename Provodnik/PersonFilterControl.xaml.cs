﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Provodnik
{
    /// <summary>
    /// Логика взаимодействия для PersonFilterControl.xaml
    /// </summary>
    public partial class PersonFilterControl : UserControl
    {
        public PersonFilterControl()
        {
            InitializeComponent();
        }

        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).IsDropDownOpen = true;
        }

    }
}
