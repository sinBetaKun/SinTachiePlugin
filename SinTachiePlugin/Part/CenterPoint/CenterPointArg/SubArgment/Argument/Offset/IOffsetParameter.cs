using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument.Argument.Offset
{
    internal interface IOffsetParameter
    {
        public Animation X { get; }

        public Animation Y { get; }

        public bool KeepPlace { get; set; }
    }
}
