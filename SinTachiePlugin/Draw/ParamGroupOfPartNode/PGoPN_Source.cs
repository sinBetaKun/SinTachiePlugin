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
                ControlledParametersOfPart cp_0 = pvfls[i].PartValue.ControlledParameters;

                if (cp_0.SourceSelectArg is ImageFileParameter ifp_0)
                {
                    ThisLayerType = LayerType.Image;
                    FilePath = ifp_0.ImageFilePath;
                }
                else if (cp_0.SourceSelectArg is PsdFileParameter pfp_0)
                {
                    ThisLayerType = LayerType.Psd;
                    FilePath = pfp_0.PsdFileInfo.FilePath ?? string.Empty;
                    EnableLayers = pfp_0.PsdFileInfo.EnableLayers;
                }
                else if (cp_0.SourceSelectArg is VideoFileParameter vfp_0)
                {
                    ThisLayerType = LayerType.Video;
                    FilePath = vfp_0.VideoFilePath;
                    Time = TimeSpan.FromSeconds((vfp_0.StartFrameNumber + pvfls[i].FL.Frame * vfp_0.PlaybackSpeed / 100.0) / desc.FPS);
                    LoopPlayback = vfp_0.LoopPlayback;
                }
                else if (cp_0.SourceSelectArg is SceneParameter sp_0)
                {
                    ThisLayerType = LayerType.Scene;
                    SceneId = sp_0.SceneId;
                    Time = TimeSpan.FromSeconds((sp_0.StartFrameNumber + pvfls[i].FL.Frame * sp_0.PlaybackSpeed / 100.0) / desc.FPS);
                    LoopPlayback = sp_0.LoopPlayback;
                }
                else
                {
                    ThisLayerType = LayerType.Group;
                }

                if (!string.IsNullOrEmpty(FilePath) || SceneId != Guid.Empty)
                {
                    IsEmpty = false;
                    break;
                }
            }
        }

        public void CopyTo(PGoPN_Source pg)
        {
            pg.ThisLayerType = ThisLayerType;
            pg.IsEmpty = IsEmpty;
            pg.FilePath = FilePath;
            pg.EnableLayers = EnableLayers;
            pg.SceneId = SceneId;
            pg.Time = Time;
            pg.LoopPlayback = LoopPlayback;
        }

        public PartNodeComparateResult Comparate(PGoPN_Source pg)
        {
            bool valueChanged =
                pg.ThisLayerType != ThisLayerType ||
                pg.IsEmpty != IsEmpty ||
                pg.FilePath != FilePath ||
                !pg.EnableLayers.SequenceEqual(EnableLayers) ||
                pg.SceneId != SceneId ||
                pg.Time != Time ||
                pg.LoopPlayback != LoopPlayback;

            return new(Value: valueChanged);
        }
    }
}
