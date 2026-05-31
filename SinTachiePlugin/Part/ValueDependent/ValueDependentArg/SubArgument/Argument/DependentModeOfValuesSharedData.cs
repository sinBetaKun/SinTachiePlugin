using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg.SubArgument.Argument
{
    internal class DependentModeOfValuesSharedData
    {
        public ValueDependentMode XYZ { get; set; } = ValueDependentMode.On;

        public ValueDependentMode Opacity { get; set; } = ValueDependentMode.On;

        public ValueDependentMode Zoom { get; set; } = ValueDependentMode.On;

        public ValueDependentMode Rotation { get; set; } = ValueDependentMode.On;

        public ValueDependentMode Invert { get; set; } = ValueDependentMode.On;

        public ValueDependentMode Camera { get; set; } = ValueDependentMode.On;

        public ValueDependentMode UnlazyEffect { get; set; } = ValueDependentMode.On;

        public DependentModeOfValuesSharedData()
        {
        }

        public DependentModeOfValuesSharedData(IDependentModeOfValueParameter parameter)
        {
            XYZ = parameter.XYZ;
            Opacity = parameter.Opacity;
            Zoom = parameter.Zoom;
            Rotation = parameter.Rotation;
            Invert = parameter.Invert;
            Camera = parameter.Camera;
            UnlazyEffect = parameter.UnlazyEffect;
        }

        public void CopyTo(IDependentModeOfValueParameter parameter)
        {
            parameter.XYZ = XYZ;
            parameter.Opacity = Opacity;
            parameter.Zoom = Zoom;
            parameter.Rotation = Rotation;
            parameter.Invert = Invert;
            parameter.Camera = Camera;
            parameter.UnlazyEffect = UnlazyEffect;
        }
    }
}
