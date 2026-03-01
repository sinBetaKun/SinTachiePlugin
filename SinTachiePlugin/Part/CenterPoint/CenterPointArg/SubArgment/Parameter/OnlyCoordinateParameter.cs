using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Armgent.XY;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgment.Parameter
{
    internal class OnlyCoordinateParameter : CenterPointSubArgBase, IXYParameter
    {
        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_X), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation X { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Y), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Y { get; } = new(0, -10000, 10000);

        public OnlyCoordinateParameter()
        {
        }

        public OnlyCoordinateParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(CenterPointSubArgBase? origin)
        {
            if (origin is IXYParameter xyParameter)
            {
                X.CopyFrom(xyParameter.X);
                Y.CopyFrom(xyParameter.Y);
            }
        }

        public override CenterPointSubArgBase GetClone()
        {
            OnlyCoordinateParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [X, Y];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new XYSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<XYSharedData>() is XYSharedData xySharedData)
            {
                X.CopyFrom(xySharedData.X);
                Y.CopyFrom(xySharedData.Y);
            }
        }
    }
}
