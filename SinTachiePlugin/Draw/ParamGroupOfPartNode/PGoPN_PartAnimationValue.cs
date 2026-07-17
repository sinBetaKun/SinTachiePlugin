using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.PartAnimationValue;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using SinTachiePlugin.PartAnimation;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Parameter;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using System.Collections.Immutable;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_PartAnimationValue
    {
        public ImmutableList<PartAnimationArg> PartAnimationArgs { get; private set; } = [];

        public void Reset()
        {
            PartAnimationArgs = [];
        }

        public void Update(List<PartValueAndFL> pvfls, TachieSourceDescription desc)
        {
            bool isMulti = false;

            for (int i = pvfls.Count - 1; i >= 0; i--)
            {
                ImageFileParameter ip = (ImageFileParameter)pvfls[i].PartValue.ControlledParameters.SourceSelectArg;

                if (ip.PartAnimationValueMode == PartAnimationValueMode.Multi)
                {
                    isMulti = true;
                    break;
                }
            }

            if (isMulti)
            {
                List<PartAnimationArg> partAnimationArgs = [];

                foreach (PartValueAndFL pvfl in pvfls)
                {
                    ControlledParametersOfPart cpp = pvfl.PartValue.ControlledParameters;

                    if (cpp.SourceSelectArg is IPartAnimationValueParameter parameter)
                    {
                        if (parameter.PartAnimationValueArg is MultiValuesParameter multiValuesParameter)
                        {
                            foreach (PartAnimationValueExtra pav in multiValuesParameter.PartAnimationValues)
                            {
                                if (partAnimationArgs.FirstOrDefault(paa => paa.AnimationTag == pav.AnimationTag) is PartAnimationArg paa1)
                                {
                                    partAnimationArgs.Remove(paa1);

                                    if (pav.Index > 0)
                                        paa1.Index = pav.Index;

                                    paa1.LinkOpeMode = pav.LinkOpeMode;
                                    paa1.NormalizationMode = pav.NormalizationMode;

                                    PartAnimationResultA result = pav.AnmOpeArgments.GetResult(desc);

                                    if (result.IsVolume)
                                    {
                                        paa1.IsVolume = true;
                                        paa1.Volume = result.Volume;
                                    }
                                    else
                                    {
                                        paa1.IsVolume = false;
                                        paa1.Text = result.Text;
                                    }

                                    switch (pav.LinkOpeMode)
                                    {
                                        case PartAnimationLinkOpeMode.DontLink:
                                            paa1.TargetPartTag = string.Empty;
                                            paa1.TargetAnimationTag = string.Empty;
                                            break;

                                        case PartAnimationLinkOpeMode.Add or PartAnimationLinkOpeMode.Multiply:
                                            DoLinkParameter lp = (DoLinkParameter)pav.LinkArgments;

                                            if (!string.IsNullOrEmpty(lp.TargetPartTag))
                                                paa1.TargetPartTag = lp.TargetPartTag;

                                            if (!string.IsNullOrEmpty(lp.TargetAnimationTag))
                                                paa1.TargetAnimationTag = lp.TargetAnimationTag;

                                            break;
                                    }

                                    partAnimationArgs.Add(paa1);
                                }
                                else
                                {
                                    PartAnimationArg paa2 = new()
                                    {
                                        AnimationTag = pav.AnimationTag,
                                    };

                                    if (pav.Index > 0)
                                        paa2.Index = pav.Index;

                                    PartAnimationResultA result = pav.AnmOpeArgments.GetResult(desc);

                                    if (result.IsVolume)
                                    {
                                        paa2.IsVolume = true;
                                        paa2.Volume = result.Volume;
                                    }
                                    else
                                    {
                                        paa2.IsVolume = false;
                                        paa2.Text = result.Text;
                                    }

                                    if (pav.NormalizationMode != PartAnimationNormalizationMode.DontOverride)
                                        paa2.NormalizationMode = pav.NormalizationMode;

                                    if (pav.LinkOpeMode != PartAnimationLinkOpeMode.DontOverride)
                                        paa2.LinkOpeMode = pav.LinkOpeMode;


                                    if (pav.LinkOpeMode == PartAnimationLinkOpeMode.Add || pav.LinkOpeMode == PartAnimationLinkOpeMode.Multiply)
                                    {
                                        DoLinkParameter lp = (DoLinkParameter)pav.LinkArgments;
                                        paa2.TargetPartTag = lp.TargetPartTag;
                                        paa2.TargetAnimationTag = lp.TargetAnimationTag;
                                    }

                                    partAnimationArgs.Add(paa2);
                                }
                            }
                        }
                    }
                }

                PartAnimationArgs = [.. partAnimationArgs];
            }
            else
            {
                PartAnimationArg partAnimationArg = new();

                foreach (PartValueAndFL pvfl in pvfls)
                {
                    ControlledParametersOfPart cp = pvfl.PartValue.ControlledParameters;

                    if (cp.SourceSelectArg is IPartAnimationValueParameter parameter)
                    {
                        if (parameter.PartAnimationValueArg is SingleValueParameter singleValuesParameter)
                        {
                            PartAnimationValue pav = singleValuesParameter.PartAnimationValue;
                            partAnimationArg.NormalizationMode = pav.NormalizationMode;
                            PartAnimationResultA result = pav.AnmOpeArgments.GetResult(desc);

                            if (result.IsVolume)
                            {
                                partAnimationArg.IsVolume = true;
                                partAnimationArg.Volume = result.Volume;
                            }
                            else
                            {
                                partAnimationArg.IsVolume = false;
                                partAnimationArg.Text = result.Text;
                            }
                        }
                    }
                }

                PartAnimationArgs = [partAnimationArg];
            }
        }

        public void CopyTo(PGoPN_PartAnimationValue pg)
        {
            pg.PartAnimationArgs = [.. PartAnimationArgs.Select(paa => paa.GetClone())];
        }

        public PartNodeComparateResult Comparate(PGoPN_PartAnimationValue pg)
        {
            bool b0;

            if (pg.PartAnimationArgs.Count != PartAnimationArgs.Count)
            {
                b0 = true;
            }
            else
            {
                for (int i = 0; i < PartAnimationArgs.Count; i++)
                {
                    if (PartAnimationArgs[i].IsEqual(pg.PartAnimationArgs[i]))
                    {
                        b0 = true;
                        break;
                    }
                }

                b0 = false;
            }

            return new(Value: b0);
        }
    }
}
