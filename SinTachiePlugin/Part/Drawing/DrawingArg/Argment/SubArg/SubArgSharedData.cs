using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment.Parameter;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.Argment.SubArg
{
    internal class SubArgSharedData
    {
        public DrawingSubArgBase SubArg { get; set; } = new MasterModeParameter();

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
