using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Provodnik
{
    partial class AboutBox1 : Form
    {
        public AboutBox1()
        {
            InitializeComponent();
            this.Text = String.Format("О {0}", "АСУ Проводник");
            //this.labelProductName.Text = "Пригород-Финанс";
            this.labelVersion.Text = String.Format("Версия {0}", AssemblyVersion);
        }

        #region Методы доступа к атрибутам сборки
        

        public string AssemblyVersion
        {
            get
            {
                try
                {
                    ApplicationDeployment a = ApplicationDeployment.CurrentDeployment;
                    return a.CurrentVersion.ToString();   // Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                catch { return ""; }
            }
        }        
        
        #endregion
    }
}
