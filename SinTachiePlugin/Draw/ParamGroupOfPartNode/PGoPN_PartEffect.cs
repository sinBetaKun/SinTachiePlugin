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
            bool isFirst = true;

            foreach (PartValueAndFL pvfl in pvfls.Reverse<PartValueAndFL>())
            {
                PartValue pv = pvfl.PartValue;
                FrameAndLength fl = pvfl.FL;

                if (pv.ControlledParameters.PartEffectArg is HavingSourcePartEffectParameter hspep)
                {
                    if (isFirst)
                    {
                        Effects.Add(new(hspep.Effects, fl));
                        isFirst = false;
                    }
                    else
                    {
                        Effects.Insert(0, new(hspep.Effects, fl));
                    }

                    if (hspep.PEOverrideMode == Enums.PartEffectOverrideMode.Override)
                        return;
                }
            }
        }

        public void CopyTo(PGoPN_PartEffect pg)
        {
            pg.Effects = [.. Effects];
        }

        public PartNodeComparateResult Comparate(PGoPN_PartEffect pg)
        {
            return new(Value: pg.Effects.Count > 0 || Effects.Count > 0);
        }
    }
}
