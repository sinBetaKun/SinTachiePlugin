using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SinTachiePlugin.Control.PartValueList.Converters
{
    internal class DepthMarginConverter : IValueConverter
    {
        const double Coef = 20;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double left = System.Convert.ToDouble(value);
            return new Thickness(left * Coef, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
