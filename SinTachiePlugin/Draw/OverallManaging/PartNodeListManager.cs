using SinTachiePlugin.Draw.Clipping;
using SinTachiePlugin.Draw.InverseKinematics;
using SinTachiePlugin.Draw.OverallManaging;
using SinTachiePlugin.Draw.PartAnimation;
using SinTachiePlugin.Part;
using System.Collections.Immutable;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Tachie;

namespace SinTachiePlugin.Draw.MultiPartBlend
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
        private readonly AudioSourceManager _asm = new();
        public ImmutableList<PartNode> Overall { get; private set; } = [];
        private readonly ClippingManager _clip;
        private readonly MultiPartBlender _blender;
        public ID2D1Image Output => _blender.Output;

        public PartNodeListManager(IGraphicsDevicesAndContext device)
        {
            _devices = device;
            _ism = new(device);
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

        public PartNodeComparateResult UpdateOverallParameter(IEnumerable<PartValueAndFL> pvfls, TimelineItemSourceDescription desc)
        {
            DummyCharactorParameter _0 = new();
            DummyTachieItemParameter _1 = new();
            TachieDescription _2 = new(_0, _1, []);
            TachieSourceDescription _3 = new(desc, _2, -1, YukkuriMovieMaker.Plugin.Voice.MouthShape.Silent);

            return UpdateOverallParameter(pvfls, _3);
        }

        public PartNodeComparateResult UpdateOverallParameter(IEnumerable<PartValueAndFL> pvfls, TachieSourceDescription desc)
        {
            List<string> s_l0 = [];
            List<List<PartValueAndFL>> pvfl_l_l0 = [];

            foreach (PartValueAndFL _0 in pvfls)
            {
                ControlledParametersOfPart _1 = _0.PartValue.ControlledParameters;

                if (_1.ThisLayerType == Enums.LayerType.Group)
                    continue;

                if (string.IsNullOrEmpty(_1.Tag))
                {
                    s_l0.Add(string.Empty);
                    pvfl_l_l0.Add([_0]);
                }
                else
                {
                    int n0 = s_l0.IndexOf(_1.Tag);

                    if (n0 < 0)
                    {
                        s_l0.Add(_1.Tag);
                        pvfl_l_l0.Add([_0]);
                    }
                    else
                    {
                        pvfl_l_l0[n0].Add(_0);
                    }
                }
            }

            List<PartNode> pn_l0 = [.. Overall];
            PartNode[] pn_a0 = new PartNode[s_l0.Count];

            for (int i = 0; i < s_l0.Count; i++)
            {
                if (pn_l0.FirstOrDefault(x => x.Tag == s_l0[i]) is PartNode _1)
                {
                    // ここ実は理論上、Tagが空文字列でもうまくいくんですよ
                    pn_l0.Remove(_1);
                    pn_a0[i] = _1;
                }
                else
                {
                    pn_a0[i] = new(_devices, _asm);
                }

                pn_a0[i].UpdateParams(pvfl_l_l0[i], desc);
            }

            Overall = [.. pn_a0];
            pn_l0.ForEach(x => x.Dispose());

            return PartNodeComparateResult.CalcSigma(pn_a0.Select(x => x.PrevCompResult));
        }

        public void UpdateOutput(TimelineItemSourceDescription desc, PartNodeComparateResult cmp)
        {
            PartAnimationManager.Generate(Overall);
            _ism.UpdateSourcesAssigning(Overall, desc);
            Overall.ForEach(x =>
            {
                UpdateAncestors(x);
                x.UpdatePreOutput(desc);
            });
            IKPathManager.OverwriteDrawDescriptionForIK(Overall);
            Overall.ForEach(x => x.UpdateOutputAndZ());
            _clip.Update(Overall);
            PartNodeClipper[] pnc_l0 = [.. _clip.Clippers.OrderBy(x => Overall.IndexOf(x.Destination))];
            IEnumerable<MultiPartBlendTupple> mpbt_e0 = pnc_l0.Select(x => new MultiPartBlendTupple(x.Destination, x.Output));

            if (cmp.Blend || pnc_l0.Any(x => x.OutputChanged))
            {
                _blender.Update(mpbt_e0, false);
                return;
            }

            if (cmp.Order || pnc_l0.Any(x => x.Destination.ZChanged))
            {
                _blender.Update(mpbt_e0, true);
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
