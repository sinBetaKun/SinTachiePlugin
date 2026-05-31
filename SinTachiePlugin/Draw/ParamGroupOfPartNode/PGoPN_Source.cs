using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using System.Collections.Immutable;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_Source
    {
        public LayerType ThisLayerType { get; private set; } = LayerType.Image;
        public bool IsEmpty { get; private set; }
        public string FilePath { get; private set; } = string.Empty;
        public ImmutableList<string> EnableLayers { get; private set; } = [];
        public Guid SceneId { get; private set; } = Guid.Empty;
        public TimeSpan Time { get; private set; }
        public bool LoopPlayback { get; private set; }

        public void Reset()
        {
            ThisLayerType = LayerType.Image;
            IsEmpty = true;
            FilePath = string.Empty;
            EnableLayers = [];
            SceneId = Guid.Empty;
            Time = TimeSpan.Zero;
            LoopPlayback = false;
        }

        public void Update(List<PartValueAndFL> pvfls, TachieSourceDescription desc)
        {
            IsEmpty = true;
            FilePath = string.Empty;
            SceneId = Guid.Empty;
            EnableLayers = [];

            for (int i = pvfls.Count - 1; i >= 0; i--)
            {
                ControlledParametersOfPart cp = pvfls[i].PartValue.ControlledParameters;

                switch (cp.ThisLayerType)
                {
                    case LayerType.Image:
                        ImageFileParameter ip = (ImageFileParameter)cp.SourceSelectArg;
                        FilePath = ip.ImageFilePath;
                        break;

                    case LayerType.Psd:
                        PsdFileParameter pp = (PsdFileParameter)cp.SourceSelectArg;
                        FilePath = pp.PsdFileInfo.FilePath ?? string.Empty;
                        EnableLayers = pp.PsdFileInfo.EnableLayers;
                        break;

                    case LayerType.Video:
                        VideoFileParameter vp = (VideoFileParameter)cp.SourceSelectArg;
                        FilePath = vp.VideoFilePath;
                        Time = TimeSpan.FromSeconds((vp.StartFrameNumber + pvfls[i].FL.Frame * vp.PlaybackSpeed / 100.0) / desc.FPS);
                        LoopPlayback = vp.LoopPlayback;
                        break;

                    case LayerType.Scene:
                        SceneParameter sp = (SceneParameter)cp.SourceSelectArg;
                        SceneId = sp.SceneId;
                        Time = TimeSpan.FromSeconds((sp.StartFrameNumber + pvfls[i].FL.Frame * sp.PlaybackSpeed / 100.0) / desc.FPS);
                        LoopPlayback = sp.LoopPlayback;
                        break;
                }

                if (!string.IsNullOrEmpty(FilePath) || SceneId != Guid.Empty)
                {
                    ThisLayerType = cp.ThisLayerType;
                    IsEmpty = false;
                    break;
                }
            }
        }
    }
}
