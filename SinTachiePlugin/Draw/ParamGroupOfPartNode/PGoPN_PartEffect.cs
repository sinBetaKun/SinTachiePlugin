using SinTachiePlugin.Part;
using SinTachiePlugin.Part.PartEffect.PartEffectArg.Parameter;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_PartEffect
    {
        public List<PartEffectsAndFL> Effects = [];

        public void Reset()
        {
            Effects.Clear();
        }

        public void Update(List<PartValueAndFL> pvfls, TachieSourceDescription desc)
        {
            Effects.Clear();

            foreach (PartValueAndFL pvfl in pvfls.Reverse<PartValueAndFL>())
            {
                PartValue pv = pvfl.PartValue;
                FrameAndLength fl = pvfl.FL;

                if (pv.ControlledParameters.PartEffectArg is HavingSourcePartEffectParameter hspep)
                {
                    Effects.Add(new(hspep.Effects, fl));

                    if (hspep.PEOverrideMode == Enums.PartEffectOverrideMode.Override)
                        break;
                }
            }

            Effects.Reverse();
        }

        public void CopyTo(PGoPN_PartEffect pg)
        {
            pg.Effects = [.. Effects];
        }

        public PartNodeComparateResult Comparate(PGoPN_PartEffect pg)
        {
            return new(Value: pg.Effects.Select(x => x.Effects.Count).Sum() > 0 || Effects.Select(x => x.Effects.Count).Sum() > 0);
        }
    }
}
