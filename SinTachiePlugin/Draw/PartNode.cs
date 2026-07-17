using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.PartAnimation.Node.ImageFileNode;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using SinTachiePlugin.Properties;
using System.Collections.Immutable;
using System.IO;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw
{
    internal class PartNode : IDisposable
    {
        public string Tag { get; set; } = string.Empty;
        public ParamsOfPartNode Params { get; init; } = new();
        public PartNodeComparateResult PrevCompResult { get; private set; } = new();
        public ImageFileNodeManager? Ifnm { get; private set; }
        public ImmutableList<PartNode> Overall { get; set; } = [];
        public ImmutableList<PartNode> Ancestors { get; private set; } = []; // 祖先の果ては末尾
        public ImmutableList<PartNode> ClippingPath { get; private set; } = [];

        private readonly IGraphicsDevicesAndContext _devices;
        private readonly ID2D1Bitmap _empty;
        private ID2D1Image? _source;
        private bool _sourceChanged = false;

        public PartNode(IGraphicsDevicesAndContext devices)
        {
            _devices = devices;
            _empty = devices.DeviceContext.CreateEmptyBitmap();
        }

        public void SetSource(ID2D1Image source)
        {
            if (_source != source)
            {
                _source = source;
                _sourceChanged = true;
            }
            else
            {
                _sourceChanged = false;
            }
        }

        public void UpdateParams(List<PartValueAndFL> pvfls, TachieSourceDescription desc)
        {
            if (pvfls.Count == 0)
            {
                PrevCompResult = new(Exception: true);
                return;
            }

            Tag = pvfls.First().PartValue.ControlledParameters.Tag;
            Params.Update(pvfls, desc);
            PrevCompResult = Params.ComparateWithPrev();
            Params.SyncPrev();
        }

        public void UpdateFileNodeManager()
        {
            if (Params.SourceGroup.ThisLayerType == LayerType.Image && Path.Exists(Params.SourceGroup.FilePath))
            {
                ImmutableList<PartAnimationArg> l0 = Params.PartAnimationValueGroup.PartAnimationArgs;
                List<PartAnimationArg?> l1 = [];

                foreach (PartAnimationArg _0 in l0)
                {
                    while (l1.Count <= _0.Index)
                        l1.Add(null);

                    l1[_0.Index] = _0;
                }


                List<PartAnimationResultA> l2 = l1.Select(x =>
                {
                    if (x is null)
                        return PartAnimationResultA.FromVolume(1);

                    List<PartAnimationArg> l2 = [x];
                    PartAnimationArg _0 = x;

                    while (true)
                    {
                        if (_0.LinkOpeMode == PartAnimationLinkOpeMode.DontOverride)
                        {
                            SinTachieDialog.ShowError(new(TextResource.PartNode_Error_InvalidAnimationLinkMode));
                            throw new Exception(TextResource.PartNode_Error_InvalidAnimationLinkMode);
                        }

                        if (_0.LinkOpeMode == PartAnimationLinkOpeMode.DontLink)
                            break;

                        if (string.IsNullOrEmpty(_0.TargetPartTag)
                        || Overall.FirstOrDefault(y => y.Tag == _0.TargetPartTag) is not PartNode _1)
                            break;

                        if (!string.IsNullOrEmpty(_0.TargetPartTag)
                        || _1.Params.PartAnimationValueGroup.PartAnimationArgs
                        .FirstOrDefault(y => y.AnimationTag == _0.TargetAnimationTag) is not PartAnimationArg _2)
                            break;

                        l2.Add(_2);

                        if (l2.GroupBy(x => x, ReferenceEqualityComparer.Instance).Any(g => g.Count() >= 2))
                        {
                            if (x.IsVolume)
                            {
                                return PartAnimationResultA.FromVolume(x.Volume);
                            }
                            else
                            {
                                return PartAnimationResultA.FromText(x.Text);
                            }
                        }

                        _0 = _2;
                    }

                    l2.RemoveAt(0);
                    PartAnimationResultA _4 = x.IsVolume
                    ? PartAnimationResultA.FromVolume(x.Volume)
                    : PartAnimationResultA.FromText(x.Text);

                    foreach (PartAnimationArg _2 in l2)
                    {
                        if (_2.IsVolume)
                        {
                            if (_2.LinkOpeMode == PartAnimationLinkOpeMode.Add)
                                _4 = new(true, _4.Text, _4.Volume + _2.Volume);
                            else
                                _4 = new(true, _4.Text, _4.Volume * _2.Volume);
                        }
                        else
                        {
                            _4 = new(false, _2.Text, _4.Volume);
                        }
                    }

                    return _4;
                }).ToList();

                List<PartAnimationNormalizationMode> l3 = l1.Select(x =>
                {
                    if (x is null)
                        return PartAnimationNormalizationMode.Limit;
                    else
                        return x.NormalizationMode;
                }).ToList();

                if (!(Ifnm is not null && Ifnm.Root?.Path == Params.SourceGroup.FilePath))
                {
                    Ifnm = new(Params.SourceGroup.FilePath);
                }

                Ifnm.Update(l2, l3);
            }
            else
            {
                Ifnm = null;
            }
        }

        public void Dispose()
        {
            _empty.Dispose();

        }
    }
}
