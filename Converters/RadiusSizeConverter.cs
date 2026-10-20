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

            if((values.Length >= 4 &&
                values[0] is int x1 &&
                values[1] is int y1 &&
                values[2] is int x2 &&
                values[3] is int y2 
                ))
            {
                int diff1 = x1 - x2;
                int diff2 = y1 - y2;
                double radius = Math.Sqrt(diff1*diff1 + diff2*diff2)/2.0;

                return new Size(radius, radius);
            }


            return new Size(0,0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
