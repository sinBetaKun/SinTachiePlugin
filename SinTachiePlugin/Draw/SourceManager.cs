using SinTachiePlugin.Draw.DataAndOutput;
using SinTachiePlugin.Enums;
using System.IO; 
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Draw
{
    internal class SourceManager : IDisposable
    {
        private readonly IGraphicsDevicesAndContext _devices;
        private List<ImageDataAndOutput> _images = [];
        private List<PsdDataAndOutput> _psds = [];
        private List<VideoDataAndOutput> _videos = [];
        private List<SceneDataAndOutput> _scenes = [];

        public SourceManager(IGraphicsDevicesAndContext devices)
        {
            _devices = devices;
        }

        public void Assign()

        public void Dispose()
        {
            foreach (ImageDataAndOutput image in _images)
                image.Dispose();

            foreach (PsdDataAndOutput psd in _psds)
                psd.Dispose();

            foreach (VideoDataAndOutput video in _videos)
                video.Dispose();

            foreach (SceneDataAndOutput scene in _scenes)
                scene.Dispose();
        }
    }
}
