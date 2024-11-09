using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Vortice.Direct2D1.Effects;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using SinTachiePlugin.Enums;
using System.Numerics;
using System.Collections.Immutable;
using YukkuriMovieMaker.Plugin;
using SinTachiePlugin.Parts.LayerValueListController;
using System.Windows.Controls;
using Path = System.IO.Path;
using SinTachiePlugin.LayerValueListController;
using YukkuriMovieMaker.Plugin.Effects;
using System.Windows.Media.Effects;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Parts
{
    public class PartNode : IDisposable
    {
        //readonly Crop cropEffect;
        readonly AffineTransform2D offset;
        readonly AffineTransform2D transform;
        readonly Opacity opacityEffect;
        //readonly GaussianBlur gblurEffect;
        //readonly DirectionalBlur dblurEffect;
        readonly ID2D1Bitmap empty;
        private readonly IGraphicsDevicesAndContext devices;
        public LayerNode? layerTree;
        ID2D1Image? source;
        public ID2D1Image? Output;

        public int BusNum { get; set; }
        public string TagName { get; set; } = string.Empty;
        public string Parent { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public List<LayerAnimationMode> LayerAnimationModes { get; set; } = [];
        public List<OuterLayerValueMode> OuterLayerValueModes { get; set; } = [];
        public List<double> LayerValues { get; set; } = [];
        public bool Appear { get; set; }
        public BlendSTP BlendMode { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Opacity { get; set; }
        public double Scale { get; set; }
        public double Rotate { get; set; }
        public bool Mirror { get; set; }
        public bool ScaleDependent { get; set; }
        public bool OpacityDependent { get; set; }
        public bool RotateDependent { get; set; }
        public double Cnt_X { get; set; }
        public double Cnt_Y { get; set; }
        public bool KeepPlace { get; set; }
        public double Exp_X { get; set; }
        public double Exp_Y { get; set; }
        //public double Top { get; set; }
        //public double Bottom { get; set; }
        //public double Left { get; set; }
        //public double Right { get; set; }
        //public double GBlurValue { get; set; }
        //public double DBlurValue { get; set; }
        //public double DBlurAngle { get; set; }
        public ImmutableList<IVideoEffectProcessor> Processors { get; set; } = [];
        public DrawDescription DrawDescription { get; set; } = new(
                new Vector3(), new Vector2(), new Vector2(), new Vector3(), Matrix4x4.Identity,
                new InterpolationMode(), 1.0, false, []
                );

        public Vector2 Shift { get; set; }
        public float Opacity2 { get; set; }
        public float Scale2 { get; set; }
        public float Rotate2 { get; set; }

        public ImmutableList<PartNode> ParentPath { get; set; } = [];

        public PartNode(IGraphicsDevicesAndContext devices)
        {
            this.devices = devices;
            //cropEffect = new Crop(devices.DeviceContext);
            offset = new AffineTransform2D(devices.DeviceContext);
            transform = new AffineTransform2D(devices.DeviceContext);
            opacityEffect = new Opacity(devices.DeviceContext);
            //gblurEffect = new GaussianBlur(devices.DeviceContext);
            //dblurEffect = new DirectionalBlur(devices.DeviceContext);
            empty = devices.DeviceContext.CreateEmptyBitmap();
        }

        public PartNode(PartNode origin) : this(devices: origin.devices)
        {
            BusNum = origin.BusNum;
            TagName = origin.TagName;
            Parent = origin.Parent;
            ImagePath = origin.ImagePath;
            LayerValues = origin.LayerValues.Select(x => x).ToList();
            Appear = origin.Appear;
            BlendMode = origin.BlendMode;
            X = origin.X;
            Y = origin.Y;
            Opacity = origin.Opacity;
            Scale = origin.Scale;
            Rotate = origin.Rotate;
            Mirror = origin.Mirror;
            ScaleDependent = origin.ScaleDependent;
            OpacityDependent = origin.OpacityDependent;
            RotateDependent = origin.RotateDependent;
            Cnt_X = origin.Cnt_X;
            Cnt_Y = origin.Cnt_Y;
            KeepPlace = origin.KeepPlace;
            Exp_X = origin.Exp_X;
            Exp_Y = origin.Exp_Y;
            //Top = origin.Top;
            //Bottom = origin.Bottom;
            //Left = origin.Left;
            //Right = origin.Right;
            //GBlurValue = origin.GBlurValue;
            //DBlurValue = origin.DBlurValue;
            //DBlurAngle = origin.DBlurAngle;
            Processors = origin.Processors;
            ParentPath = [.. origin.ParentPath];
        }

        public PartNode(IGraphicsDevicesAndContext devices, /*TimelineItemSourceDescription description,*/ PartBlock part, long length, long frame, int fps, double voiceVolume) : this(devices: devices)
        {
            this.devices = devices;
            BusNum = (int)part.BusNum.GetValue(frame, length, fps);
            TagName = part.TagName;
            Parent = part.Parent;
            ImagePath = part.ImagePath;
            LayerAnimationModes = part.LayerValues.Select(part => part.AnimationMode).ToList();
            OuterLayerValueModes = part.LayerValues.Select(part => part.OuterMode).ToList();
            LayerValues = part.LayerValues.Select(x => x.GetValue(length, frame, fps, voiceVolume)).ToList();
            Appear = part.Appear;
            BlendMode = part.BlendMode;
            X = part.X.GetValue(frame, length, fps);
            Y = part.Y.GetValue(frame, length, fps);
            Opacity = part.Opacity.GetValue(frame, length, fps);
            Scale = part.Scale.GetValue(frame, length, fps);
            Rotate = part.Rotate.GetValue(frame, length, fps);
            Mirror = (part.Mirror.GetValue(frame, length, fps) > 0.5);
            ScaleDependent = part.ScaleDependent;
            OpacityDependent = part.OpacityDependent;
            RotateDependent = part.RotateDependent;
            Cnt_X = part.Cnt_X.GetValue(frame, length, fps);
            Cnt_Y = part.Cnt_Y.GetValue(frame, length, fps);
            KeepPlace = part.KeepPlace;
            Exp_X = part.Exp_X.GetValue(frame, length, fps);
            Exp_Y = part.Exp_Y.GetValue(frame, length, fps);
            //Top = part.Top.GetValue(frame, length, fps);
            //Bottom = part.Bottom.GetValue(frame, length, fps);
            //Left = part.Left.GetValue(frame, length, fps);
            //Right = part.Right.GetValue(frame, length, fps);
            //GBlurValue = part.GBlurValue.GetValue(frame, length, fps);
            //DBlurValue = part.DBlurValue.GetValue(frame, length, fps);
            //DBlurAngle = part.DBlurAngle.GetValue(frame, length, fps);
            layerTree = new LayerNode(ImagePath, devices);
            offset.SetInput(0, layerTree.GetSource(LayerValues, OuterLayerValueModes), true);
            //Processors = part.Effects.Select(effect => effect.CreateVideoEffect(devices)).ToImmutableList();
            //var tmpSource = offset.Output;
            //var defRect = devices.DeviceContext.GetImageLocalBounds(tmpSource);
            //TimelineSourceDescription description1 = new((int)(defRect.Left - defRect.Right), (int)(defRect.Bottom - defRect.Top), fps, (int)frame, (int)length, description.Usage);
            //TimelineItemSourceDescription description2 = new(description1, (int)frame, (int)length, description.Layer);
            //EffectDescription description3 = new(description2, DrawDescription, 0);
            //int i = 0;
            //foreach (var processor in Processors)
            //{
            //    processor.SetInput(tmpSource);
            //    DrawDescription = processor.Update(description3);
            //}
            //source = tmpSource;
            source = offset.Output;
        }

        public bool Update(PartBlock part, long length, long frame, int fps, double voiceVolume)
        {
            //var busNum = part.GetBusNum(frame, length, fps);
            var appear = part.Appear;
            var busNum = (int)part.BusNum.GetValue(frame, length, fps);
            var tagName = part.TagName;
            var parent = part.Parent;
            var imagePath = part.ImagePath;
            var layerAnimationModes = part.LayerValues.Select(part => part.AnimationMode).ToList();
            var outerLayerValueModes = part.LayerValues.Select(part => part.OuterMode).ToList();
            var layerValues = part.LayerValues.Select(x => x.GetValue(length, frame, fps, voiceVolume)).ToList();
            var blendMode = part.BlendMode;
            var x = part.X.GetValue(frame, length, fps);
            var y = part.Y.GetValue(frame, length, fps);
            var opacity = part.Opacity.GetValue(frame, length, fps);
            var scale = part.Scale.GetValue(frame, length, fps);
            var rotate = part.Rotate.GetValue(frame, length, fps);
            var mirror = (part.Mirror.GetValue(frame, length, fps) > 0.5);
            var scaleDependent = part.ScaleDependent;
            var opacityDependent = part.OpacityDependent;
            var rotateDependent = part.RotateDependent;
            var cntX = part.Cnt_X.GetValue(frame, length, fps);
            var cntY = part.Cnt_Y.GetValue(frame, length, fps);
            var keepPlace = part.KeepPlace;
            var expX = part.Exp_X.GetValue(frame, length, fps);
            var expY = part.Exp_Y.GetValue(frame, length, fps);
            //var top = part.Top.GetValue(frame, length, fps);
            //var bottom = part.Bottom.GetValue(frame, length, fps);
            //var left = part.Left.GetValue(frame, length, fps);
            //var right = part.Right.GetValue(frame, length, fps);
            //var gBlurValue = part.GBlurValue.GetValue(frame, length, fps);
            //var dBlurValue = part.DBlurValue.GetValue(frame, length, fps);
            //var dBlurAngle = part.DBlurAngle.GetValue(frame, length, fps);

            //var effects = part.Effects;

            bool isOld = false;


            if (ImagePath != imagePath)
            {
                isOld = true;

                ImagePath = imagePath;
                LayerAnimationModes = layerAnimationModes;
                OuterLayerValueModes = outerLayerValueModes;
                LayerValues = layerValues;
                offset.SetInput(0, null, true);
                layerTree?.Dispose();
                source?.Dispose();
                layerTree = new LayerNode(ImagePath, devices);
                offset.SetInput(0, layerTree.GetSource(LayerValues, OuterLayerValueModes), true);
                source = offset.Output;
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
                    offset.SetInput(0, null, true);
                    source?.Dispose();
                    if (layerTree is null)
                        layerTree = new LayerNode(ImagePath, devices);
                    offset.SetInput(0, layerTree.GetSource(LayerValues, OuterLayerValueModes), true);
                    source = offset.Output;
                }
            }

            if (BusNum != busNum || TagName != tagName || Parent != parent || appear != Appear || BlendMode != blendMode ||
                X != x || Y != y || Opacity != opacity || Scale != scale || Rotate != rotate || Mirror != mirror ||
                Cnt_X != cntX || Cnt_Y != cntY || keepPlace != KeepPlace || Exp_X != expX || Exp_Y != expY ||
                //Top != top || Bottom != bottom || Left != left || Right != right 
                //|| GBlurValue != gBlurValue || DBlurValue != dBlurValue || DBlurAngle != dBlurAngle ||
                ScaleDependent != scaleDependent || OpacityDependent != opacityDependent || RotateDependent != rotateDependent)
            {
                isOld = true;
                BusNum = busNum;
                TagName = tagName;
                Parent = parent;
                Appear = appear;
                BlendMode = blendMode;
                X = x;
                Y = y;
                Opacity = opacity;
                Scale = scale;
                Rotate = rotate;
                Mirror = mirror;
                Cnt_X = cntX;
                Cnt_Y = cntY;
                KeepPlace = keepPlace;
                Exp_X = expX;
                Exp_Y = expY;
                //Top = top;
                //Bottom = bottom;
                //Left = left;
                //Right = right;
                //GBlurValue = gBlurValue;
                //DBlurValue = dBlurValue;
                //DBlurAngle = dBlurAngle;
                TagName = tagName;
                Parent = parent;
                ScaleDependent = scaleDependent;
                OpacityDependent = opacityDependent;
                RotateDependent = rotateDependent;
            }

            return isOld;
        }

        public void CommitOutput()
        {
            if (source == null)
            {
                Output = empty;
                return;
            }

            ID2D1Image? output = source;

            // リセット
            //dblurEffect.SetInput(0, null, true);
            //gblurEffect.SetInput(0, null, true);
            opacityEffect.SetInput(0, null, true);
            transform.SetInput(0, null, true);
            //cropEffect.SetInput(0, null, true);

            // 依存関係から最終的な描画位置などを計算する。
            double x = 0, y = 0, scale = 1, opacity = 1, rotate = 0;
            foreach (var node in ParentPath)
            {
                double x2 = node.X + (node.KeepPlace ? node.Cnt_X : 0), y2 = node.Y + (node.KeepPlace ? node.Cnt_Y : 0);
                float rotate2 = (float)(rotate * Math.PI / 180);
                x += scale * (x2 * Math.Cos(rotate2) - y2 * Math.Sin(rotate2));
                y += scale * (x2 * Math.Sin(rotate2) + y2 * Math.Cos(rotate2));
                opacity = ((node.OpacityDependent) ? opacity : 1) * node.Opacity / 100;
                scale = ((node.ScaleDependent) ? scale : 1) * node.Scale / 100;
                rotate = ((node.RotateDependent) ? rotate : 0) + node.Rotate;
            }
            float rotate3 = (float)(rotate * Math.PI / 180);
            double x3 = X - (KeepPlace ? 0 : Cnt_X), y3 = Y - (KeepPlace ? 0 : Cnt_Y);
            x += scale * (x3 * Math.Cos(rotate3) - y3 * Math.Sin(rotate3));
            y += scale * (x3 * Math.Sin(rotate3) + y3 * Math.Cos(rotate3));
            opacity = (OpacityDependent ? opacity : 1) * Opacity / 100;
            scale = (ScaleDependent ? scale : 1) * Scale / 100;
            Shift = new Vector2((float)x, (float)y);
            Opacity2 = (float)opacity;
            Scale2 = (float)scale;
            Rotate2 = (float)(RotateDependent ? (rotate * Math.PI / 180) : 0);

            // 四角形切り抜きエフェクト
            /*if (Left != 0.0 || Top != 0.0 || Right != 0.0 || Bottom != 0.0)
            {
                var defRect = devices.DeviceContext.GetImageLocalBounds(output);
                var defVect = new Vector4(defRect.Left, defRect.Top, defRect.Right, defRect.Bottom);
                var X = (float)(defVect.X + Left);
                var Y = (float)(defVect.Y + Top);
                var Z = (float)(defVect.Z - Right);
                var W = (float)(defVect.W - Bottom);
                cropEffect.Rectangle = new Vector4(X, Y, Z, W);
                cropEffect.SetInput(0, output, true);
                output = cropEffect.Output;
            }*/

            // アフィン変換(インスタンスOutputを固定するために，このエフェクトは無条件に間に挟む)
            var result = (Mirror) ? new Matrix3x2(-1, 0, 0, 1, 2 * (float)Cnt_X, 0) : Matrix3x2.Identity;
            float expX = (float)(Exp_X / 100.0);
            float expY = (float)(Exp_Y / 100.0);
            float rotate4 = (float)(Rotate * Math.PI / 180);
            float cx = (float)(Cnt_X * Math.Cos(Rotate2) - Cnt_Y * Math.Sin(Rotate2));
            float cy = (float)(Cnt_X * Math.Sin(Rotate2) + Cnt_Y * Math.Cos(Rotate2));
            if (Scale2 != 1.0 || expX != 1.0 || expY != 1.0)
                result *= Matrix3x2.CreateScale(Scale2 * expX, Scale2 * expY, new Vector2(cx, cy));
            if (Rotate2 != 0.0)
                result *= Matrix3x2.CreateRotation(Rotate2);
            if (Rotate != 0.0)
                result *= Matrix3x2.CreateRotation((float)rotate4, new Vector2(cx, cy));
            //MessageBox.Show($"TagName={TagName}\nRotate2={Rotate2}");
            transform.TransformMatrix = result;
            transform.SetInput(0, output, true);
            output = transform.Output;

            // 不透明エフェクト
            if (Opacity2 != 1.0)
            {
                opacityEffect.Value = Opacity2;
                opacityEffect.SetInput(0, output, true);
                output = opacityEffect.Output;
            }

            // ぼかしエフェクト
            //if (GBlurValue != 0.0)
            //{
            //    gblurEffect.StandardDeviation = (float)GBlurValue;
            //    gblurEffect.SetInput(0, output, true);
            //    output = gblurEffect.Output;
            //}

            // 方向ブラーエフェクト
            //if (DBlurValue != 0.0)
            //{
            //    dblurEffect.StandardDeviation = (float)DBlurValue;
            //    dblurEffect.Angle = (float)(-DBlurAngle - rotate);
            //    dblurEffect.SetInput(0, output, true);
            //    output = dblurEffect.Output;
            //}

            // 出力イメージ
            Output = output;
        }

        public void Dispose()
        {
            Output?.Dispose(); // EffectからgetしたOutputは必ずDisposeする必要がある。Effect側では開放されない。

            //dblurEffect.SetInput(0, null, true); // EffectのInputは必ずnullに戻す。
            //dblurEffect.Dispose();

            //gblurEffect.SetInput(0, null, true); // EffectのInputは必ずnullに戻す。
            //gblurEffect.Dispose();

            opacityEffect.SetInput(0, null, true); // EffectのInputは必ずnullに戻す。
            opacityEffect.Dispose();

            transform.SetInput(0, null, true); // EffectのInputは必ずnullに戻す。
            transform.Dispose();

            //cropEffect.SetInput(0, null, true); // EffectのInputは必ずnullに戻す。
            //cropEffect.Dispose();

            source?.Dispose();
            offset.SetInput(0, null, true); // EffectのInputは必ずnullに戻す。
            offset.Dispose();
            layerTree?.Dispose(); // 読み込んだ画像を破棄
            empty.Dispose();
        }
    }
}
