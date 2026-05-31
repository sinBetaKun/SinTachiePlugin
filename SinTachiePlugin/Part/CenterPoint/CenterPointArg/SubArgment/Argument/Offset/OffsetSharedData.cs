using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument.Argument.Offset
{
    internal class OffsetSharedData
    {
        public Animation X { get; } = new(0, -10000, 10000);

        public Animation Y { get; } = new(0, -10000, 10000);

        public bool KeepPlace { get; set; } = false;

        public OffsetSharedData()
        {
        }

        public OffsetSharedData(IOffsetParameter parameter)
        {
            X.CopyFrom(parameter.X);
            Y.CopyFrom(parameter.Y);
            KeepPlace = parameter.KeepPlace;
        }

        public void CopyTo(IOffsetParameter parameter)
        {
            parameter.X.CopyFrom(X);
            parameter.Y.CopyFrom(Y);
            parameter.KeepPlace = KeepPlace;
        }
    }
}
