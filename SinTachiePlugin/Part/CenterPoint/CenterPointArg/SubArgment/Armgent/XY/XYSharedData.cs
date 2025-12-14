using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Armgent.XY
{
    internal class XYSharedData
    {
        public Animation X { get; } = new(0, -10000, 10000);

        public Animation Y { get; } = new(0, -10000, 10000);

        public XYSharedData()
        {
        }

        public XYSharedData(IXYParameter parameter)
        {
            X.CopyFrom(parameter.X);
            Y.CopyFrom(parameter.Y);
        }

        public void CopyTo(IXYParameter parameter)
        {
            parameter.X.CopyFrom(X);
            parameter.Y.CopyFrom(Y);
        }
    }
}
