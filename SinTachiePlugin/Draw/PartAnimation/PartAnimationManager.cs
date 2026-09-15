using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using SinTachiePlugin.Properties;
using System.Collections.Immutable;
using System.IO;

namespace SinTachiePlugin.Draw.PartAnimation
{
    internal static class PartAnimationManager
    {
        public static void Generate(IEnumerable<PartNode> pns)
        {
            PartNode[] overall = [.. pns];

            foreach (PartNode pn0 in overall)
            {
                if (pn0.Params.SourceGroup.ThisLayerType == LayerType.Image && File.Exists(pn0.Params.SourceGroup.FilePath))
                {
                    ImmutableList<PartAnimationArg> l0 = pn0.Params.PartAnimationValueGroup.PartAnimationArgs;
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
                            || overall.FirstOrDefault(y => y.Tag == _0.TargetPartTag) is not PartNode _1)
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

                    pn0.UpdateIfnm(l2, l3);
                }
                else
                {
                    pn0.ClearIfnm();
                }
            }
        }
    }
}
