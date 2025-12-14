using SinTachiePlugin.Part;
using SinTachiePlugin.Part.Drawing.DrawingArg.Argment.SubArg;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment.Argment.Values;
using System.Globalization;
using System.Windows.Data;

namespace SinTachiePlugin.Control.PartValueList.Converters
{
    internal class PriorityPanelValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ControlledParametersOfPart cp)
                return string.Empty;

            if (cp.DrawingArg is not ISubArgParameter subArgParameter)
                return string.Empty;

            if (subArgParameter.SubArg is not IValuesParameter valuesParameter)
                return string.Empty;

            return valuesParameter.Priority.Values[0].Value.ToString("F1");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
