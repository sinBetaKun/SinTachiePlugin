using System.Collections.Immutable;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Draw
{
    internal class PartEffectsAndFL(IEnumerable<IVideoEffect> effects, FrameAndLength fl)
    {
        public ImmutableList<IVideoEffect> Effects { get; init; } = [.. effects.Where(x => x.IsEnabled)];

        public FrameAndLength FL { get; init; } = fl;
    }
}
