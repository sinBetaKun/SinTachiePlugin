using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Parent;
using System.Globalization;
using System.Windows.Data;

namespace SinTachiePlugin.Control.PartValueList.Converters
{
    internal class ParentPanelValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ControlledParametersOfPart cp)
                return string.Empty;

            if (cp.SourceSelectArg is not IParentParameter parentParameter)
                return string.Empty;

            return parentParameter.Parent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
