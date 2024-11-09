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
    internal class StartSharedData
    {
        public Animation Start { get; } = new Animation(0, 0, 9999);

        public StartSharedData()
        {
        }

        public StartSharedData(IStartParameter parameter)
        {
            Start.CopyFrom(parameter.Start);
        }

        public void CopyTo(IStartParameter parameter)
        {
            parameter.Start.CopyFrom(Start);
        }
    }
}
