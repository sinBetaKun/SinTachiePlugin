using SinTachiePlugin.Part;
using System.Collections.Immutable;
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
        private readonly SourceManager _sm;
        public ImmutableList<PartNode> Overall { get; private set; } = [];

        public PartNodeListManager(IGraphicsDevicesAndContext device)
        {
            _devices = device;
             _sm = new(device);
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
            List<string> l0 = [];
            List<List<PartValueAndFL>> l1 = [];

            foreach (PartValueAndFL _0 in pvfls)
            {
                ControlledParametersOfPart _1 = _0.PartValue.ControlledParameters;

                if (_1.ThisLayerType == Enums.LayerType.Group)
                    continue;

                if (string.IsNullOrEmpty(_1.Tag))
                {
                    l0.Add(string.Empty);
                    l1.Add([_0]);
                }
                else
                {
                    int n0 = l0.IndexOf(_1.Tag);

                    if (n0 < 0)
                    {
                        l0.Add(_1.Tag);
                        l1.Add([_0]);
                    }
                    else
                    {
                        l1[n0].Add(_0);
                    }
                }
            }

            List<PartNode> l2 = [.. Overall];
            PartNode[] a0 = new PartNode[l0.Count];

            for (int i = 0; i < l0.Count; i++)
            {
                if (l2.FirstOrDefault(x => x.Tag == l0[i]) is PartNode _1)
                {
                    // ここ実は理論上、Tagが空文字列でもうまくいくんですよ
                    l2.Remove(_1);
                    a0[i] = _1;
                }
                else
                {
                    a0[i] = new(_devices);
                }

                a0[i].UpdateParams(l1[i], desc);
            }

            Overall = [.. a0];
            l2.ForEach(x => x.Dispose());

            return PartNodeComparateResult.CalcSigma(a0.Select(x => x.PrevCompResult));
        }

        public void SourceUpdate(TimelineItemSourceDescription desc)
        {
            Overall.ForEach(x => x.UpdateFileNodeManager());
            _sm.UpdateSourcesAssigning(Overall, desc);
        }


        public void Dispose()
        {
            _sm.Dispose();
            Overall.ForEach(x => x.Dispose());
        }
    }
}
