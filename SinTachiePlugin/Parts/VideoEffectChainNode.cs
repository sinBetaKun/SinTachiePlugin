using Vortice.Direct2D1.Effects;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Parts
{
    internal class VideoEffectChainNode : IDisposable
    {
        class SubChainNode(IVideoEffect effect, IVideoEffectProcessor proseccor, FrameAndLength fl)
        {
            public IVideoEffect Effect = effect;
            public IVideoEffectProcessor Proseccor = proseccor;
            public FrameAndLength Fl = fl;
            public ID2D1Image? Input;
        }

        readonly IGraphicsDevicesAndContext devices;
        readonly AffineTransform2D transform;
        readonly ID2D1Bitmap empty;
        readonly DisposeCollector disposer = new();
        bool wasEmpty;
        List<SubChainNode> Chain = [];

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

            Chain = [.. effects.Select(effect => new SubChainNode(effect, effect.CreateVideoEffect(devices), new FrameAndLength(fl)))];
        }

        public void UpdateChain(IEnumerable<IVideoEffect> effects, FrameAndLength fl)
        {
            // 使われなくなった映像エフェクトを見つけ出す。
            var disposedIndex = from e_ep in Chain
                                where !effects.Contains(e_ep.Effect)
                                select Chain.IndexOf(e_ep) into i
                                orderby i descending
                                select i;

            // 使われなくなった映像エフェクトを開放する。
            foreach (int index in disposedIndex)
            {
                SubChainNode node = Chain[index];
                node.Proseccor.ClearInput();
                node.Proseccor.Dispose();
                Chain.RemoveAt(index);
            }

            // 
            List<IVideoEffect> keeped = [.. Chain.Select((e_ep) => e_ep.Effect)];
            List<SubChainNode> newChain = new(effects.Count());
            foreach (var effect in effects)
            {
                int index = keeped.IndexOf(effect);
                if (index < 0)
                {
                    // 新しく使われる映像エフェクトの場合はタプルを新しく生成
                    newChain.Add(new(effect, effect.CreateVideoEffect(devices), fl));
                }
                else
                {
                    // すでに使われている映像エフェクトの場合は再使用
                    Chain[index].Fl = fl;
                    newChain.Add(Chain[index]);
                }
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
                if (tuple.Effect.IsEnabled)
                {
                    IVideoEffectProcessor item = tuple.Proseccor;
                    FrameAndLength fl = tuple.Fl;
                    if (tuple.Input != output) 
                        item.SetInput(output);
                    timeLineItemSourceDescription = new(timelineSourceDescription, fl.Frame, fl.Length, 0);
                    EffectDescription effectDescription = new(timeLineItemSourceDescription, result, 0, 1, 0, 1);
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
                        i.Proseccor.ClearInput();
                        i.Proseccor.Dispose();
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
