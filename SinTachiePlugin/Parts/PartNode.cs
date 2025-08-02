using System.Collections.Immutable;
using System.Numerics;
using SinTachiePlugin.Enums;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

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

        public ImmutableList<PartNode> ParentPath { get; set; } = []; // 自分自身が最初にきている。

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
                var (node, chain) = NodesAndChains[index];
                chain.Dispose();
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

            transform.SetInput(0, input, true);
            ID2D1Image input2 = transform.Output;

            Update_NodesAndChain();
            drawDescription = new(default, default, new Vector2(1f), default, Matrix4x4.Identity, InterpolationMode.Linear, 1.0, false, []);

            Vector3 draw = new();
            Vector2 zoom = new(1f);
            Matrix4x4 camera = Matrix4x4.Identity, rotateM = Matrix4x4.Identity;
            InterpolationMode zoomInterpolationMode = InterpolationMode.Linear;
            double opacity = 1.0;
            bool xyzDependent, rotateDependent, scaleDependent, opacityDependent, mirrorDependent, mirror = false,
                cameraDependent, unlazyEffectDependent;

            xyzDependent = rotateDependent = scaleDependent = opacityDependent = mirrorDependent
                = cameraDependent = unlazyEffectDependent
                = true;

            Vector3[] draws = new Vector3[NodesAndChains.Count];
            Vector2[] zooms = new Vector2[NodesAndChains.Count];
            Vector3[] rotates = new Vector3[NodesAndChains.Count];
            bool[] mirrors = new bool[NodesAndChains.Count];
            bool[] scaleDependents = new bool[NodesAndChains.Count];
            bool[] rotateDependents = new bool[NodesAndChains.Count];
            bool[] mirrorDependents = new bool[NodesAndChains.Count];
            int len = 0;

            foreach (var tuple in NodesAndChains)
            {
                var node = tuple.node;

                drawDescription = new DrawDescription(
                     new Vector3(
                        (float)(node.Params.Draw.X + (node.Params.KeepPlace ? node.Params.Center.X : 0.0)),
                        (float)(node.Params.Draw.Y + (node.Params.KeepPlace ? node.Params.Center.Y : 0.0)),
                        (float)node.Params.Draw.Z
                        ),
                    default,
                    new Vector2((float)(node.Params.Scale / 100.0)),
                    new Vector3(0, 0, (float)node.Params.Rotate),
                    Matrix4x4.Identity,
                    zoomInterpolationMode,
                    node.Params.Opacity / 100.0,
                    node.Params.Mirror,
                    []
                    );

                var chain = tuple.chain;

                chain.UpdateChain(node.Params.Effects, node.Params.FrameAndLength);
                drawDescription = chain.UpdateOutputAndDescription(input2, desc, drawDescription);

                if (unlazyEffectDependent)
                {
                    input2 = chain.Output;
                    unlazyEffectDependent = node.Params.UnlazyEffectDependent;
                }

                if (xyzDependent)
                {
                    draws[len] = drawDescription.Draw;
                    zooms[len] = drawDescription.Zoom;
                    rotates[len] = drawDescription.Rotation;
                    mirrors[len] = drawDescription.Invert;
                    scaleDependents[len] = node.Params.ScaleDependent;
                    rotateDependents[len] = node.Params.RotateDependent;
                    mirrorDependents[len] = node.Params.MirrorDependent;
                    len++;
                    
                    xyzDependent = node.Params.XYZDependent;
                }

                if (rotateDependent)
                {
                    rotateM *=
                        Matrix4x4.CreateRotationZ(MathF.PI * drawDescription.Rotation.Z / 180f)
                        * Matrix4x4.CreateRotationY(MathF.PI * -drawDescription.Rotation.Y / 180f)
                        * Matrix4x4.CreateRotationX(MathF.PI * -drawDescription.Rotation.X / 180f);

                    rotateDependent = node.Params.RotateDependent;
                }

                if (opacityDependent)
                {
                    opacity *= drawDescription.Opacity;
                    opacityDependent = node.Params.OpacityDependent;
                }
                
                if (scaleDependent)
                {
                    zoom *= drawDescription.Zoom;
                    scaleDependent = node.Params.ScaleDependent;
                }

                if (cameraDependent)
                {
                    camera *= drawDescription.Camera;
                    cameraDependent = node.Params.CameraDependent;
                }

                zoomInterpolationMode = drawDescription.ZoomInterpolationMode;

                if (mirrorDependent)
                {
                    mirror ^= drawDescription.Invert;
                    mirrorDependent = node.Params.MirrorDependent;
                }
            }

            Matrix4x4 zoomM = Matrix4x4.Identity;
            Matrix4x4 rottM = Matrix4x4.Identity;
            bool mirror2 = false;

            for (int i = len - 1; i >= 0; i--)
            {
                Matrix4x4 m =
                    Matrix4x4.CreateTranslation(draws[i])
                    * zoomM
                    * (mirror2 ? Matrix4x4.CreateScale(-1f, 1f, 1f) : Matrix4x4.Identity)
                    * rottM;

                draw.X += m.M41;
                draw.Y += m.M42;
                draw.Z += m.M43;

                if (scaleDependents[i])
                {
                    zoomM *= Matrix4x4.CreateScale(new Vector3(zooms[i], 0));
                }
                else
                {
                    zoomM = Matrix4x4.CreateScale(new Vector3(zooms[i], 0));
                }

                if (mirrorDependents[i])
                {
                    mirror2 ^= mirrors[i];
                }
                else
                {
                    mirror2 = mirrors[i];
                }

                Matrix4x4 rm = Matrix4x4.CreateRotationZ(MathF.PI * rotates[i].Z / 180f)
                    * Matrix4x4.CreateRotationY(MathF.PI * -rotates[i].Y / 180f)
                    * Matrix4x4.CreateRotationX(MathF.PI * -rotates[i].X / 180f);

                if (rotateDependents[i])
                {
                    rottM *= rm;
                }
                else
                {
                    rottM = rm;
                }
            }

            

            cropEffect.SetInput(0, input2, true);
            Vector2 centerPoint = drawDescription.CenterPoint;
            AffineTransform2DInterpolationMode interPolationMode = drawDescription.ZoomInterpolationMode.ToTransform2D();
            Transform3DInterpolationMode interPolationMode2 = drawDescription.ZoomInterpolationMode.ToTransform3D();
            transform.InterPolationMode = interPolationMode;
            transform.TransformMatrix =
                Matrix3x2.CreateTranslation(-1 * new Vector2((float)Params.Center.X, (float)Params.Center.Y))
                * Matrix3x2.CreateScale((float)(zoom.X * Params.ExpXY.X / 100.0), (float)(zoom.Y * Params.ExpXY.Y / 100.0));
            renderEffect.InterPolationMode = interPolationMode2;
            renderEffect.TransformMatrix = (
                mirror ? Matrix4x4.CreateScale(-1f, 1f, 1f, new Vector3(centerPoint, 0f)) : Matrix4x4.Identity)
                * rotateM
                * Matrix4x4.CreateTranslation(draw)
                * camera
                * new Matrix4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, -0.001f, 0f, 0f, 0f, 1f);

            Apply(devices.DeviceContext);
            opacityEffect.Value = (float)opacity;
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
