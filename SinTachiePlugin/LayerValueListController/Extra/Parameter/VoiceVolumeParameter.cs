using SinTachiePlugin.Parts;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.LayerValueListController.Extra.Parameter
{
    public class VoiceVolumeParameter : LayerValueExtraBase, INoVoiceValueParameter
    {
        [Display(Name = nameof(Resources.ParamName_NoVoiceValue), Description = nameof(Resources.ParamDesc_NoVoiceValue), ResourceType = typeof(Resources))]
        [AnimationSlider("F1", "%", -150, 150)]
        public Animation NoVoiceValue { get; } = new Animation(0, -10000, 10000);

        /// <summary>
        /// 制御モードが周期的往復/ループのとき、差分を指定する値を返す。
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="length"></param>
        /// <param name="fps"></param>
        /// <returns>0から1までのdouble</returns>
        public override double GetValue(FrameAndLength fl, int fps)
        {
            return fl.GetValue(NoVoiceValue, fps);
        }

        public VoiceVolumeParameter()
        {
        }

        public VoiceVolumeParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(LayerValueExtraBase? origin)
        {
            var origin2 = origin as VoiceVolumeParameter;
            if (origin2 == null) return;
            NoVoiceValue.CopyFrom(origin2.NoVoiceValue);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [NoVoiceValue];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new NoVoiceValueSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<NoVoiceValueSharedData>() is NoVoiceValueSharedData NVVParameter)
                NVVParameter.CopyTo(this);
        }
    }
}
