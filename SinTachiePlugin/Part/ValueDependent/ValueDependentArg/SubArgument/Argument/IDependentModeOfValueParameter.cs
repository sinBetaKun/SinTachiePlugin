using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg.SubArgument.Argument
{
    internal interface IDependentModeOfValueParameter
    {
        public ValueDependentMode XYZ { get; set; }

        public ValueDependentMode Opacity { get; set; }

        public ValueDependentMode Zoom { get; set; }

        public ValueDependentMode Rotation { get; set; }

        public ValueDependentMode Invert { get; set; }

        public ValueDependentMode Camera { get; set; }

        public ValueDependentMode UnlazyEffect { get; set; }
    }
}
