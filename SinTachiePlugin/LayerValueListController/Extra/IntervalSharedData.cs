using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;

namespace SinTachiePlugin.LayerValueListController.Extra
{
    internal class IntervalSharedData
    {
        public Animation Interval { get; } = new Animation(0, 0, 9999);

        public IntervalSharedData()
        {
        }

        public IntervalSharedData(IIntervalParameter parameter)
        {
            Interval.CopyFrom(parameter.Interval);
        }

        public void CopyTo(IIntervalParameter parameter)
        {
            parameter.Interval.CopyFrom(Interval);
        }
    }
}
