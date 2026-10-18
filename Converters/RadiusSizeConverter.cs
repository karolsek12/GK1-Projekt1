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
                values[0] is Vertex p1 &&
                values[1] is Vertex p2))
            {
                int diff1 = p1.X - p2.X;
                int diff2 = p1.Y - p2.Y;
                double radius = Math.Sqrt(diff1*diff1 + diff2*diff2);

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
