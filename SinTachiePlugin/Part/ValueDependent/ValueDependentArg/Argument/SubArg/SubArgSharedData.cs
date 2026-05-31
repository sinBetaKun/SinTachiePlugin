using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.SubArgument;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.SubArgument.Parameter;

namespace SinTachiePlugin.Part.ValueDependent.ValueDependentArg.Argument.SubArg
{
    internal class SubArgSharedData
    {
        public ValueDependentSubArgBase SubArg { get; set; } = new MasterModeParameter();

        public SubArgSharedData()
        {
        }

        public SubArgSharedData(ISubArgParameter parameter)
        {
            SubArg = parameter.SubArg.GetClone();
        }

        public void CopyTo(ISubArgParameter parameter)
        {
            parameter.SubArg = SubArg.GetClone();
        }
    }
}
