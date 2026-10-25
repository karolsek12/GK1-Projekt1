using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using GK1_1.Components;

namespace GK1_1.Converters
{
    public class RadiusSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if((values.Length >= 2 &&
                values[0] is double br &&
                values[1] is double r
                ))
            {

                return new Size(r, r);
            }


            return new Size(0,0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
