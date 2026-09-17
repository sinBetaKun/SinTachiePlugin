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
        /// <summary>
        /// 描画順序リスト上の全パーツの全ｱﾆﾒｰｼｮﾝﾚｲﾔｰの出力を導出する。
        /// <br/>
        /// 別パーツのｱﾆﾒｰｼｮﾝﾚｲﾔｰと連動するｱﾆﾒｰｼｮﾝﾚｲﾔｰもあるのでその処理も行う。
        /// </summary>
        /// <param name="pns">描画順序リスト全体</param>
        /// <exception cref="Exception"></exception>
        public static void Generate(IEnumerable<PartNode> pns)
        {
            // 描画順序リスト全体
            PartNode[] overall = [.. pns];

            foreach (PartNode pn_0 in overall)
            {
                if (pn_0.Params.SourceGroup.ThisLayerType == LayerType.Image && File.Exists(pn_0.Params.SourceGroup.FilePath))
                {
                    ImmutableList<PartAnimationArg> paa_l0 = pn_0.Params.PartAnimationValueGroup.PartAnimationArgs;
                    List<PartAnimationArg?> paa_l1 = [];

                    foreach (PartAnimationArg paa_0 in paa_l0)
                    {
                        // 値が指定されず飛ばされたｱﾆﾒｰｼｮﾝﾚｲﾔｰ次元軸の情報を埋める。
                        while (paa_l1.Count <= paa_0.Index)
                            paa_l1.Add(null);

                        paa_l1[paa_0.Index] = paa_0;
                    }

                    // 
                    List<PartAnimationResultA> parA_l2 = [];
                    List<PartAnimationArg> paa_l2 = [];

                    foreach (PartAnimationArg? paa_0 in paa_l1)
                    {
                        if (paa_0 is null)
                        {
                            parA_l2.Add(PartAnimationResultA.FromVolume(1.0));
                            continue;
                        }

                        paa_l2.Clear();
                        paa_l2.Add(paa_0);
                        PartAnimationArg paa_1 = paa_0;

                        // LinkOpeModeの処理（他のパーツのｱﾆﾒｰｼｮﾝﾚｲﾔｰとの連動を処理）
                        while (true)
                        {
                            // 演算不可能な状態。そもそもこの状態になることは理論上無い。
                            if (paa_1.LinkOpeMode == PartAnimationLinkOpeMode.DontOverride)
                            {
                                SinTachieDialog.ShowError(new(TextResource.PartNode_Error_InvalidAnimationLinkMode));
                                throw new Exception(TextResource.PartNode_Error_InvalidAnimationLinkMode);
                            }

                            // 連動しない。（独立）
                            if (paa_1.LinkOpeMode == PartAnimationLinkOpeMode.DontLink)
                                break;

                            // 連動の参照先パーツが指定されていない、又は該当する参照先が存在しない。
                            if (string.IsNullOrEmpty(paa_1.TargetPartTag)
                                || overall.FirstOrDefault(y => y.Tag == paa_1.TargetPartTag) is not PartNode pn_1)
                                break;

                            // 連動の参照先ｱﾆﾒｰｼｮﾝﾚｲﾔｰが指定されていない、又は該当する参照先が存在しない。
                            if (string.IsNullOrEmpty(paa_1.TargetAnimationTag)
                                || pn_1.Params.PartAnimationValueGroup.PartAnimationArgs
                                    .FirstOrDefault(y => y.AnimationTag == paa_1.TargetAnimationTag) is not PartAnimationArg paa_2)
                                break;

                            paa_l2.Add(paa_2);

                            if (paa_l2.GroupBy(x => x, ReferenceEqualityComparer.Instance).Any(g => g.Count() >= 2))
                            {
                                if (paa_0.IsVolume)
                                {
                                    parA_l2.Add(PartAnimationResultA.FromVolume(paa_0.Volume));
                                    continue;
                                }
                                else
                                {
                                    parA_l2.Add(PartAnimationResultA.FromText(paa_0.Text));
                                    continue;
                                }
                            }

                            paa_1 = paa_2;
                        }

                        paa_l2.RemoveAt(0);
                        PartAnimationResultA parA_3 = paa_0.IsVolume
                        ? PartAnimationResultA.FromVolume(paa_0.Volume)
                        : PartAnimationResultA.FromText(paa_0.Text);

                        foreach (PartAnimationArg paa_2 in paa_l2)
                        {
                            if (paa_2.IsVolume)
                            {
                                if (paa_2.LinkOpeMode == PartAnimationLinkOpeMode.Add)
                                    parA_3 = new(true, parA_3.Text, parA_3.Volume + paa_2.Volume);
                                else
                                    parA_3 = new(true, parA_3.Text, parA_3.Volume * paa_2.Volume);
                            }
                            else
                            {
                                parA_3 = new(false, paa_2.Text, parA_3.Volume);
                            }
                        }

                        parA_l2.Add(parA_3);
                        continue;
                    }

                    List<PartAnimationNormalizationMode> panm_l0 = [.. paa_l1.Select(x =>
                    {
                        if (x is null)
                            return PartAnimationNormalizationMode.Limit;
                        else
                            return x.NormalizationMode;
                    })];

                    pn_0.UpdateIfnm(parA_l2, panm_l0);
                }
                else
                {
                    pn_0.ClearIfnm();
                }
            }
        }
    }
}
