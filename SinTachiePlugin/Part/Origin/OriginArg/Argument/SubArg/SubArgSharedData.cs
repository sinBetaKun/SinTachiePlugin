using SinTachiePlugin.Part.Origin.OriginArg.SubArgument;
using SinTachiePlugin.Part.Origin.OriginArg.SubArgument.Parameter;

namespace SinTachiePlugin.Part.Origin.OriginArg.Argument.SubArg
{
    internal class SubArgSharedData
    {
        public OriginSubArgBase SubArg { get; set; } = new OriginNoneOptionParameter();

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
