using Vortice.Direct2D1.Effects;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using SinTachiePlugin.Enums;
using System.Numerics;
using System.Collections.Immutable;
using YukkuriMovieMaker.Player.Video;
using Vortice.Mathematics;

namespace SinTachiePlugin.Parts
{
    public class PartNode : IDisposable
    {
        readonly public PartBlock block;
        readonly IGraphicsDevicesAndContext devices;
        readonly DisposeCollector disposer = new();
        readonly AffineTransform2D transform;
        readonly Crop cropEffect;
        readonly Transform3D renderEffect;
        readonly Opacity opacityEffect;
        readonly FrameAndLength lastFL = new();
        readonly ID2D1Image renderOutput;
        public ID2D1Image? Output;
        List<(PartNode node, VideoEffectChainNode chain)> NodesAndChains = [];
        DrawDescription drawDescription = new(
            default, default, new Vector2(1f, 1f), default,
            Matrix4x4.Identity, InterpolationMode.Linear, 1.0, false, []);


        readonly ParamsOfPartNode Params;
        public string TagName => Params.TagName;
        public string Parent => Params.Parent;
        public bool Appear => Params.Appear;
        public BlendSTP BlendMode => Params.BlendMode;
        public ZSortMode ZSortMode => Params.ZSortMode;
        public int BusNum => Params.BusNum;
        public float M43;

        public ImmutableList<PartNode> ParentPath { get; set; } = [];

        public PartNode(IGraphicsDevicesAndContext devices, PartBlock block, FrameAndLength FL, int fps, double voiceVolume)
        {
            this.devices = devices;
            this.block = block;
            transform = new AffineTransform2D(devices.DeviceContext);
            disposer.Collect(transform);

            cropEffect = new Crop(devices.DeviceContext);
            disposer.Collect(cropEffect);

            renderEffect = new Transform3D(devices.DeviceContext);
            disposer.Collect(renderEffect);

            opacityEffect = new Opacity(devices.DeviceContext);
            disposer.Collect(opacityEffect);

            using (var image = transform.Output)
                cropEffect.SetInput(0, image, true);

            using (var image = cropEffect.Output)
                renderEffect.SetInput(0, image, true);

            using (var image = renderEffect.Output)
                opacityEffect.SetInput(0, image, true);

            renderOutput = renderEffect.Output;
            disposer.Collect(renderOutput);

            Output = opacityEffect.Output;
            disposer.Collect(Output);

            Params = new(devices, block, FL, fps, voiceVolume);
            disposer.Collect(Params);
        }

        void Update_NodesAndChain()
        {
            var disposedIndex = from i in from tuple in NodesAndChains
                                          where ParentPath.IndexOf(tuple.node) < 0
                                          select NodesAndChains.IndexOf(tuple)
                                orderby i descending
                                select i;
            foreach (int index in disposedIndex)
            {
                var e_ep = NodesAndChains[index];
                e_ep.chain.Dispose();
                NodesAndChains.RemoveAt(index);
            }

            List<PartNode> keeped = NodesAndChains.Select((e_ep) => e_ep.node).ToList();
            List<(PartNode, VideoEffectChainNode)> newNodesAndChains = new(ParentPath.Count);
            foreach (var node in ParentPath)
            {
                int index = keeped.IndexOf(node);
                newNodesAndChains.Add(index < 0 ? (node, new VideoEffectChainNode(devices, node.Params.Effects, node.lastFL)) : NodesAndChains[index]);
            }

            NodesAndChains = newNodesAndChains;
        }

        public UpdateCase UpdateParams(FrameAndLength fl, int fps, double voiceVolume)
        {
            lastFL.CopyFrom(fl);
            return Params.Update(devices, block, fl, fps, voiceVolume);
        }

        public void UpdateOutput(TimelineSourceDescription desc)
        {
            ID2D1Image input = Params.Output;

            ID2D1Image input2 = input;

            Update_NodesAndChain();
            drawDescription = new(default, default, new Vector2(1f), default, Matrix4x4.Identity, InterpolationMode.Linear, 1.0, false, []);

            Double3 draw = new(), draw2 = new(), rotate = new();
            (double cos, double sin) Trig = new();
            Vector2 zoom = new(1f);
            Matrix4x4 camera = Matrix4x4.Identity;
            InterpolationMode zoomInterpolationMode = InterpolationMode.Linear;
            double scale = 1.0, opacity = 1.0, rotate2, scale2;
            bool xyzDependent, rotateDependent, scaleDependent, opacityDependent, mirrorDependent, mirror = false,
                effectXYZDependent, effectRotateDependent, effectZoomDependent, effectOpacityDependent, effectMirrorDependent, effectCameraDependent, effectUnlazyDependent;

            xyzDependent = rotateDependent = scaleDependent = opacityDependent = mirrorDependent
                = effectXYZDependent = effectRotateDependent = effectZoomDependent = effectOpacityDependent = effectMirrorDependent = effectCameraDependent = effectUnlazyDependent
                = true;
            foreach (var tuple in NodesAndChains)
            {
                var node = tuple.node;
                rotate2 = node.Params.Rotate * Math.PI / 180.0;
                scale2 = node.Params.Scale / 100.0;

                if (xyzDependent)
                {
                    Trig.cos = Math.Cos(rotate2);
                    Trig.sin = Math.Sin(rotate2);
                    draw.X *= (scaleDependent ? scale2 : 1) * (node.Params.Mirror && mirrorDependent ? -1 : 1);
                    draw.Y *= scaleDependent ? scale2 : 1;
                    draw.Z *= scaleDependent ? scale2 : 1;
                    draw2.X = Trig.cos * draw.X - Trig.sin * draw.Y;
                    draw2.Y = Trig.sin * draw.X + Trig.cos * draw.Y;
                    draw2.Z = draw.Z;

                    draw.X = draw2.X + (node.Params.Draw.X + (node.Params.KeepPlace ? node.Params.Center.X : 0.0));
                    draw.Y = draw2.Y + (node.Params.Draw.Y + (node.Params.KeepPlace ? node.Params.Center.Y : 0.0));
                    draw.Z = draw2.Z + node.Params.Draw.Z;

                    xyzDependent = node.Params.XYZDependent;
                }

                if (scaleDependent)
                {
                    scale *= scale2;
                    zoom *= (float)scale2;
                    scaleDependent = node.Params.ScaleDependent;
                }

                if (opacityDependent)
                {
                    opacity *= node.Params.Opacity / 100.0;
                    scaleDependent = node.Params.OpacityDependent;
                }

                if (rotateDependent)
                {
                    rotate.Z += node.Params.Rotate;
                    rotateDependent = node.Params.RotateDependent;
                }

                if (mirrorDependent)
                {
                    mirror ^= node.Params.Mirror;
                    mirrorDependent = node.Params.MirrorDependent;
                }

                drawDescription = new DrawDescription(
                    new Vector3((float)draw.X, (float)draw.Y, (float)draw.Z),
                    default,
                    new Vector2((float)(zoom.X * Params.ExpXY.X / 100.0), (float)(zoom.Y * Params.ExpXY.Y / 100.0)),
                    new Vector3((float)rotate.X, (float)rotate.Y, (float)rotate.Z),
                    camera,
                    zoomInterpolationMode,
                    opacity,
                    mirror,
                    []
                    );

                var chain = tuple.chain;

                chain.UpdateChain(node.Params.Effects, node.Params.FrameAndLength);
                drawDescription = chain.UpdateOutputAndDescription(input2, desc, drawDescription);

                if (effectUnlazyDependent)
                {
                    input2 = chain.Output;
                    effectUnlazyDependent = node.Params.EffectUnlazyDependent;
                }

                if (effectXYZDependent)
                {
                    draw.X = drawDescription.Draw.X;
                    draw.Y = drawDescription.Draw.Y;
                    draw.Z = drawDescription.Draw.Z;
                    effectXYZDependent = node.Params.EffectXYZDependent;
                }

                if (effectRotateDependent)
                {
                    rotate.X = drawDescription.Rotation.X;
                    rotate.Y = drawDescription.Rotation.Y;
                    rotate.Z = drawDescription.Rotation.Z;
                    effectRotateDependent = node.Params.EffectRotateDependent;
                }

                if (effectOpacityDependent)
                {
                    opacity = drawDescription.Opacity;
                    effectOpacityDependent = node.Params.EffectOpacityDependent;
                }

                if (effectZoomDependent)
                {
                    zoom = drawDescription.Zoom;
                    effectZoomDependent = node.Params.EffectZoomDependent;
                }

                if (effectCameraDependent)
                {
                    camera = drawDescription.Camera;
                    effectCameraDependent = node.Params.EffectZoomDependent;
                }

                zoomInterpolationMode = drawDescription.ZoomInterpolationMode;

                if (effectMirrorDependent)
                {
                    mirror = drawDescription.Invert;
                    effectMirrorDependent = node.Params.EffectMirrorDependent;
                }
            }

            transform.SetInput(0, input2, true);
            Vector3 draw3 = drawDescription.Draw;
            Vector2 centerPoint = drawDescription.CenterPoint;
            AffineTransform2DInterpolationMode interPolationMode = drawDescription.ZoomInterpolationMode.ToTransform2D();
            Transform3DInterpolationMode interPolationMode2 = drawDescription.ZoomInterpolationMode.ToTransform3D();
            transform.InterPolationMode = interPolationMode;
            transform.TransformMatrix = Matrix3x2.CreateTranslation(-1 * new Vector2((float)Params.Center.X, (float)Params.Center.Y)) * Matrix3x2.CreateScale(zoom.X, zoom.Y);
            renderEffect.InterPolationMode = interPolationMode2;
            renderEffect.TransformMatrix = (
                mirror ? Matrix4x4.CreateScale(-1f, 1f, 1f, new Vector3(centerPoint, 0f)) : Matrix4x4.Identity)
                * Matrix4x4.CreateRotationZ(MathF.PI * (float)rotate.Z / 180f)
                * Matrix4x4.CreateRotationY(MathF.PI * -(float)rotate.Y / 180f)
                * Matrix4x4.CreateRotationX(MathF.PI * -(float)rotate.X / 180f)
                * Matrix4x4.CreateTranslation(draw3)
                * camera
                * new Matrix4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, -0.001f, 0f, 0f, 0f, 1f);

            Apply(devices.DeviceContext);
            opacityEffect.Value = (float)drawDescription.Opacity;
            M43 = renderEffect.TransformMatrix.M43;
        }

        void ClearEffectChain()
        {
            opacityEffect.SetInput(0, null, true);
            renderEffect.SetInput(0, null, true);
            cropEffect.SetInput(0, null, true);
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
                    NodesAndChains.ForEach(tuple => tuple.chain.Dispose());
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

        #region SafeTransform3DHelper
        const float D3D11_FTOI_INSTRUCTION_MAX_INPUT = 2.1474836E+09f;
        const float D3D11_FTOI_INSTRUCTION_MIN_INPUT = -2.1474836E+09f;

        void Apply(ID2D1DeviceContext deviceContext)
        {
            //transform3dエフェクトの出力画像1pxあたりの入力サイズが4096pxを超えるとエラーになる
            //エラー時には出力サイズがD3D11_FTOI_INSTRUCTION_MAX_INPUTになるため、cropエフェクトを使用し入力サイズを4096pxに制限する

            //一旦cropエフェクトの範囲を初期化する
            cropEffect.Rectangle = new Vector4(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
            var renderBounds = deviceContext.GetImageLocalBounds(renderOutput);
            if (renderBounds.Left == D3D11_FTOI_INSTRUCTION_MIN_INPUT
                || renderBounds.Top == D3D11_FTOI_INSTRUCTION_MIN_INPUT
                || renderBounds.Right == D3D11_FTOI_INSTRUCTION_MAX_INPUT
                || renderBounds.Bottom == D3D11_FTOI_INSTRUCTION_MAX_INPUT)
            {
                //エラーの場合にのみ入力サイズを制限する
                cropEffect.Rectangle = new Vector4(-2048, -2048, 2048, 2048);
            }
        }
        #endregion
    }
}
