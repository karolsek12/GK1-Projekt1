using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using GK1_1.Components;

namespace GK1_1.Converters
{
    public class ContinuityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Continuity c)
            {
                switch (c)
                {
                    case Continuity.G1:
                        return "G1";

                    case Continuity.C1:
                        return "C1";

                    case Continuity.G0: // TODO comment out the uncessecary ones
                        return "G0";
                    case Continuity.NotSet:
                        return "ns";
                    case Continuity.ControlPoint:
                        return "cp";
                    default:
                        return "";
                }
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
