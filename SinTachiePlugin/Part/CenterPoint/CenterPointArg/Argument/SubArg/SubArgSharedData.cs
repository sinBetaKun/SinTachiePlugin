using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument.Parameter;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.Argument.SubArg
{
    internal class SubArgSharedData
    {
        public CenterPointSubArgBase SubArg { get; set; } = new NoOptionParameter();

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
