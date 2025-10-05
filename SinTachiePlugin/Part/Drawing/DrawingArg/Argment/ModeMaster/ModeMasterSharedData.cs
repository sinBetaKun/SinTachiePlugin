using SinTachiePlugin.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.Argment.ModeMaster
{
    internal class ModeMasterSharedData
    {
        public DrawingValueModeMaster ModeMaster { get; set; } = DrawingValueModeMaster.Override;

        public ModeMasterSharedData()
        {
        }

        public ModeMasterSharedData(IModeMasterParameter parameter)
        {
            ModeMaster = parameter.ModeMaster;
        }

        public void CopyTo(IModeMasterParameter parameter)
        {
            parameter.ModeMaster = ModeMaster;
        }
    }
}
