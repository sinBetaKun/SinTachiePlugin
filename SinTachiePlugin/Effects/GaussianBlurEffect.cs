using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Effects
{
    public class GaussianBlurEffect : VideoEffectBaseForSinTachie
    {
        readonly IGraphicsDevicesAndContext devices;
        readonly GaussianBlur gaussian;


        public override string Label => "ぼかし";

        public override ID2D1Image? Output { get; }
        bool isFirst = true;

        public GaussianBlurEffect(IGraphicsDevicesAndContext devices)
        {
            this.devices = devices;
            gaussian = new GaussianBlur(devices.DeviceContext);
        }

        public override GaussianBlurEffect Clone()
        {
            throw new NotImplementedException();
        }

        public override void ClearInput()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            Output?.Dispose();
        }

        public override void SetInput(ID2D1Image? input)
        {
            throw new NotImplementedException();
        }

        public override DrawDescriptionForSinTachie Update(DrawDescriptionForSinTachie effectDescription)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IAnimatable> GetAnimatables()
        {
            throw new NotImplementedException();
        }
    }
}
