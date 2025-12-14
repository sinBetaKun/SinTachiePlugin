using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SinTachiePlugin.Control.PartValueList.Converters
{
    internal class OpenGroupButtonIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not ControlledParametersOfPart cp)
                return new PathGeometry();

            if (cp.SourceSelectArg is not GroupParameter gp)
                return new PathGeometry();

            if (gp.IsOpened)
                return Geometry.Parse("M8,8 L16,0 L0,0 Z");

            return Geometry.Parse("M8,0 L16,8 L0,8 Z");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Windows.Data.Binding.DoNothing;
        }
    }
}
