using SinTachiePlugin.Part;
using SinTachiePlugin.Part.Drawing.DrawingArg.Argument.SubArg;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument.Argument.Values;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SinTachiePlugin.Control.PartValueList.Converters
{
    internal class PriorityPanelVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ControlledParametersOfPart cp)
                return Visibility.Collapsed;

            if (cp.DrawingArg is not ISubArgParameter subArgParameter)
                return Visibility.Collapsed;

            if (subArgParameter.SubArg is not IValuesParameter)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
