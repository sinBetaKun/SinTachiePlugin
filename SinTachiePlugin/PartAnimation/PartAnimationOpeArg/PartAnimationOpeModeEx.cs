using SinTachiePlugin.Enums;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg
{
    internal static class PartAnimationOpeModeEx
    {
        public static PartAnimationOpeArgBase Convert(this PartAnimationOpeMode mode, PartAnimationOpeArgBase current)
        {
            var store = current.GetSharedData();
            PartAnimationOpeArgBase param = mode switch
            {
                PartAnimationOpeMode.Simple => new SimpleParameter(store),
                PartAnimationOpeMode.Sum => new SumParameter(store),
                PartAnimationOpeMode.Product => new ProductParameter(store),
                PartAnimationOpeMode.Sin => new SinParameter(store),
                PartAnimationOpeMode.VoiceVolume => new VoiceVolumeParameter(store),
                PartAnimationOpeMode.VoiceVolumePlus => new VoiceVolumePlusParameter(store),
                PartAnimationOpeMode.AIUEOMouth => new AIUEOMouthParameter(store),
                PartAnimationOpeMode.PeriodicShuttle => new PeriodicShuttleParameter(store),
                PartAnimationOpeMode.PeriodicLoop => new PeriodicLoopParameter(store),
                PartAnimationOpeMode.RandomShuttle => new RandomShuttleParameter(store),
                PartAnimationOpeMode.RandomLoop => new RandomLoopParameter(store),
                PartAnimationOpeMode.StringInput => new StringInputParameter(store),
                _ => throw new ArgumentOutOfRangeException(nameof(mode)),
            };

            if (param.GetType() != current.GetType())
                return param;

            return current;
        }
    }
}
