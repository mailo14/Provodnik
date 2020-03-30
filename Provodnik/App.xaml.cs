using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Provodnik
{
    public class Configuration
        {
        internal string FtpAdress;
        internal string FtpUser;
        internal string FtpPassw;

        //public string db
        public string DbConnection { get; internal set; }
        }
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static User user;

        internal static Configuration CurrentConfig
        //   = new Configuration { DbConnection = "LocalConnection",FtpAdress= "127.0.0.1", FtpUser= "provodnikAdmin", FtpPassw= "Qwerty123" };
             = new Configuration { DbConnection = "DefaultConnection" ,FtpAdress= "31.31.196.80", FtpUser="u0920601",FtpPassw= "XP83yno_" };

    public static void setCursor(bool wait = true, bool appStart = false)
        {
            if (appStart)
                Current.Dispatcher.BeginInvoke((Action)(() => { Mouse.OverrideCursor = Cursors.AppStarting; }));
            Current.Dispatcher.BeginInvoke((Action)(() => { Mouse.OverrideCursor = wait ? Cursors.Wait : null; }));
        }
        protected override void OnStartup(StartupEventArgs e)
        {
        }
        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).IsDropDownOpen = true;
        }
    }
}
