using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment.Argment.CustomMode
{
    internal class CustomModeSharedData
    {
        public DrawingValueMode XYZMode { get; set; } = DrawingValueMode.Override;

        public DrawingValueMode OpacityMode { get; set; } = DrawingValueMode.Override;

        public DrawingValueMode ZoomMode { get; set; } = DrawingValueMode.Override;

        public DrawingValueMode RotationMode { get; set; } = DrawingValueMode.Override;

        public DrawingValueMode InverseMode { get; set; } = DrawingValueMode.Override;

        public DrawingValueMode BlendMode { get; set; } = DrawingValueMode.Override;

        public DrawingValueMode ZSortMode { get; set; } = DrawingValueMode.Override;

        public DrawingValueMode PriorityMode { get; set; } = DrawingValueMode.Override;

        public CustomModeSharedData()
        {
        }

        public CustomModeSharedData(ICustomModeParameter parameter)
        {
            XYZMode = parameter.XYZMode;
            OpacityMode = parameter.OpacityMode;
            ZoomMode = parameter.ZoomMode;
            RotationMode = parameter.RotationMode;
            InverseMode = parameter.InverseMode;
            BlendMode = parameter.BlendMode;
            ZSortMode = parameter.ZSortMode;
            PriorityMode = parameter.PriorityMode;
        }

        public void CopyTo(ICustomModeParameter parameter)
        {
            parameter.XYZMode = XYZMode;
            parameter.OpacityMode = OpacityMode;
            parameter.ZoomMode = ZoomMode;
            parameter.RotationMode = RotationMode;
            parameter.InverseMode = InverseMode;
            parameter.BlendMode = BlendMode;
            parameter.ZSortMode = ZSortMode;
            parameter.PriorityMode = PriorityMode;
        }
    }
}
