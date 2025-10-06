using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            SubArg = parameter.SubArg;
        }

        public void CopyTo(ISubArgParameter parameter)
        {
            parameter.SubArg = SubArg;
        }
    }
}
