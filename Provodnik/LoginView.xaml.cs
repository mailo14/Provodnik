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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
        {
            public static LoginView p;
            public static bool isMasterMode = false;
            public static bool isWarnedThatReadOnly = false;
            public LoginView()
        {

            Application.Current.DispatcherUnhandledException += (s, ex) =>
            {
                if (ex != null)
                {
                    MessageBox.Show(ex.Exception.ToString());
                }
            };
            /* var s = "123".GetHashCode().ToString();
             var db = new ProvodnikContext();
             foreach (var l in db.Users)
                 l.Wand = s;
             db.SaveChanges();*/


            try
                {
                    bool isFirstTime = (p == null);
                    p = this;
                    InitializeComponent();

                    isonload = MainWindow.p == null;
                    rowNewPass.Height = new GridLength(0);
                    if (isonload)
                    {
                        if (Properties.Settings.Default.userId > 0)
                        {
                            App.user = new ProvodnikContext().Users.FirstOrDefault(pp => pp.Id == Properties.Settings.Default.userId);//+safedb
                            rabPopup.Text=App.user.Login;
                            passwordBox.Focus();
                        }
                    }
                    else
                    {
                        Title = "Сменa пользователя";
                        buttonOpen.Content = "Сменить пользователя";
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message + " " + ex.StackTrace.ToString() + " " + ex.Data.ToString()); }

                // MessageBox.Show("вход end");

            }
       /*
            public LoginView(RabotnikSmeni rabSm)
            {
                Rabotnik rab;
                if (rabSm != null)
                {
                    rab = db.Rabotnik.First(pp => pp.id == rabSm.rabId);
                    rabPopup.selElem =
                        new PopUpTextBoxTab.Elem() { id = rab.id, name = rab.name, displayName = Смены.getShortName(rab.name), param = rab };
                    rabPopup.Text = rabPopup.selElem.displayName;
                }
                if (rabSm != null)
                {
                    passwordBox.Focus();

                }               
            }*/


            private void Window_Loaded(object sender, RoutedEventArgs e)
            {

            }

            private void TextBoxSelectAll(object sender, RoutedEventArgs e)
            {
                TextBox tb = (sender as TextBox);
                if (tb != null)
                {
                    tb.SelectAll();
                }
            }

            private void TextBoxSelectivelyIgnoreMouseButton(object sender,
                MouseButtonEventArgs e)
            {
                TextBox tb = (sender as TextBox);
                if (tb != null)
                {
                    if (!tb.IsKeyboardFocusWithin)
                    {
                        e.Handled = true;
                        tb.Focus();
                    }
                }
            }

            private bool isonload;



            private void buttonOpen_Click(object sender, RoutedEventArgs e)
            {
                string error = null;
                // Rabotnik r = null;  
                if (string.IsNullOrWhiteSpace(rabPopup.Text))
                    error = "Не выбран пользователь!";
                else
                {
                    App.user = new ProvodnikContext().Users.First(pp => pp.Login== rabPopup.Text);//rabPopup.selElem.param as Rabotnik;
                    if (App.user.Wand != passwordBox.Password.GetHashCode().ToString())
                        error = "Не верный пароль!";
                }
                if (rowNewPass.Height.IsAuto && newPasswordBox.Password.Length == 0 && isonload)
                    error = "Пустой новый пароль!";
                if (error != null) { MessageBox.Show(error, "Ошибка"); return; }
                /**/if (passwordBox.Password == "123" && rowNewPass.Height.IsAbsolute
                        && MessageBox.Show("Рекомендуется сменить стандартный пароль! Сменить сейчас?", "Смена пароля", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    rowNewPass.Height = new GridLength(1, GridUnitType.Auto);
                    newPasswordBox.Focus();
                    return;
                }
                if (newPasswordBox.Password.Length > 0)
                {
                if (!checkNewPassword(newPasswordBox.Password))
                {
                    MessageBox.Show(error, "Пароль слишком простой");
                    return;
                }
                    /* SafeQuery.RunAsync(() =>
                    SafeQuery.Run(() =>
                    {*/
                    App.user.Wand = newPasswordBox.Password.GetHashCode().ToString();
                using (var db = new ProvodnikContext())
                {
                    db.Users.First(pp => pp.Id == App.user.Id).Wand = App.user.Wand;
                    db.SaveChanges();
                }
                    /* }, 15000, () => { })
                     , () => {//MessageBox.Show("Пароль изменен!");
                     });*/
                }
                Properties.Settings.Default.userId = App.user.Id; Properties.Settings.Default.Save();
                if (buttonOpen.Content == "Сменить пользователя")
                    MessageBox.Show("Перезапустите программу");
                else
                new MainWindow().Show(); 
                Close();               
            }

        private bool checkNewPassword(string password)
        {
            if (password.Length < 3 || password.StartsWith("123") || password.StartsWith("111")) return false;
            return true;
        }

        private void buttonOut_Click(object sender, RoutedEventArgs e)
            {
                rabPopup.Text = "";
                rabPopup.Focus();
                rowPass.Height = rowNewPass.Height = new GridLength(0);
            }

            private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
            {
                //if (DialogResult==null) 
            }

        private void passwordBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                buttonOpen_Click(null, null);
        }
    }
    }