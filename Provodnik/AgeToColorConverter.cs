using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Provodnik
{
    public class AgeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = value as PersonShortViewModel;
            SolidColorBrush result = new SolidColorBrush(Colors.White);

            var age = vm.Vozrast;
            if (age.HasValue && age < CommonConsts.Sovershennolentie) result = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
            else
            {
                var ageOnYearStart=Helper.GetVozrast(vm.BirthDat,new DateTime(DateTime.Today.Year,1,1).AddDays(-1));
                if (ageOnYearStart==19) result = new SolidColorBrush(Color.FromArgb(50, 250, 196, 81));
            }

            return result;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
