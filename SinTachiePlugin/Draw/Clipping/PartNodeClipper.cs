using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Draw.Clipping
{
    internal class PartNodeClipper : IDisposable
    {
        public readonly PartNode Destination;
        private PartNodeClipper? _mask;
        private readonly IGraphicsDevicesAndContext _devices;
        private AlphaMask? _effect;
        public ID2D1Image Output { get; private set; }
        public bool OutputChanged { get; private set; } = true;
        private bool _isFirst = true;

        public PartNodeClipper(IGraphicsDevicesAndContext dc, PartNode ds)
        {
            _devices = dc;
            Destination = ds;
            Output = ds.Output;
        }

        /// <summary>
        /// クリッピングの更新
        /// </summary>
        /// <param name="ms">アルファマスクに使うもの。nullならDestinationをそのまま描画</param>
        public void Update(PartNodeClipper? ms)
        {
            if (_effect is null)
            {
                if (ms is null)
                {
                    if (Destination.OutputChanged)
                    {
                        Output = Destination.Output;
                        OutputChanged = true;
                    }
                    else
                    {
                        OutputChanged = _isFirst;
                        _isFirst = false;
                    }
                }
                else
                {
                    _effect = new(_devices.DeviceContext);
                    _effect.SetInput(0, Destination.Output, true);
                    _effect.SetInput(1, ms.Output, true);
                    _mask = ms;
                    Output = _effect.Output;
                    OutputChanged = true;
                }
            }
            else
            {
                if (ms is null)
                {
                    _effect.SetInput(0, null, true);
                    _effect.SetInput(1, null, true);
                    Output.Dispose();
                    _effect.Dispose();
                    _effect = null;
                    _mask = null;
                    Output = Destination.Output;
                    OutputChanged = true;
                }
                else
                {
                    OutputChanged = false;

                    if (Destination.OutputChanged)
                    {
                        _effect.SetInput(0, Destination.Output, true);
                    }

                    if (_mask != ms)
                    {
                        _mask = ms;
                        _effect.SetInput(1, ms.Output, true);
                    }
                    else if (ms.OutputChanged)
                    {
                        _effect.SetInput(1, ms.Output, true);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (_effect is not null)
            {
                _effect.SetInput(0, null, true);
                _effect.SetInput(1, null, true);
                Output.Dispose();
                _effect.Dispose();
                _effect = null;
            }
        }
    }
}
