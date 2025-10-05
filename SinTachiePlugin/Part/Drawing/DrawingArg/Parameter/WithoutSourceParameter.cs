using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.Parameter
{
    internal class WithoutSourceParameter : DrawingArgBase
    {
        public WithoutSourceParameter()
        {
        }

        public WithoutSourceParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(DrawingArgBase? origin)
        {
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
        }
    }
}
