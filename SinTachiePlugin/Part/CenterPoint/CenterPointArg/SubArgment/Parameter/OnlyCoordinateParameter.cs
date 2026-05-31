using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument.Argument.Offset;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument.Parameter
{
    internal class OnlyCoordinateParameter : CenterPointSubArgBase, IOffsetParameter
    {
        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_X), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation X { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Y), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Y { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(TextResource.PartParam_CenterPoint_KeepPlace), ResourceType = typeof(TextResource))]
        public bool KeepPlace { get => _keepPlace; set => Set(ref _keepPlace, value); }
        private bool _keepPlace = false;

        public OnlyCoordinateParameter()
        {
        }

        public OnlyCoordinateParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(CenterPointSubArgBase? origin)
        {
            if (origin is IOffsetParameter offsetParameter)
            {
                X.CopyFrom(offsetParameter.X);
                Y.CopyFrom(offsetParameter.Y);
                KeepPlace = offsetParameter.KeepPlace;
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
            store.Save(new OffsetSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<OffsetSharedData>() is OffsetSharedData offsetSharedData)
            {
                X.CopyFrom(offsetSharedData.X);
                Y.CopyFrom(offsetSharedData.Y);
                KeepPlace = offsetSharedData.KeepPlace;
            }
        }
    }
}
