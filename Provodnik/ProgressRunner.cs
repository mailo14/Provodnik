using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provodnik
{
    public interface IProgressWindow
    {
        void ProgressChanged(double progress, string msg = null);
        void Show();
        void Close();
    }

    public delegate void ProgressHandler(double progressToAdd, string msg = null);

    public class ProgressRunner
    {
        //[STAThread]
        public async Task RunAsync(Action<ProgressHandler> func)
        {
            IProgressWindow progress = new ProgressWindow();
            ProgressHandler ProgressChanged =progress.ProgressChanged; //(p, m) => { }; //
            progress.Show();
            try
            {
                
                   await Task.Run(() => func(ProgressChanged));
            }
            finally
            {
                progress.Close();
            }
        }
    }
    // event ProgressHandler ProgressChanged;
}
