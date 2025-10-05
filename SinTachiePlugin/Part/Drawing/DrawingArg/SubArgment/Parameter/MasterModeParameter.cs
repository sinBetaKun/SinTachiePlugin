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
        [Display(Name = nameof(Texts.PartParam_Drawing_Value_X), ResourceType = typeof(Texts))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation X { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(Texts.PartParam_Drawing_Value_Y), ResourceType = typeof(Texts))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Y { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(Texts.PartParam_Drawing_Value_Z), ResourceType = typeof(Texts))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Z { get; } = new(0, -10000, 10000);

        [Display(Name = nameof(Texts.PartParam_Drawing_Value_Opacity), ResourceType = typeof(Texts))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Opacity { get; } = new(100, 0, 100);

        [Display(Name = nameof(Texts.PartParam_Drawing_Value_Zoom), ResourceType = typeof(Texts))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Zoom { get; } = new Animation(100, 0, 5000);

        [Display(Name = nameof(Texts.PartParam_Drawing_Value_Rotation), ResourceType = typeof(Texts))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Rotation { get; } = new Animation(0, -36000, 36000, 360);

        [Display(Name = nameof(Texts.PartParam_Drawing_Value_Inverse), ResourceType = typeof(Texts))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Inverse { get; } = new Animation(0, 0, 1);

        [Display(Name = nameof(Texts.PartParam_Drawing_Value_Blend), ResourceType = typeof(Texts))]
        [EnumComboBox]
        public Blend Blend { get; set; } = Blend.Normal;

        [Display(Name = nameof(Texts.PartParam_Drawing_Value_ZSort), ResourceType = typeof(Texts))]
        [EnumComboBox]
        public ZSortMode2 ZSort { get; set; } = ZSortMode2.InGroup;

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
                Inverse.CopyFrom(valuesParameter.Inverse);
                Blend = valuesParameter.Blend;
                ZSort = valuesParameter.ZSort;
            }
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [X, Y, Z, Opacity, Zoom, Rotation, Inverse];

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
