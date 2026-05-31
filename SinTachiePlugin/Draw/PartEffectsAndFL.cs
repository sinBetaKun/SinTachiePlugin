using System.Collections.Immutable;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Draw
{
    internal class PartEffectsAndFL(ImmutableList<IVideoEffect> effects, FrameAndLength fl)
    {
        public ImmutableList<IVideoEffect> Effects { get; init; } = effects;

        public FrameAndLength FL { get; init; } = fl;
    }
}
