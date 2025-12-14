using SinTachiePlugin.Enums;
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

        public Animation Invert { get; }

        public Blend Blend { get; set; }

        public ZSortMode2 ZSort { get; set; }

        public Animation Priority { get; }

        public Animation Zoom_X { get; }

        public Animation Zoom_Y { get; }
    }
}
