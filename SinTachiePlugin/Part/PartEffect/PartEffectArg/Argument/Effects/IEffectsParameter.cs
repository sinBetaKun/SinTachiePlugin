using System.Collections.Immutable;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Part.PartEffect.PartEffectArg.Argument.Effects
{
    internal interface IEffectsParameter
    {
        public ImmutableList<IVideoEffect> Effects { get; set; }
    }
}
