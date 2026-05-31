using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.PartEffect.PartEffectArg.Argument.Effects;
using SinTachiePlugin.Part.PartEffect.PartEffectArg.Argument.OverrideMode;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Plugin.Effects;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.PartEffect.PartEffectArg.Parameter
{
    internal class HavingSourcePartEffectParameter : PartEffectArgBase, IEffectsParameter, IPEOverrideModeParameter
    {
        [EnumComboBox]
        public PartEffectOverrideMode PEOverrideMode { get => _peOverrideMode; set => Set(ref _peOverrideMode, value); }
        private PartEffectOverrideMode _peOverrideMode;

        [VideoEffectSelector(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<IVideoEffect> Effects { get => _effects; set => Set(ref _effects, value); }
        private ImmutableList<IVideoEffect> _effects = [];

        public HavingSourcePartEffectParameter()
        {
        }

        public HavingSourcePartEffectParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(PartEffectArgBase? origin)
        {
            if (origin is IEffectsParameter effectsParameter)
                _effects = [.. effectsParameter.Effects];
        }

        public override PartEffectArgBase GetClone()
        {
            HavingSourcePartEffectParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [.. Effects];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new PEOverrideModeSharedData(this));
            store.Save(new EffectsSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<PEOverrideModeSharedData>() is  PEOverrideModeSharedData peOverrideModeSharedData)
                PEOverrideMode = peOverrideModeSharedData.PEOverrideMode;
            if (store.Load<EffectsSharedData>() is EffectsSharedData effectsSharedData)
                Effects = [.. effectsSharedData.Effects];
        }
    }
}
