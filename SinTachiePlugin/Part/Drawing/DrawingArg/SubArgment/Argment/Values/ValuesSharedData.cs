using SinTachiePlugin.Enums;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment.Argment.Values
{
    internal class ValuesSharedData
    {
        public Animation X { get; set; } = new(0, -10000, 10000);

        public Animation Y { get; set; } = new(0, -10000, 10000);

        public Animation Z { get; set; } = new(0, -10000, 10000);

        public Animation Opacity { get; set; } = new(100, 0, 100);

        public Animation Zoom { get; set; } = new Animation(100, 0, 5000);

        public Animation Rotation { get; set; } = new Animation(0, -36000, 36000, 360);

        public Animation Inverse { get; set; } = new Animation(0, 0, 1);

        public Blend Blend { get; set; } = Blend.Normal;

        public ZSortMode2 ZSort { get; set; } = ZSortMode2.InGroup;

        public ValuesSharedData()
        {
        }

        public ValuesSharedData(IValuesParameter parameter)
        {
            X.CopyFrom(parameter.X);
            Y.CopyFrom(parameter.Y);
            Z.CopyFrom(parameter.Z);
            Opacity.CopyFrom(parameter.Opacity);
            Zoom.CopyFrom(parameter.Zoom);
            Rotation.CopyFrom(parameter.Rotation);
            Inverse.CopyFrom(parameter.Inverse);
            Blend = parameter.Blend;
            ZSort = parameter.ZSort;
        }

        public void CopyTo(IValuesParameter parameter)
        {
            parameter.X.CopyFrom(X);
            parameter.Y.CopyFrom(Y);
            parameter.Z.CopyFrom(Z);
            parameter.Opacity.CopyFrom(Opacity);
            parameter.Zoom.CopyFrom(Zoom);
            parameter.Rotation.CopyFrom(Rotation);
            parameter.Inverse.CopyFrom(Inverse);
            parameter.Blend = Blend;
            parameter.ZSort = ZSort;
        }
    }
}
