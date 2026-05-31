using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgument;
using SinTachiePlugin.Part.CustomPoint.CustomPointArg.SubArgument.Parameter;

namespace SinTachiePlugin.Part.CustomPoint.CustomPointArg.Argument.SubArg
{
    internal class SubArgSharedData
    {
        public CustomPointSubArgBase SubArg { get; set; } = new NothingParameter();

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
