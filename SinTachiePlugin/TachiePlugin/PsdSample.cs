using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using PsdParser;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.FileSource.Psd;
using static System.Net.Mime.MediaTypeNames;

namespace SinTachiePlugin.TachiePlugin
{
    internal class PsdSample
    {
        public void Parse(string path, IGraphicsDevicesAndContext devices)
        {
            if (File.Exists(path))
            {
                PsdFile psdFile = new(path);
                PsdFolder psdRoot = PsdFolder.Parse(psdFile);
                ImmutableList<string> defaultEnableLayers = ImmutableList.Create(new ReadOnlySpan<string>([.. psdRoot.GetEnableItems()]));
                psdRoot.SetEnableItems(defaultEnableLayers);
                DisposeCollector disposer = new();
                ID2D1Bitmap bitmap = PsdFileSourcePlugin.CreateBitmap(devices, psdRoot, psdFile.Header.Width, psdFile.Header.Height) ?? devices.DeviceContext.CreateEmptyBitmap();
                disposer.Collect(bitmap);
                AffineTransform2D centeringEffect = new (devices.DeviceContext);
                centeringEffect.SetInput(0, bitmap, true);
                centeringEffect.TransformMatrix = Matrix3x2.CreateTranslation((0f - bitmap.Size.Width) / 2f, (0f - bitmap.Size.Height) / 2f);
            }
        }
    }
}
