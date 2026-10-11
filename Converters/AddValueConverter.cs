using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GK1_1.Converters
{
    public class AddValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) // sciagniete ze StackOverflow
        {
            object result = value;
            int parameterValue;

            if (value != null && targetType == typeof(Double) &&
                int.TryParse((string)parameter,
                NumberStyles.Integer, culture, out parameterValue))
            {
                result = (int)value + (int)parameterValue;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
