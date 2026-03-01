using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Parameter;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.Argment.SubArg
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
