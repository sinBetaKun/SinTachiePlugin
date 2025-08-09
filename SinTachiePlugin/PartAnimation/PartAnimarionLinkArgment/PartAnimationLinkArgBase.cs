using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment
{
    internal abstract class PartAnimationLinkArgBase : SharedParameterBase
    {
        public PartAnimationLinkArgBase()
        {
        }

        public PartAnimationLinkArgBase(SharedDataStore? store = null) : base(store)
        {
        }

        public abstract void CopyTo(PartAnimationLinkArgBase? origin);
    }
}
