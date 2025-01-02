using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct2D1.Effects;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Parts
{
    internal class VideoEffectChainNode
    {
        readonly IGraphicsDevicesAndContext devices;
        readonly AffineTransform2D transform;
        readonly ID2D1Bitmap empty;
        readonly DisposeCollector disposer = new();
        bool wasEmpty;
        bool disposedValue = false;
        List<(IVideoEffect, IVideoEffectProcessor)> Chain;

        public ID2D1Image Output;

        public VideoEffectChainNode(IGraphicsDevicesAndContext devices, IEnumerable<IVideoEffect> effects)
        {
            this.devices = devices;
            transform = new AffineTransform2D(devices.DeviceContext);
            disposer.Collect(transform);

            empty = devices.DeviceContext.CreateEmptyBitmap();
            disposer.Collect(empty);

            Output = transform.Output;
            disposer.Collect(Output);

            wasEmpty = false;

            Chain = effects.Select(effect => (effect, effect.CreateVideoEffect(devices))).ToList();
        }

        public void UpdateChain(IEnumerable<IVideoEffect> effects)
        {
            var disposedIndex = from e_ep in Chain
                                where !effects.Contains(e_ep.Item1)
                                select Chain.IndexOf(e_ep) into i
                                orderby i descending
                                select i;
            foreach (int index in disposedIndex)
            {
                (IVideoEffect, IVideoEffectProcessor) tuple = Chain[index];
                tuple.Item2.ClearInput();
                tuple.Item2.Dispose();
                Chain.RemoveAt(index);
            }

            List<IVideoEffect> keeped = Chain.Select((e_ep) => e_ep.Item1).ToList();
            List<(IVideoEffect, IVideoEffectProcessor)> newChain = new(effects.Count());
            foreach (var effect in effects)
            {
                int index = keeped.IndexOf(effect);
                newChain.Add(index < 0 ? (effect, effect.CreateVideoEffect(devices)) : Chain[index]);
            }

            Chain = newChain;
        }

        public DrawDescription UpdateOutputAndDescription(ID2D1Image? input, TimelineItemSourceDescription timeLineItemSourceDescription, DrawDescription drawDescription)
        {
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
                if (tuple.Item1.IsEnabled)
                {
                    IVideoEffectProcessor item = tuple.Item2;
                    item.SetInput(output);
                    EffectDescription effectDescription = new(timeLineItemSourceDescription, result, 0);
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

        public void Dispose()
        {
            if (!disposedValue)
            {
                Chain.ForEach(i =>
                {
                    i.Item2.ClearInput();
                    i.Item2.Dispose();
                });
                ClearEffectChain();
                disposer.Dispose();
                GC.SuppressFinalize(this);
                disposedValue = true;
            }
        }
    }
}
