using SinTachiePlugin.Enums;
using SinTachiePlugin.LayerValueListController;
using SinTachiePlugin.Parts.LayerValueListController;
using System.Collections.Immutable;
using System.Numerics;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.Effects;
using Size = System.Drawing.Size;

namespace SinTachiePlugin.Parts
{
    internal class ParamsOfPartNode : IDisposable
    {
        readonly DisposeCollector disposer = new();
        readonly AffineTransform2D transform;
        public LayerNode LayerTree;
        public ID2D1Image Output;

        bool disposedValue = false;

        public bool Appear { get; set; }
        public BlendSTP BlendMode { get; set; }
        public string TagName { get; set; } = string.Empty;
        public string Parent { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public List<LayerAnimationMode> LayerAnimationModes { get; set; } = [];
        public List<OuterLayerValueMode> OuterLayerValueModes { get; set; } = [];
        public List<double> LayerValues { get; set; } = [];
        public Double3 Draw { get; set; }
        public double Opacity { get; set; }
        public double Scale { get; set; }
        public double Rotate { get; set; }
        public bool Mirror { get; set; }
        public Double2 Center { get; set; } = new();
        public bool KeepPlace { get; set; }
        public Double2 ExpXY { get; set; } = new();
        public bool XYZDependent { get; set; }
        public bool ScaleDependent { get; set; }
        public bool OpacityDependent { get; set; }
        public bool RotateDependent { get; set; }
        public bool MirrorDependent { get; set; }
        public bool EffectXYZDependent { get; set; }
        public bool EffectZoomDependent { get; set; }
        public bool EffectOpacityDependent { get; set; }
        public bool EffectRotateDependent { get; set; }
        public bool EffectMirrorDependent { get; set; }
        public bool EffectCameraDependent { get; set; }
        public bool EffectUnlazyDependent { get; set; }
        public ImmutableList<IVideoEffect> Effects { get; set; }

        public ParamsOfPartNode(IGraphicsDevicesAndContext devices, PartBlock block, long length, long frame, int fps, double voiceVolume)
        {
            Appear = block.Appear;
            BlendMode = block.BlendMode;
            Draw = new(
                block.X.GetValue(frame, length, fps),
                block.Y.GetValue(frame, length, fps),
                block.Z.GetValue(frame, length, fps)
                );
            Opacity = block.Opacity.GetValue(frame, length, fps);
            Scale = block.Scale.GetValue(frame, length, fps);
            Rotate = block.Rotate.GetValue(frame, length, fps);
            Mirror = block.Mirror.GetValue(frame, length, fps) > 0.5;
            Center = new(
                block.Cnt_X.GetValue(frame, length, fps),
                block.Cnt_Y.GetValue(frame, length, fps)
                );
            KeepPlace = block.KeepPlace;
            ExpXY = new(
                block.Exp_X.GetValue(frame, length, fps),
                block.Exp_Y.GetValue(frame, length, fps)
                );
            TagName = block.TagName;
            Parent = block.Parent;
            ImagePath = block.ImagePath;
            LayerAnimationModes = block.LayerValues.Select(part => part.AnimationMode).ToList();
            OuterLayerValueModes = block.LayerValues.Select(part => part.OuterMode).ToList();
            LayerValues = block.LayerValues.Select(x => x.GetValue(length, frame, fps, voiceVolume)).ToList();
            XYZDependent = block.XYZDependent;
            ScaleDependent = block.ScaleDependent;
            OpacityDependent = block.OpacityDependent;
            RotateDependent = block.RotateDependent;
            MirrorDependent = block.MirrorDependent;
            EffectXYZDependent = block.EffectXYZDependent;
            EffectZoomDependent = block.EffectZoomDependent;
            EffectOpacityDependent = block.EffectOpacityDependent;
            EffectRotateDependent = block.EffectRotateDependent;
            EffectMirrorDependent = block.EffectMirrorDependent;
            EffectCameraDependent = block.EffectCameraDependent;
            EffectUnlazyDependent = block.EffectUnlazyDependent;
            Effects = block.Effects;

            LayerTree = new LayerNode(ImagePath, devices);
            disposer.Collect(LayerTree);

            transform = new(devices.DeviceContext);
            disposer.Collect(transform);

            transform.SetInput(0, LayerTree.GetSource(LayerValues, OuterLayerValueModes), true);

            Output = transform.Output;
            disposer.Collect(Output);
        }

        public bool Update(IGraphicsDevicesAndContext devices, PartBlock block, long length, long frame, int fps, double voiceVolume)
        {
            var appear = block.Appear;
            var blendMode = block.BlendMode;
            var tagName = block.TagName;
            var imagePath = block.ImagePath;
            var layerAnimationModes = block.LayerValues.Select(part => part.AnimationMode).ToList();
            var outerLayerValueModes = block.LayerValues.Select(part => part.OuterMode).ToList();
            var layerValues = block.LayerValues.Select(x => x.GetValue(length, frame, fps, voiceVolume)).ToList();
            var parent = block.Parent;
            var draw = new Double3(
                block.X.GetValue(frame, length, fps),
                block.Y.GetValue(frame, length, fps),
                block.Z.GetValue(frame, length, fps)
                );
            var opacity = block.Opacity.GetValue(frame, length, fps);
            var scale = block.Scale.GetValue(frame, length, fps);
            var rotate = block.Rotate.GetValue(frame, length, fps);
            var mirror = block.Mirror.GetValue(frame, length, fps) > 0.5;
            var center = new Double2(
                block.Cnt_X.GetValue(frame, length, fps),
                block.Cnt_Y.GetValue(frame, length, fps)
                );
            var keepPlace = block.KeepPlace;
            var expXY = new Double2(
                block.Exp_X.GetValue(frame, length, fps),
                block.Exp_Y.GetValue(frame, length, fps)
                );
            var xyzDependent = block.XYZDependent;
            var scaleDependent = block.ScaleDependent;
            var opacityDependent = block.OpacityDependent;
            var rotateDependent = block.RotateDependent;
            var mirrorDependent = block.MirrorDependent;
            var effectXYZDependent = block.EffectXYZDependent;
            var effectScaleDependent = block.EffectZoomDependent;
            var effectRotateDependent = block.EffectRotateDependent;
            var effectOpacityDependent = block.EffectOpacityDependent;
            var effectCameraDependent = block.EffectCameraDependent;
            var effectMirrorDependent = block.EffectMirrorDependent;
            var effectUnlazyDependent = block.EffectUnlazyDependent;
            var effects = block.Effects;

            bool isOld = false;

            if (ImagePath != imagePath)
            {
                isOld = true;

                ImagePath = imagePath;
                LayerAnimationModes = layerAnimationModes;
                OuterLayerValueModes = outerLayerValueModes;
                LayerValues = layerValues;
                transform.SetInput(0, null, true);
                LayerTree.Dispose();
                LayerTree = new LayerNode(ImagePath, devices);
                transform.SetInput(0, LayerTree.GetSource(LayerValues, OuterLayerValueModes), true);
            }
            else
            {
                bool layerValuesIsEqual = LayerValues.Count == layerValues.Count;
                if (layerValuesIsEqual)
                {
                    for (int i = 0; i < LayerValues.Count; i++)
                        if (LayerAnimationModes[i] != layerAnimationModes[i] || OuterLayerValueModes[i] != outerLayerValueModes[i] || LayerValues[i] != layerValues[i])
                        {
                            layerValuesIsEqual = false;
                            break;
                        }
                }
                if (!layerValuesIsEqual)
                {
                    isOld = true;
                    LayerAnimationModes = layerAnimationModes;
                    OuterLayerValueModes = outerLayerValueModes;
                    LayerValues = layerValues;
                    transform.SetInput(0, null, true);
                    //LayerTree ??= new LayerNode(ImagePath, devices);
                    transform.SetInput(0, LayerTree.GetSource(LayerValues, OuterLayerValueModes), true);
                }
            }

            if (Appear != appear || BlendMode != blendMode || Draw != draw || Scale != scale || Opacity != opacity || Rotate != rotate || Mirror != mirror
                || Center != center || KeepPlace != keepPlace || ExpXY != expXY || TagName != tagName || Parent != parent

                || XYZDependent != xyzDependent || ScaleDependent != scaleDependent || OpacityDependent != opacityDependent
                || RotateDependent != rotateDependent || MirrorDependent != mirrorDependent

                || EffectXYZDependent != effectXYZDependent || EffectZoomDependent != effectScaleDependent || EffectRotateDependent != effectRotateDependent
                || EffectOpacityDependent != effectOpacityDependent || EffectMirrorDependent != effectMirrorDependent || EffectCameraDependent != effectCameraDependent
                || EffectUnlazyDependent != effectUnlazyDependent
                || effects.Count > 0 || effects.Count != Effects.Count)
            {
                Appear = appear;
                BlendMode = blendMode;
                Draw = draw;
                Opacity = opacity;
                Scale = scale;
                Rotate = rotate;
                Mirror = mirror;
                Center = center;
                KeepPlace = keepPlace;
                ExpXY = expXY;
                TagName = tagName;
                Parent = parent;
                XYZDependent = xyzDependent;
                ScaleDependent = scaleDependent;
                OpacityDependent = opacityDependent;
                RotateDependent = rotateDependent;
                MirrorDependent = mirrorDependent;
                EffectXYZDependent = effectXYZDependent;
                EffectZoomDependent = effectScaleDependent;
                EffectOpacityDependent = effectOpacityDependent;
                EffectMirrorDependent = effectMirrorDependent;
                EffectCameraDependent = effectCameraDependent;
                EffectUnlazyDependent = effectUnlazyDependent;
                Effects = effects;
                isOld = true;
            }
            return isOld;
        }

        void ClearEffectChain()
        {
            transform.SetInput(0, null, true);
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                ClearEffectChain();
                disposer.Dispose();
                GC.SuppressFinalize(this);
                disposedValue = true;
            }
        }
    }
}
