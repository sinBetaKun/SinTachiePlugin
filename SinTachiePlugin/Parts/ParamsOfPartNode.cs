using SinTachiePlugin.Enums;
using SinTachiePlugin.LayerValueListController;
using System.Collections.Immutable;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Parts
{
    internal class ParamsOfPartNode : IDisposable
    {
        readonly DisposeCollector disposer = new();
        readonly AffineTransform2D transform;
        public LayerNodeManager LayerTree;
        public ID2D1Image Output;
        public FrameAndLength FrameAndLength;

        public bool Appear { get; set; }
        public BlendSTP BlendMode { get; set; }
        public ZSortMode ZSortMode { get; set; }
        public int BusNum { get; set; }
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

        public ParamsOfPartNode(IGraphicsDevicesAndContext devices, PartBlock block, FrameAndLength fl, int fps, double voiceVolume)
        {
            FrameAndLength = new(fl);
            Appear = block.Appear;
            BlendMode = block.BlendMode;
            ZSortMode = block.ZSortMode;
            BusNum = (int)fl.GetValue(block.BusNum, fps);
            Draw = fl.GetDouble3(block.X, block.Y, block.Z, fps);
            Opacity = fl.GetValue(block.Opacity, fps);
            Scale = fl.GetValue(block.Scale, fps);
            Rotate = fl.GetValue(block.Rotate, fps);
            Mirror = fl.GetValue(block.Mirror, fps) > 0.5;
            Center = fl.GetDouble2(block.Cnt_X, block.Cnt_Y, fps);
            KeepPlace = block.KeepPlace;
            ExpXY = fl.GetDouble2(block.Exp_X, block.Exp_Y, fps);
            TagName = block.TagName;
            Parent = block.Parent;
            ImagePath = block.ImagePath;
            LayerAnimationModes = block.LayerValues.Select(part => part.AnimationMode).ToList();
            OuterLayerValueModes = block.LayerValues.Select(part => part.OuterMode).ToList();
            LayerValues = block.LayerValues.Select(x => x.GetValue(fl, fps, voiceVolume)).ToList();
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

            LayerTree = new(ImagePath, devices);
            disposer.Collect(LayerTree);

            transform = new(devices.DeviceContext);
            disposer.Collect(transform);

            LayerTree.UpdateSource(LayerValues, OuterLayerValueModes);
            transform.SetInput(0, LayerTree.Output, true);

            Output = transform.Output;
            disposer.Collect(Output);
        }

        public UpdateCase Update(IGraphicsDevicesAndContext devices, PartBlock block, FrameAndLength fl, int fps, double voiceVolume)
        {
            FrameAndLength.CopyFrom(fl);

            var appear = block.Appear;
            var blendMode = block.BlendMode;
            var zSortMode = block.ZSortMode;
            var busNum = (int)block.BusNum.GetValue(fl.Frame, fl.Length, fps);
            var tagName = block.TagName;
            var imagePath = block.ImagePath;
            var layerAnimationModes = block.LayerValues.Select(part => part.AnimationMode).ToList();
            var outerLayerValueModes = block.LayerValues.Select(part => part.OuterMode).ToList();
            var layerValues = block.LayerValues.Select(x => x.GetValue(fl, fps, voiceVolume)).ToList();
            var parent = block.Parent;
            var draw = fl.GetDouble3(block.X, block.Y, block.Z, fps);
            var opacity = fl.GetValue(block.Opacity, fps);
            var scale = fl.GetValue(block.Scale, fps);
            var rotate = fl.GetValue(block.Rotate, fps);
            var mirror = fl.GetValue(block.Mirror, fps) > 0.5;
            var center = fl.GetDouble2(block.Cnt_X, block.Cnt_Y, fps);
            var keepPlace = block.KeepPlace;
            var expXY = fl.GetDouble2(block.Exp_X, block.Exp_Y, fps);
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

            UpdateCase updateCase = UpdateCase.None;

            if (ImagePath != imagePath)
            {
                updateCase = UpdateCase.BitmapParams;

                ImagePath = imagePath;
                LayerAnimationModes = layerAnimationModes;
                OuterLayerValueModes = outerLayerValueModes;
                LayerValues = layerValues;
                transform.SetInput(0, null, true);
                LayerTree.ChangeImageFile(ImagePath);
                LayerTree.UpdateSource(LayerValues, OuterLayerValueModes);
                transform.SetInput(0, LayerTree.Output, true);
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
                    updateCase = UpdateCase.BitmapParams;
                    LayerAnimationModes = layerAnimationModes;
                    OuterLayerValueModes = outerLayerValueModes;
                    LayerValues = layerValues;
                    transform.SetInput(0, null, true);
                    LayerTree.ChangeImageFile(ImagePath);
                    LayerTree.UpdateSource(LayerValues, OuterLayerValueModes);
                    transform.SetInput(0, LayerTree.Output, true);
                }
            }

            if (Appear != appear || ZSortMode != zSortMode || BusNum != busNum || Draw != draw || Scale != scale || Opacity != opacity || Rotate != rotate || Mirror != mirror
                || Center != center || KeepPlace != keepPlace || ExpXY != expXY || TagName != tagName || Parent != parent

                || XYZDependent != xyzDependent || ScaleDependent != scaleDependent || OpacityDependent != opacityDependent
                || RotateDependent != rotateDependent || MirrorDependent != mirrorDependent

                || EffectXYZDependent != effectXYZDependent || EffectZoomDependent != effectScaleDependent || EffectRotateDependent != effectRotateDependent
                || EffectOpacityDependent != effectOpacityDependent || EffectMirrorDependent != effectMirrorDependent || EffectCameraDependent != effectCameraDependent
                || EffectUnlazyDependent != effectUnlazyDependent
                || effects.Count > 0 || effects.Count != Effects.Count)
            {
                updateCase = UpdateCase.BitmapParams;

                Appear = appear;
                ZSortMode = zSortMode;
                BusNum = busNum;
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
            }

            if (BlendMode != blendMode)
            {
                BlendMode = blendMode;
                updateCase |= UpdateCase.BlendMode;
                return updateCase;
            }

            return updateCase;
        }

        void ClearEffectChain()
        {
            transform.SetInput(0, null, true);
        }

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // マネージド状態を破棄します (マネージド オブジェクト)
                    ClearEffectChain();
                    disposer.Dispose();
                }

                // アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
