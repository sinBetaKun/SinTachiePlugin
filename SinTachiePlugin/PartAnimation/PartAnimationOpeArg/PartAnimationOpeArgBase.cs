using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinTachiePlugin.LayerValueListController.Extra;
using SinTachiePlugin.Parts;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg
{
    internal abstract class PartAnimationOpeArgBase : SharedParameterBase
    {
        public PartAnimationOpeArgBase()
        {
        }

        public PartAnimationOpeArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract double GetValue(FrameAndLength fl, int fps, double voiceVolume);

        public abstract void CopyFrom(PartAnimationOpeArgBase? origin);
    }
}
