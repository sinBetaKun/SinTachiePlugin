using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment.Argment.Values;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment.Parameter
{
    internal class MasterModeParameter : DrawingSubArgBase, IValuesParameter
    {
        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_X), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation X { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Y), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Y { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Z), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Z { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Opacity), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation Opacity { get; } = new(100, 0, 100);

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Zoom), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "%", 0, 400)]
        public Animation Zoom { get; } = new Animation(100, 0, 5000);

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Rotation), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "°", -360, 360)]
        public Animation Rotation { get; } = new Animation(0, -36000, 36000, 360);

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Invert), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "", 0, 1)]
        public Animation Invert { get; } = new Animation(0, 0, 1);

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Blend), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public Blend Blend { get => _blend; set => Set(ref _blend, value); }
        private Blend _blend = Blend.Normal;

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_ZSort), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public ZSortMode2 ZSort { get => _zSort; set => Set(ref _zSort, value); }
        private ZSortMode2 _zSort = ZSortMode2.BasedOnPriority;

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Priority), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "", -10, 10)]
        public Animation Priority { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Zoom_X), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Zoom_X { get; } = new Animation(100, 0, 5000);

        [Display(Name = nameof(TextResource.PartParam_Drawing_Value_Zoom_Y), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Zoom_Y { get; } = new Animation(100, 0, 5000);

        public MasterModeParameter()
        {
        }

        public MasterModeParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(DrawingSubArgBase? origin)
        {
            if (origin is IValuesParameter valuesParameter)
            {
                X.CopyFrom(valuesParameter.X);
                Y.CopyFrom(valuesParameter.Y);
                Z.CopyFrom(valuesParameter.Z);
                Opacity.CopyFrom(valuesParameter.Opacity);
                Zoom.CopyFrom(valuesParameter.Zoom);
                Rotation.CopyFrom(valuesParameter.Rotation);
                Invert.CopyFrom(valuesParameter.Invert);
                Blend = valuesParameter.Blend;
                ZSort = valuesParameter.ZSort;
                Priority.CopyFrom(valuesParameter.Priority);
                Zoom_X.CopyFrom(valuesParameter.Zoom_X);
                Zoom_Y.CopyFrom(valuesParameter.Zoom_Y);
            }
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [X, Y, Z, Opacity, Zoom, Rotation, Invert, Priority];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new ValuesSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<ValuesSharedData>() is ValuesSharedData valuesSharedData)
                valuesSharedData.CopyTo(this);
        }
    }
}
