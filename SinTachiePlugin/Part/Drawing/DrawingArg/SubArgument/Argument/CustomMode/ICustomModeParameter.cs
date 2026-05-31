using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument.Argument.CustomMode
{
    internal interface ICustomModeParameter
    {
        public DrawingValueMode XYZMode { get; set; }

        public DrawingValueMode OpacityMode { get; set; }

        public DrawingValueMode ZoomMode { get; set; }

        public DrawingValueMode RotationMode { get; set; }

        public DrawingValueMode InverseMode { get; set; }

        public DrawingValueMode BlendMode { get; set; }

        public DrawingValueMode ZSortMode { get; set; }

        public DrawingValueMode PriorityMode { get; set; }
    }
}
