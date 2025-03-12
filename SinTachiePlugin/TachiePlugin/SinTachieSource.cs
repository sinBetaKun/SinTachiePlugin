using Vortice.Direct2D1.Effects;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.Tachie;
using YukkuriMovieMaker.Player.Video;
using SinTachiePlugin.TachiePlugin;
using SinTachiePlugin.Enums;
using System.Linq;

namespace SinTachiePlugin.Parts
{
    internal class SinTachieSource : SinTachieSourceBase, ITachieSource2
    {
        public ID2D1Image Output => output;

        readonly AffineTransform2D transformEffect;
        readonly ID2D1Image output;

        public SinTachieSource(IGraphicsDevicesAndContext devices) : base(devices)
        {
            transformEffect = new(devices.DeviceContext);
            disposer.Collect(transformEffect);
            transformEffect.SetInput(0, empty, true);
            output = transformEffect.Output;
            disposer.Collect(output);
        }

        /// <summary>
        /// 表示を更新する
        /// </summary>
        public void Update(TachieSourceDescription description)
        {
            UpdateCase updateCase = UpdateNodeListForTachie(description);

            if (updateCase != UpdateCase.None)
            {
                if (updateCase.HasFlag(UpdateCase.BitmapParams))
                {
                    UpdateParentPaths();
                    UpdateOutputs(description);
                }
                SetCommandList(updateCase);

                transformEffect.SetInput(0, commandList, true);
            }
        }

        private UpdateCase UpdateNodeListForTachie(TachieSourceDescription description)
        {
            //var cp = description.Tachie.CharacterParameter as SinTachieCharacterParameter;

            // 立ち絵アイテムがnullの時
            if (description.Tachie.ItemParameter is not SinTachieItemParameter ip)
            {
                if (this.numOfNodes < 0)
                    return UpdateCase.None;

                RemoveNodes(0);
                this.numOfNodes = -1;
                return UpdateCase.Both;
            }

            // fps を取得
            int fps = description.FPS;

            // 再生位置における声の大きさ（セリフアイテムがないときは -1 が渡されるので、強制的に 0 にしています。）
            double voiceVolume = description.VoiceVolume;
            if (voiceVolume < 0) voiceVolume = 0;

            // 立ち絵アイテム上の再生位置とアイテムの長さを description から取得
            FrameAndLength initialFL = new(description);

            int count = description.Tachie.Faces
                .Select(face => (face.FaceParameter as SinTachieFaceParameter).Parts.Count)
                .Sum()
                + ip.Parts.Count;

            // 並び替えた後の
            PartBlock[] blocks = new PartBlock[count];

            FrameAndLength[] FLs = new FrameAndLength[count];

            int pos = 0;

            IOrderedEnumerable<int> layerNums = description.Tachie.Faces.Select(face => face.Layer).Append(description.Layer).OrderBy(i => i);
            foreach (var layerNum in layerNums)
            {
                if (description.Tachie.Faces.FirstOrDefault(face => face.Layer == layerNum) is TachieFaceDescription fd)
                {
                    FrameAndLength fl = new(
                        fd.ItemPosition.Frame,
                        fd.ItemDuration.Frame
                        );

                    var fp = fd.FaceParameter as SinTachieFaceParameter;

                    for (int i = 0; i < fp.Parts.Count; i++)
                    {
                        blocks[pos + i] = fp.Parts[i];
                        FLs[pos + i] = fl;
                    }

                    pos += fp.Parts.Count;
                }
                else
                {
                    FrameAndLength fl = new(
                        description.ItemPosition.Frame,
                        description.ItemDuration.Frame
                        );

                    for (int i = 0; i < ip.Parts.Count; i++)
                    {
                        blocks[pos + i] = ip.Parts[i];
                        FLs[pos + i] = fl;
                    }

                    pos += ip.Parts.Count;
                }
            }


            return UpdateNodeList(blocks, FLs, blocks.Length, fps, voiceVolume);
        }


    }
}
