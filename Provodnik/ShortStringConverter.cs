using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Provodnik
{
    public class ShortStringConverter
        : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              System.Globalization.CultureInfo culture)
        {
            var values = value as string;
            if (values == null) return null;
            values = values.Replace(Environment.NewLine, "; ");
            if (values.Length > 50)
                return values.Substring(0, 47) + "...";
            return values;
        }


        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
