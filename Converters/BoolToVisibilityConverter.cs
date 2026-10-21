using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;
using GK1_1.Components.Edges;

namespace GK1_1.Converters
{
    public class BoolToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 &&
                values[0] is bool b &&
                values[1] is Edge e)
            {
                if (b || e.IsAlwaysVisible())
                {
                    return System.Windows.Visibility.Visible;
                }
            }
            return System.Windows.Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
