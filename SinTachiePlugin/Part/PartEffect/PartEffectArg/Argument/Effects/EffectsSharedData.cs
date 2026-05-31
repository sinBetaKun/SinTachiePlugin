using System.Collections.Immutable;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Part.PartEffect.PartEffectArg.Argument.Effects
{
    internal class EffectsSharedData
    {
        public ImmutableList<IVideoEffect> Effects { get; set; } = [];

        public EffectsSharedData()
        {
        }

        public EffectsSharedData(IEffectsParameter parameter)
        {
            Effects = [.. parameter.Effects];
        }

        public void CopyTo(IEffectsParameter parameter)
        {
            parameter.Effects = [.. Effects];
        }
    }
}
