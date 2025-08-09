using SinTachiePlugin.Parts;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.LayerValueListController.Extra.Parameter
{
    [Obsolete]
    public class VoiceVolumeParameter : LayerValueExtraBase, INoVoiceValueParameter
    {
        public Animation NoVoiceValue { get; } = new Animation(0, -10000, 10000);

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
