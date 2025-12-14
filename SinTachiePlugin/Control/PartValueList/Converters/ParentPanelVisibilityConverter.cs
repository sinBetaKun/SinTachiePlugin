using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Parent;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SinTachiePlugin.Control.PartValueList.Converters
{
    internal class ParentPanelVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ControlledParametersOfPart cp)
                return Visibility.Collapsed;

            if (cp.SourceSelectArg is not IParentParameter parentParameter)
                return Visibility.Collapsed;

            if (parentParameter.Parent == string.Empty)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
