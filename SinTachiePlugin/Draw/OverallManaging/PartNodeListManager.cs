using SinTachiePlugin.Draw.Clipping;
using SinTachiePlugin.Draw.InverseKinematics;
using SinTachiePlugin.Draw.MultiPartBlend;
using SinTachiePlugin.Draw.PartAnimation;
using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using System.Collections.Immutable;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Tachie;

namespace SinTachiePlugin.Draw.OverallManaging
{
    internal class PartNodeListManager : IDisposable
    {
        public class DummyCharactorParameter() : TachieCharacterParameterBase
        {
        }
        
        public class DummyTachieItemParameter() : TachieItemParameterBase
        {
            protected override IEnumerable<IAnimatable> GetAnimatables() => [];
        }

        private readonly IGraphicsDevicesAndContext _devices;
        private readonly ImageSourceManager _ism;
        private readonly AudioSourceManager _asm;
        public ImmutableList<PartNode> Overall { get; private set; } = [];
        private readonly ClippingManager _clip;
        private readonly MultiPartsBlender _blender;
        public ID2D1Image Output => _blender.Output;

        public PartNodeListManager(IGraphicsDevicesAndContext device)
        {
            _devices = device;
            _ism = new(device);
            _asm = new();
            _clip = new(device);
            _blender = new(device);
        }

        public void Reset()
        {
            _blender.Reset();
            _clip.Reset();
            Overall.ForEach(x => x.Dispose());
            Overall = [];
            _ism.ClearSources();
        }

        public void Update(IEnumerable<PartValueAndFL> pvfls, TimelineItemSourceDescription desc)
        {
            DummyCharactorParameter _0 = new();
            DummyTachieItemParameter _1 = new();
            TachieDescription _2 = new(_0, _1, []);
            TachieSourceDescription _3 = new(desc, _2, -1, YukkuriMovieMaker.Plugin.Voice.MouthShape.Silent);
            Update(pvfls, _3);
        }

        public void Update(IEnumerable<PartValueAndFL> pvfls, TachieSourceDescription desc)
        {
            List<string> s_l0 = [];
            List<List<PartValueAndFL>> pvfl_l_l0 = [];

            foreach (PartValueAndFL pvfl_0 in pvfls)
            {
                ControlledParametersOfPart pvfl_1 = pvfl_0.PartValue.ControlledParameters;

                if (pvfl_1.ThisLayerType == LayerType.Group)
                    continue;

                if (string.IsNullOrEmpty(pvfl_1.Tag))
                {
                    s_l0.Add(string.Empty);
                    pvfl_l_l0.Add([pvfl_0]);
                }
                else
                {
                    int i_0 = s_l0.IndexOf(pvfl_1.Tag);

                    if (i_0 < 0)
                    {
                        s_l0.Add(pvfl_1.Tag);
                        pvfl_l_l0.Add([pvfl_0]);
                    }
                    else
                    {
                        pvfl_l_l0[i_0].Add(pvfl_0);
                    }
                }
            }

            List<PartNode> pn_l0 = [.. Overall];
            PartNode[] pn_a0 = new PartNode[s_l0.Count];
            bool b_0 = false;
            _asm.ResetAsseted();

            for (int i_0 = 0; i_0 < s_l0.Count; i_0++)
            {
                if (pn_l0.FirstOrDefault(x => x.Tag == s_l0[i_0]) is PartNode pn_1)
                {
                    // ここ実は理論上、Tagが空文字列でもうまくいくんですよ
                    pn_l0.Remove(pn_1);
                    pn_a0[i_0] = pn_1;
                }
                else
                {
                    pn_a0[i_0] = new(_devices, _asm);
                    b_0 = true;
                }

                pn_a0[i_0].UpdateParams(pvfl_l_l0[i_0], desc);
            }

            _asm.DisposeVacancySource();

            if (pn_l0.Count > 0)
            {
                pn_l0.ForEach(x => x.Dispose());
                b_0 = true;
            }

            bool b_1 = !Overall.SequenceEqual(pn_a0);

            if (b_1)
            {
                Overall = [.. pn_a0];
            }

            PartNodeComparateResult cmp_0;

            if (b_0)
                cmp_0 = PartNodeComparateResult.AllChanged();
            else
                cmp_0 = PartNodeComparateResult.CalcSigma(pn_a0.Select(x => x.PrevCompResult))
                | new PartNodeComparateResult(Order: b_1);

            if (cmp_0.AnyParamChanged())
                UpdateOutput(desc, cmp_0);
        }

        private void UpdateOutput(TimelineItemSourceDescription desc, PartNodeComparateResult cmp)
        {
            if (cmp.Value)
            {
                PartAnimationManager.Generate(Overall);
                _ism.UpdateSourcesAssigning(Overall, desc);
            }

            if (cmp.PartNetwork)
                Overall.ForEach(UpdateAncestors);

            if (cmp.Value || cmp.PartNetwork)
            {
                Overall.ForEach(x => x.UpdatePreOutput(desc));
                IKPathManager.OverwriteDrawDescriptionForIK(Overall);
                Overall.ForEach(x => x.UpdateOutputAndZ());
                _clip.Update(Overall);
            }

            PartNodeClipper[] pnc_l0 = [.. _clip.Clippers.OrderBy(x => Overall.IndexOf(x.Destination))];
            IEnumerable<MultiPartBlendTuple> mpbt_e0 = pnc_l0.Select(x => new MultiPartBlendTuple(x.Destination, x.Output));

            if (cmp.Order || pnc_l0.Any(x => x.Destination.ZChanged))
            {
                _blender.Update(mpbt_e0, true);
                return;
            }

            if (cmp.Blend || pnc_l0.Any(x => x.OutputChanged))
            {
                _blender.Update(mpbt_e0, false);
                return;
            }
        }

        private void UpdateAncestors(PartNode pn)
        {
            List<PartNode> pn_l0 = [];
            PartNode pn_0 = pn;

            while (true)
            {
                string s_4prt_0 = pn_0.Params.LayerInformationGroup.Parent;

                if (string.IsNullOrEmpty(pn_0.Tag) || Overall.FirstOrDefault(x => x.Tag == s_4prt_0) is not PartNode pn_1)
                {
                    break;
                }

                if (pn_l0.Contains(pn_1))
                {
                    pn.Ancestors = [];
                    return;
                }

                pn_l0.Add(pn_1);
                pn_0 = pn_1;
            }

            pn.Ancestors = [.. pn_l0];
        }

        public void Dispose()
        {
            _ism.Dispose();
            _asm.Dispose();
            Overall.ForEach(x => x.Dispose());
            _clip.Dispose();
            _blender.Dispose();
        }
    }
}
