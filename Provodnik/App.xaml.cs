using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Provodnik
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static User user;

        public static void setCursor(bool wait = true, bool appStart = false)
        {
            if (appStart)
                Current.Dispatcher.BeginInvoke((Action)(() => { Mouse.OverrideCursor = Cursors.AppStarting; }));
            Current.Dispatcher.BeginInvoke((Action)(() => { Mouse.OverrideCursor = wait ? Cursors.Wait : null; }));
        }
        protected override void OnStartup(StartupEventArgs e)
        {
        }
    }
}
