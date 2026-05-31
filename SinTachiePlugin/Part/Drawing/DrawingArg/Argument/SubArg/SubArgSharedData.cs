using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument.Parameter;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.Argument.SubArg
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
