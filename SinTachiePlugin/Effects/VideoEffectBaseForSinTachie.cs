using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Effects
{
    public abstract class VideoEffectBaseForSinTachie : Animatable, IDisposable
    {
        public abstract string Label { get; }
        public abstract ID2D1Image? Output { get; }

        public abstract VideoEffectBaseForSinTachie Clone();

        public abstract void Dispose();

        public abstract DrawDescriptionForSinTachie Update(DrawDescriptionForSinTachie effectDescription);

        public abstract void SetInput(ID2D1Image? input);

        public abstract void ClearInput();
    }
}
