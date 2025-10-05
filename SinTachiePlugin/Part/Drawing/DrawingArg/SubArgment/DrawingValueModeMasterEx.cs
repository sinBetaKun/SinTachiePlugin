using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment.Parameter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment
{
    internal static class DrawingValueModeMasterEx
    {
        public static DrawingSubArgBase Convert(this DrawingValueModeMaster mode, DrawingSubArgBase current)
        {
            var store = current.GetSharedData();
            DrawingSubArgBase param = mode switch
            {
                DrawingValueModeMaster.Override => new MasterModeParameter(store),
                DrawingValueModeMaster.Compose => new MasterModeParameter(store),
                DrawingValueModeMaster.Custom => new CustomModeParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
