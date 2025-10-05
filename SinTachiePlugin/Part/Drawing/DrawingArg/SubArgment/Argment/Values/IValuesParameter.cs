using SinTachiePlugin.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment.Argment.Values
{
    internal interface IValuesParameter
    {
        public Animation X { get; }
        
        public Animation Y { get; }
        
        public Animation Z { get; }

        public Animation Opacity { get; }

        public Animation Zoom { get; }

        public Animation Rotation { get; }

        public Animation Inverse { get; }

        public Blend Blend { get; set; }

        public ZSortMode2 ZSort { get; set; }
    }
}
