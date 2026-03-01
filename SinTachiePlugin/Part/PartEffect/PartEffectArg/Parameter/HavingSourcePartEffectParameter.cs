using SinTachiePlugin.Part.PartEffect.PartEffectArg.Argment.Effects;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Plugin.Effects;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.PartEffect.PartEffectArg.Parameter
{
    internal class HavingSourcePartEffectParameter : PartEffectArgBase, IEffectsParameter
    {
        [VideoEffectSelector(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<IVideoEffect> Effects { get => effects; set => Set(ref effects, value); }
        ImmutableList<IVideoEffect> effects = [];

        public HavingSourcePartEffectParameter()
        {
        }

        public HavingSourcePartEffectParameter(SharedDataStore? store = null) : base(store)
        {
        }

        public override void CopyFrom(PartEffectArgBase? origin)
        {
            if (origin is IEffectsParameter effectsParameter)
                effects = [.. effectsParameter.Effects];
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
            store.Save(new EffectsSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<EffectsSharedData>() is EffectsSharedData effectsSharedData)
                Effects = [.. effectsSharedData.Effects];
        }
    }
}
