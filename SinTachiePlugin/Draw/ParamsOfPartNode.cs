using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.Drawing.DrawingArg.Parameter;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgment.Parameter;
using SinTachiePlugin.Part.LayerInformation.ClippingArg.Argment.PartToClipTo;
using SinTachiePlugin.Part.LayerInformation.PartAnimationValueArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Clip;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.Parent;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argment.PartAnimationValue;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using SinTachiePlugin.PartAnimation;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Parameter;
using System.Collections.Immutable;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Draw
{
    internal class ParamsOfPartNode : IDisposable
    {
        public bool Hide { get; private set; } = false;
        public LayerType ThisLayerType { get; private set; } = LayerType.Image;
        public bool IsEmpty { get; private set; }
        public string FilePath { get; private set; } = string.Empty;
        public ImmutableList<string> EnableLayers { get; private set; } = [];
        public Guid SceneId { get; private set; } = Guid.Empty;
        public TimeSpan Time { get; private set; }
        public bool LoopPlayback { get; private set; }
        public ImmutableList<PartAnimationArg> PartAnimationArgs { get; private set; } = [];
        public ClippingMode ClippingMode { get; private set; }
        public string TagToClipTo { get; private set; } = string.Empty;
        public string Parent { get; private set; } = string.Empty;
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public double Opacity { get; private set; }
        public double Zoom_X { get; private set; }
        public double Zoom_Y { get; private set; }
        public double Rotate { get; private set; }
        public bool Invert { get; private set; }
        public Blend Blend { get; private set; }
        public ZSortMode2 ZSort { get; private set; }
        public double Priority { get; private set; }

        public UpdateCase Update(List<PartValueAndFL> pvfls, double voiceVolume, TimelineSourceDescription desc)
        {
            if (pvfls.Count == 0)
                return UpdateCase.None;

            #region Source
            IsEmpty = true;
            FilePath = string.Empty;
            SceneId = Guid.Empty;
            EnableLayers = [];

            for (int i = pvfls.Count - 1; i >= 0; i--)
            {
                ControlledParametersOfPart cp = pvfls[i].PartValue.ControlledParameters;

                switch (cp.ThisLayerType)
                {
                    case LayerType.Image:
                        ImageFileParameter ip = (ImageFileParameter)cp.SourceSelectArg;
                        FilePath = ip.ImageFilePath;
                        break;

                    case LayerType.Psd:
                        PsdFileParameter pp = (PsdFileParameter)cp.SourceSelectArg;
                        FilePath = pp.PsdFileInfo.FilePath ?? string.Empty;
                        EnableLayers = pp.PsdFileInfo.EnableLayers;
                        break;

                    case LayerType.Video:
                        VideoFileParameter vp = (VideoFileParameter)cp.SourceSelectArg;
                        FilePath = vp.VideoFilePath;
                        Time = TimeSpan.FromSeconds((vp.StartFrameNumber + pvfls[i].FL.Frame * vp.PlaybackSpeed / 100.0) / desc.FPS);
                        LoopPlayback = vp.LoopPlayback;
                        break;

                    case LayerType.Scene:
                        SceneParameter sp = (SceneParameter)cp.SourceSelectArg;
                        SceneId = sp.SceneId;
                        Time = TimeSpan.FromSeconds((sp.StartFrameNumber + pvfls[i].FL.Frame * sp.PlaybackSpeed / 100.0) / desc.FPS);
                        LoopPlayback = sp.LoopPlayback;
                        break;
                }

                if (!string.IsNullOrEmpty(FilePath) || SceneId != Guid.Empty)
                {
                    ThisLayerType = cp.ThisLayerType;
                    IsEmpty = false;
                    break;
                }
            }

            if (IsEmpty)
                return UpdateCase.None;
            #endregion

            #region PartAnimationValue
            PartAnimationArgs.Clear();

            if (ThisLayerType == LayerType.Image)
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
                    else
                    {
                        break;
                    }
                }

                if (isMulti)
                {
                    List<PartAnimationArg> partAnimationArgs = [];

                    foreach (PartValueAndFL pvfl in pvfls)
                    {
                        ControlledParametersOfPart cp = pvfl.PartValue.ControlledParameters;

                        if (cp.SourceSelectArg is IPartAnimationValueParameter parameter)
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
                                        paa1.Value = pav.AnmOpeArgments.GetValue(pvfl.FL, desc.FPS, voiceVolume);

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
                                            Index = pav.Index,
                                            NormalizationMode = pav.NormalizationMode,
                                            LinkOpeMode = pav.LinkOpeMode,
                                            Value = pav.AnmOpeArgments.GetValue(pvfl.FL, desc.FPS, voiceVolume)
                                        };

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
                                partAnimationArg.Value = pav.AnmOpeArgments.GetValue(pvfl.FL, desc.FPS, voiceVolume);
                            }
                        }
                    }

                    PartAnimationArgs = [partAnimationArg];
                }
            }
            #endregion

            #region LayerInformation
            TagToClipTo = string.Empty;
            Parent = string.Empty;

            foreach (PartValueAndFL pvfl in pvfls)
            {
                ControlledParametersOfPart cp = pvfl.PartValue.ControlledParameters;

                if (cp.SourceSelectArg is IClipParameter clipParameter)
                {
                    ClippingMode = clipParameter.ClippingMode;

                    if (clipParameter.ClippingArg is IPartToClipToParameter partToClipToParameter)
                    {
                        if (!string.IsNullOrEmpty(partToClipToParameter.PartToClipTo))
                            TagToClipTo = partToClipToParameter.PartToClipTo;
                    }
                    else
                    {
                        TagToClipTo = string.Empty;
                    }
                }

                if (cp.SourceSelectArg is IParentParameter parentParameter)
                    if (string.IsNullOrEmpty(parentParameter.Parent))
                        Parent = parentParameter.Parent;
            }
            #endregion

            #region Drawing
            X = Y = Z = Rotate = Priority = 0;
            Opacity = Zoom_X = Zoom_Y = 1;
            Invert = false;
            Blend = Blend.Normal;
            ZSort = ZSortMode2.BasedOnPriority;
            double zoom = 1;

            foreach (PartValueAndFL pvfl in pvfls)
            {
                ControlledParametersOfPart cp = pvfl.PartValue.ControlledParameters;
                FrameAndLength fl = pvfl.FL;
                int fps = desc.FPS;

                if (cp.DrawingArg is HavingSourceDrawingParameter hsdp)
                {
                    if (hsdp.SubArg is MasterModeParameter mmp)
                    {
                        switch (hsdp.ModeMaster)
                        {
                            case DrawingValueModeMaster.Override:
                                X = fl.GetValue(mmp.X, fps);
                                Y = fl.GetValue(mmp.Y, fps);
                                Z = fl.GetValue(mmp.Z, fps);
                                Opacity = fl.GetValue(mmp.Opacity, fps) / 100;
                                zoom = fl.GetValue(mmp.Zoom, fps) / 100;
                                Rotate = fl.GetValue(mmp.Rotation, fps);
                                Invert = fl.GetValue(mmp.Invert, fps) < 0.5;
                                Blend = mmp.Blend;
                                ZSort = mmp.ZSort;
                                Zoom_X = fl.GetValue(mmp.Zoom_X, fps) / 100;
                                Zoom_Y = fl.GetValue(mmp.Zoom_Y, fps) / 100;
                                Priority = fl.GetValue(mmp.Priority, fps);
                                break;

                            case DrawingValueModeMaster.Compose:
                                X += fl.GetValue(mmp.X, fps);
                                Y += fl.GetValue(mmp.Y, fps);
                                Z += fl.GetValue(mmp.Z, fps);
                                Opacity *= fl.GetValue(mmp.Opacity, fps) / 100;
                                zoom *= fl.GetValue(mmp.Zoom, fps) / 100;
                                Rotate += fl.GetValue(mmp.Rotation, fps);
                                Invert ^= fl.GetValue(mmp.Invert, fps) < 0.5;
                                Zoom_X *= fl.GetValue(mmp.Zoom_X, fps) / 100;
                                Zoom_Y *= fl.GetValue(mmp.Zoom_Y, fps) / 100;
                                Priority += fl.GetValue(mmp.Priority, fps);
                                break;
                        }
                    }
                    else if (hsdp.SubArg is CustomModeParameter cmp)
                    {
                        if (cmp.XYZMode == DrawingValueMode.Override)
                        {
                            X = fl.GetValue(cmp.X, fps);
                            Y = fl.GetValue(cmp.Y, fps);
                            Z = fl.GetValue(cmp.Z, fps);
                        }
                        else
                        {
                            X += fl.GetValue(cmp.X, fps);
                            Y += fl.GetValue(cmp.Y, fps);
                            Z += fl.GetValue(cmp.Z, fps);
                        }

                        if (cmp.OpacityMode == DrawingValueMode.Override)
                        {
                            Opacity = fl.GetValue(cmp.Opacity, fps) / 100;
                        }
                        else
                        {
                            Opacity *= fl.GetValue(cmp.Opacity, fps) / 100;
                        }

                        if (cmp.ZoomMode == DrawingValueMode.Override)
                        {
                            zoom = fl.GetValue(cmp.Zoom, fps) / 100;
                            Zoom_X = fl.GetValue(cmp.Zoom_X, fps) / 100;
                            Zoom_Y = fl.GetValue(cmp.Zoom_Y, fps) / 100;
                        }
                        else
                        {
                            zoom *= fl.GetValue(cmp.Zoom, fps) / 100;
                            Zoom_X *= fl.GetValue(cmp.Zoom_X, fps) / 100;
                            Zoom_Y *= fl.GetValue(cmp.Zoom_Y, fps) / 100;
                        }

                        if (cmp.RotationMode == DrawingValueMode.Override)
                        {
                            Rotate = fl.GetValue(cmp.Rotation, fps);
                        }
                        else
                        {
                            Rotate += fl.GetValue(cmp.Rotation, fps);
                        }

                        if (cmp.InverseMode == DrawingValueMode.Override)
                        {
                            Invert = fl.GetValue(cmp.Invert, fps) < 0.5;
                        }
                        else
                        {
                            Invert ^= fl.GetValue(cmp.Invert, fps) < 0.5;
                        }

                        if (cmp.BlendMode == DrawingValueMode.Override)
                        {
                            Blend = cmp.Blend;
                        }

                        if (cmp.ZSortMode == DrawingValueMode.Override)
                        {
                            ZSort = cmp.ZSort;
                        }

                        if (cmp.PriorityMode == DrawingValueMode.Override)
                        {
                            Priority = fl.GetValue(cmp.Priority, fps);
                        }
                        else
                        {
                            Priority += fl.GetValue(cmp.Priority, fps);
                        }
                    }
                }
            }
            #endregion
        }
    }
}
