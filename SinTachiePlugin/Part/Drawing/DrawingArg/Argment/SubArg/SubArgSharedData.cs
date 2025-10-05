using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            SubArg = parameter.SubArg;
        }

        public void CopyTo(ISubArgParameter parameter)
        {
            parameter.SubArg = SubArg;
        }
    }
}
