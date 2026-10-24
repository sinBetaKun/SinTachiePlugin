using System.Numerics;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw
{
    internal class PartNodeDrawer : IDisposable
    {
        private enum InputEffectType
        {
            Null,
            Trans2D,
            Trans3D,
            Opacity,
        }

        private readonly IGraphicsDevicesAndContext _devices;
        private readonly AffineTransform2D _trans2D;
        private Crop? _crop;
        private Transform3D? _trans3D;
        private Opacity? _opacity;
        private readonly ID2D1Image _trans2dOutput;
        private ID2D1Image? _trans3dOutput;
        private ID2D1Image? _opacityOutput;
        private readonly ID2D1Bitmap1 _empty;
        private ID2D1Image _input;
        public ID2D1Image Output;

        private InputEffectType _opacityInput = InputEffectType.Null;
        private InputEffectType _trans3DInput = InputEffectType.Null;
        private InputEffectType _outputObject = InputEffectType.Null;

        public PartNodeDrawer(IGraphicsDevicesAndContext devices)
        {
            _devices = devices;
            _empty = devices.DeviceContext.CreateEmptyBitmap();
            _trans2D = new(_devices.DeviceContext);
            _input = _empty;
            _trans2D.SetInput(0, _empty, true);
            _trans2dOutput = _trans2D.Output;
            Output = _empty;
        }

        public bool Update(ID2D1Image input, PartNodeDrawDescription desc)
        {
            bool ret = false;
            InputEffectType trans3DInput, opacityInput, outputObject;

            if (_input != input)
            {
                _input = input;
                _trans2D.SetInput(0, input, true);
            }

            if (desc.Opacity == 0.0f || desc.Zoom.X == 0.0f || desc.Zoom.Y == 0.0f)
            {
                trans3DInput = InputEffectType.Null;
                opacityInput = InputEffectType.Null;
                outputObject = InputEffectType.Null;
            }
            else
            {
                if (desc.Is2DTransform())
                {
                    trans3DInput = InputEffectType.Null;

                    if (desc.Opacity < 1.0f)
                    {
                        opacityInput = InputEffectType.Trans2D;
                        outputObject = InputEffectType.Opacity;
                    }
                    else
                    {
                        opacityInput = InputEffectType.Null;
                        outputObject = InputEffectType.Trans2D;
                    }
                }
                else
                {
                    trans3DInput = InputEffectType.Trans2D;

                    if (desc.Opacity < 1.0f)
                    {
                        opacityInput = InputEffectType.Trans3D;
                        outputObject = InputEffectType.Opacity;
                    }
                    else
                    {
                        opacityInput = InputEffectType.Null;
                        outputObject = InputEffectType.Trans3D;
                    }
                }
            }
            
            if (outputObject == InputEffectType.Null)
            {
                if (_outputObject != InputEffectType.Null)
                {
                    ret = true;
                    _trans3DInput = InputEffectType.Null;
                    _opacityInput = InputEffectType.Null;
                    _outputObject = InputEffectType.Null;
                    _trans2D.TransformMatrix = Matrix3x2.Identity;
                    Output = _empty;
                    DisposeOpacity();
                    Dispose3DTrans();
                }
            }
            else
            {
                if (_trans3DInput != trans3DInput)
                {
                    if (trans3DInput == InputEffectType.Null)
                    {
                        Dispose3DTrans();
                    }
                    else // InputEffectType.Trans2D
                    {
                        _trans3D = new(_devices.DeviceContext);
                        _crop = new(_devices.DeviceContext);
                        _crop.SetInput(0, _trans2dOutput, true);
                        _trans3D.SetInput(0, _crop.Output, true);
                        _trans3dOutput = _trans3D.Output;
                    }

                    _trans3DInput = trans3DInput;
                }

                if (_trans3D is null)
                {
                    AffineTransform2DInterpolationMode interPolationMode2d = desc.InterpolationMode.ToTransform2D();
                    _trans2D.InterPolationMode = interPolationMode2d;
                    Matrix4x4 rttM = desc.Rotate;
                    _trans2D.TransformMatrix = Matrix3x2.CreateTranslation(-1 * desc.Center)
                        * Matrix3x2.CreateScale(desc.Zoom)
                        * new Matrix3x2(rttM.M11, rttM.M12, rttM.M21, rttM.M22, desc.Draw.X, desc.Draw.Y);
                }
                else
                {
                    AffineTransform2DInterpolationMode interPolationMode2d = desc.InterpolationMode.ToTransform2D();
                    Transform3DInterpolationMode interPolationMode3d = desc.InterpolationMode.ToTransform3D();
                    _trans2D.InterPolationMode = interPolationMode2d;
                    _trans3D.InterPolationMode = interPolationMode3d;
                    _trans2D.TransformMatrix = Matrix3x2.CreateTranslation(-1 * desc.Center) * Matrix3x2.CreateScale(desc.Zoom);
                    _trans3D.TransformMatrix = desc.Rotate
                        * Matrix4x4.CreateTranslation(desc.Draw)
                        * desc.Camera
                        * new Matrix4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, -0.001f, 0f, 0f, 0f, 1f);
                    Apply();
                }

                if (_opacityInput != opacityInput)
                {
                    if (opacityInput == InputEffectType.Null)
                    {
                        if (_opacity is not null)
                            DisposeOpacity();
                    }
                    else
                    {
                        _opacity ??= new(_devices.DeviceContext);
                        _opacityOutput ??= _opacity.Output;

                        if (opacityInput == InputEffectType.Trans3D)
                        {
                            _opacity.SetInput(0, _trans3dOutput, true);
                        }
                        else // InputEffectType.Trans2D
                        {
                            _opacity.SetInput(0, _trans2dOutput, true);
                        }
                    }

                    _opacityInput = opacityInput;
                }

                _opacity?.Value = desc.Opacity;

                if (_outputObject != outputObject)
                {
                    switch (outputObject)
                    {
                        case InputEffectType.Trans2D:
                            Output = _trans2dOutput;
                            break;

                        case InputEffectType.Trans3D:
                            Output = _trans3dOutput!;
                            break;

                        case InputEffectType.Opacity:
                            Output = _opacityOutput!;
                            break;
                    }

                    ret = true;
                    _outputObject = outputObject;
                }
            }

            return ret;
        }

        private void Dispose3DTrans()
        {
            if (_trans3D is not null)
            {
                _trans3D.SetInput(0, null, true);
                _trans3dOutput?.Dispose();
                _trans3dOutput = null;
                _trans3D.Dispose();
                _trans3D = null;
            }

            if (_crop is not null)
            {
                _crop.SetInput(0, null, true);
                _crop.Dispose();
                _crop = null;
            }
        }

        private void DisposeOpacity()
        {
            if (_opacity is not null)
            {
                _opacity.SetInput(0, null, true);
                _opacityOutput?.Dispose();
                _opacity.Dispose();
                _opacity = null;
            }
        }

        public void Dispose()
        {
            DisposeOpacity();
            Dispose3DTrans();
            _trans2D.SetInput(0, null, true);
            _trans2dOutput.Dispose();
            _trans2D.Dispose();
            _empty.Dispose();
        }

        #region SafeTransform3DHelper
        const float D3D11_FTOI_INSTRUCTION_MAX_INPUT = 2.1474836E+09f;
        const float D3D11_FTOI_INSTRUCTION_MIN_INPUT = -2.1474836E+09f;

        private void Apply()
        {
            //transform3dエフェクトの出力画像1pxあたりの入力サイズが4096pxを超えるとエラーになる
            //エラー時には出力サイズがD3D11_FTOI_INSTRUCTION_MAX_INPUTになるため、cropエフェクトを使用し入力サイズを4096pxに制限する

            if (_crop is null || _trans3dOutput is null)
                return;

            //一旦cropエフェクトの範囲を初期化する
            _crop.Rectangle = new Vector4(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
            var renderBounds = _devices.DeviceContext.GetImageLocalBounds(_trans3dOutput);
            if (renderBounds.Left == D3D11_FTOI_INSTRUCTION_MIN_INPUT
                || renderBounds.Top == D3D11_FTOI_INSTRUCTION_MIN_INPUT
                || renderBounds.Right == D3D11_FTOI_INSTRUCTION_MAX_INPUT
                || renderBounds.Bottom == D3D11_FTOI_INSTRUCTION_MAX_INPUT)
            {
                //エラーの場合にのみ入力サイズを制限する
                _crop.Rectangle = new Vector4(-2048, -2048, 2048, 2048);
            }
        }
        #endregion
    }
}
