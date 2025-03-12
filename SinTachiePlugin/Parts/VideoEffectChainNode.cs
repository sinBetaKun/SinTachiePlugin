using Vortice.Direct2D1.Effects;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Parts
{
    internal class VideoEffectChainNode : IDisposable
    {
        readonly IGraphicsDevicesAndContext devices;
        readonly AffineTransform2D transform;
        readonly ID2D1Bitmap empty;
        readonly DisposeCollector disposer = new();
        bool wasEmpty;
        List<(IVideoEffect effect, IVideoEffectProcessor proseccer, FrameAndLength fl)> Chain = [];

        public ID2D1Image Output;

        public VideoEffectChainNode(IGraphicsDevicesAndContext devices, IEnumerable<IVideoEffect> effects, FrameAndLength fl)
        {
            this.devices = devices;
            transform = new AffineTransform2D(devices.DeviceContext);
            disposer.Collect(transform);

            empty = devices.DeviceContext.CreateEmptyBitmap();
            disposer.Collect(empty);

            Output = transform.Output;
            disposer.Collect(Output);

            wasEmpty = false;

            Chain = effects.Select(effect => (effect, effect.CreateVideoEffect(devices), new FrameAndLength(fl))).ToList();
        }

        public void UpdateChain(IEnumerable<IVideoEffect> effects, FrameAndLength fl)
        {
            var disposedIndex = from e_ep in Chain
                                where !effects.Contains(e_ep.effect)
                                select Chain.IndexOf(e_ep) into i
                                orderby i descending
                                select i;
            foreach (int index in disposedIndex)
            {
                (IVideoEffect effect, IVideoEffectProcessor processor, FrameAndLength fl) tuple = Chain[index];
                tuple.processor.ClearInput();
                tuple.processor.Dispose();
                Chain.RemoveAt(index);
            }

            List<IVideoEffect> keeped = Chain.Select((e_ep) => e_ep.effect).ToList();
            List<(IVideoEffect, IVideoEffectProcessor, FrameAndLength)> newChain = new(effects.Count());
            foreach (var effect in effects)
            {
                int index = keeped.IndexOf(effect);
                newChain.Add(index < 0 ? (effect, effect.CreateVideoEffect(devices), fl) : Chain[index] with { fl = fl});
            }

            Chain = newChain;
        }

        public DrawDescription UpdateOutputAndDescription(ID2D1Image? input, TimelineSourceDescription timelineSourceDescription, DrawDescription drawDescription)
        {
            TimelineItemSourceDescription timeLineItemSourceDescription;
            DrawDescription result = new(
                drawDescription.Draw,
                drawDescription.CenterPoint,
                drawDescription.Zoom,
                drawDescription.Rotation,
                drawDescription.Camera,
                drawDescription.ZoomInterpolationMode,
                drawDescription.Opacity,
                drawDescription.Invert,
                drawDescription.Controllers
                );

            if (input == null)
            {
                if (!wasEmpty)
                {
                    Output = empty;
                    wasEmpty = true;
                }
                return result;
            }

            ID2D1Image? output = input;
            foreach (var tuple in Chain)
            {
                if (tuple.effect.IsEnabled)
                {
                    IVideoEffectProcessor item = tuple.proseccer;
                    FrameAndLength fl = tuple.fl;
                    item.SetInput(output);
                    timeLineItemSourceDescription = new(timelineSourceDescription, fl.Frame, fl.Length, 0);
                    EffectDescription effectDescription = new(timeLineItemSourceDescription, result, 0, 1);
                    result = item.Update(effectDescription);
                    
                    output = item.Output;
                }
            }
            transform.SetInput(0, output, true);

            if (wasEmpty)
            {
                Output = transform.Output;
                wasEmpty = false;
            }

            return result;
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
                    Chain.ForEach(i =>
                    {
                        i.proseccer.ClearInput();
                        i.proseccer.Dispose();
                    });
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
