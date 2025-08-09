using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Theta
{
    internal class ThetaSharedData
    {
        public Animation Theta { get; } = new Animation(0, 0, 9999);

        public ThetaSharedData()
        {
        }

        public ThetaSharedData(IThetaParameter parameter)
        {
            Theta.CopyFrom(parameter.Theta);
        }

        public void CopyTo(IThetaParameter parameter)
        {
            parameter.Theta.CopyFrom(Theta);
        }
    }
}
