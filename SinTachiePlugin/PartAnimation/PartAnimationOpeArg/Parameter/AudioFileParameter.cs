using NAudio.Wave;
using SinTachiePlugin.Draw;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.Abrir;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.AudioFilePath;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.Cerrar;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.PlaybackSpeed;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.StartFrameNumber;
using SinTachiePlugin.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter
{
    internal class AudioFileParameter : PartAnimationOpeArgBase, IAbrirParameter, ICerrarParameter, IAudioFilePathParameter, IStartFrameNumberParameter, IPlaybackSpeedParameter
    {
        [Display(Name = nameof(TextResource.PartAnimationOpeArg_Open), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Abrir { get; } = new Animation(100, -10000, 10000);

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_Close), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Cerrar { get; } = new Animation(0, -10000, 10000);

        [Display(Name = nameof(TextResource.PartAnimationOpeMode_AudioFile), ResourceType = typeof(TextResource))]
        [FileSelector(YukkuriMovieMaker.Settings.FileGroupType.AudioItem)]
        public string AudioFilePath { get => audioFilePath; set => Set(ref audioFilePath, value); }
        private string audioFilePath = string.Empty;

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_StartFrameNumber), ResourceType = typeof(TextResource))]
        [FrameNumberEditor]
        [DefaultValue(0)]
        [Range(0, 99999)]
        public int StartFrameNumber { get => startFrameNumber; set => Set(ref startFrameNumber, value); }
        private int startFrameNumber = 0;

        [Display(Name = nameof(TextResource.PartAnimationOpeArg_PlaybackSpeed), ResourceType = typeof(TextResource))]
        [TextBoxSlider("F2", "%", 0, 200)]
        [DefaultValue(100)]
        [Range(0, 99999)]
        public double PlaybackSpeed { get => playbackSpeed; set => Set(ref playbackSpeed, value); }
        private double playbackSpeed = 100;

        private int lastFrame = 0;
        private int length = 0;
        private int fps = 0;
        private double[] volumeArray = [];

        public AudioFileParameter()
        {
        }

        public AudioFileParameter(SharedDataStore? store) : base(store)
        {
        }

        public override PartAnimationResultA GetResult(TachieSourceDescription desc)
        {
            FrameAndLength fl = new(desc);

            if (lastFrame != StartFrameNumber || length != fl.Length || fps != desc.FPS)
            {
                lastFrame = StartFrameNumber;
                length = fl.Length;
                fps = desc.FPS;
                UpdateVolumeArray();
            }

            double open = fl.GetValue(Abrir, fps);
            double close = fl.GetValue(Cerrar, fps);

            return PartAnimationResultA.FromVolume(close + (open - close) * volumeArray[(int)(fl.Frame * PlaybackSpeed / 100)]);
        }

        public override void CopyFrom(PartAnimationOpeArgBase? origin)
        {
            if (origin is IAbrirParameter abrirParam)
                Abrir.CopyFrom(abrirParam.Abrir);
            if (origin is ICerrarParameter cerrarParam)
                Cerrar.CopyFrom(cerrarParam.Cerrar);
            if (origin is IAudioFilePathParameter audioFilePathParam)
                AudioFilePath = audioFilePathParam.AudioFilePath;
            if (origin is IStartFrameNumberParameter startFrameNumberParam)
                StartFrameNumber = startFrameNumberParam.StartFrameNumber;
            if (origin is IPlaybackSpeedParameter playbackSpeedParameter)
                PlaybackSpeed = playbackSpeedParameter.PlaybackSpeed;
        }

        public override PartAnimationOpeArgBase GetClone()
        {
            AudioFileParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Abrir, Cerrar];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new AbrirSharedData(this));
            store.Save(new CerrarSharedData(this));
            store.Save(new AudioFilePathSharedData(this));
            store.Save(new StartFrameNumberSharedData(this));
            store.Save(new PlaybackSpeedSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<AbrirSharedData>() is AbrirSharedData abrirSharedData)
                abrirSharedData.CopyTo(this);
            if (store.Load<CerrarSharedData>() is CerrarSharedData cerrarSharedData)
                cerrarSharedData.CopyTo(this);
            if (store.Load<AudioFilePathSharedData>() is AudioFilePathSharedData audioFilePathSharedData)
                audioFilePathSharedData.CopyTo(this);
            if (store.Load<StartFrameNumberSharedData>() is StartFrameNumberSharedData startTimeSpanSharedData)
                startTimeSpanSharedData.CopyTo(this);
            if (store.Load<PlaybackSpeedSharedData>() is PlaybackSpeedSharedData playbackSpeedSharedData)
                playbackSpeedSharedData.CopyTo(this);
        }

        /// <summary>
        /// 指定されている音声ファイルから、アイテムの長さ（単位：フレーム）だけ平均 dB を求めて配列に格納する。
        /// </summary>
        private void UpdateVolumeArray()
        {
            volumeArray = new double[length];

            using (var reader = new AudioFileReader(AudioFilePath))
            {
                int channels = reader.WaveFormat.Channels;
                int sampleRate = reader.WaveFormat.SampleRate;

                for (int frame = 0; frame < length; frame++)
                {
                    TimeSpan start = TimeSpan.FromSeconds((double)StartFrameNumber / fps) + TimeSpan.FromSeconds((double)frame / fps);
                    TimeSpan end = start + TimeSpan.FromSeconds(1.0 / fps);
                    double ave = GetAverageVolume(reader, start, end, channels, sampleRate);

                    if (ave < 0)
                    {
                        Array.Clear(volumeArray, frame, length - frame);
                        break;
                    }

                    volumeArray[frame] = ave;
                }
            }
        }

        /// <summary>
        /// 音声ファイルのリーダーを使って、区間内の平均 dB を計算する。
        /// </summary>
        /// <param name="reader">音声ファイルのリーダー</param>
        /// <param name="start">区間の開始位置</param>
        /// <param name="end">区間の終了位置</param>
        /// <param name="channels">チャンネル数</param>
        /// <param name="sampleRate">サンプリングレート</param>
        /// <returns>平均 dB</returns>
        private static double GetAverageVolume(AudioFileReader reader, TimeSpan start, TimeSpan end, int channels, int sampleRate)
        {
            // 開始位置にシーク
            reader.CurrentTime = start;
            int samplesNeeded = (int)((end - start).TotalSeconds * sampleRate * channels);

            float[] buffer = new float[samplesNeeded];
            int samplesRead = reader.Read(buffer, 0, samplesNeeded);

            if (samplesRead == 0)
                return -1;

            double sumSquares = 0;

            for (int i = 0; i < samplesRead; i++)
                sumSquares += buffer[i] * buffer[i];

            return Math.Sqrt(sumSquares / samplesNeeded); // RMS値
        }
    }
}
