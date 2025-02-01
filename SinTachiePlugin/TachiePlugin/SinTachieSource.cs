using Vortice.Direct2D1.Effects;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.Tachie;
using YukkuriMovieMaker.Player.Video;
using SinTachiePlugin.TachiePlugin;

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
            if (UpdateNodeListForTachie(description))
            {
                UpdateParentPaths();

                UpdateOutputs(description);

                SetCommandList();

                transformEffect.SetInput(0, commandList, true);
            }
        }

        private bool UpdateNodeListForTachie(TachieSourceDescription description)
        {
            //var cp = description.Tachie.CharacterParameter as SinTachieCharacterParameter;
            var ip = description.Tachie.ItemParameter as SinTachieItemParameter;

            // 立ち絵アイテムがnullの時
            if (ip?.Parts == null)
            {
                if (this.numOfNodes < 0)
                    return false;

                RemoveNodes(0);
                this.numOfNodes = -1;
                return true;
            }

            var fdList = description.Tachie.Faces.ToList();
            fdList.Reverse();
            int fps = description.FPS;
            FrameAndLength initialFL = new(description);
            List<(PartBlock block, FrameAndLength fl)> tupleList =
                ip.Parts.Select(
                    x => (x, new FrameAndLength(initialFL))
                    ).ToList();
            double voiceVolume = description.VoiceVolume;
            if (voiceVolume < 0) voiceVolume = 0;

            // 表情アイテムがnullでない時
            if (fdList != null)
            {
                // ディクショナリを使う
                var tagLookup = new Dictionary<string, int>();
                int lengthOfParts = tupleList.Count;

                for (int i = 0; i < lengthOfParts; i++)
                    if (!string.IsNullOrEmpty(tupleList[i].block.TagName))
                        tagLookup[tupleList[i].block.TagName] = i;

                foreach (var fd in fdList)
                {
                    if (fd.FaceParameter is SinTachieFaceParameter fp)
                    {
                        int lengthOfFace = fd.ItemDuration.Frame;
                        int frameOfFace = fd.ItemPosition.Frame;
                        // 表情アイテムのパーツで立ち絵アイテムと名前が重複するパーツを上書き・補足する
                        var tagsOfFace = (from node in fp.Parts select node.TagName).ToList();
                        for (int i = 0; i < tagsOfFace.Count; i++)
                        {
                            PartBlock part = fp.Parts[i];
                            if (string.IsNullOrEmpty(part.TagName)) continue;

                            string tag = tagsOfFace[i];
                            if (tagLookup.TryGetValue(part.TagName, out int index))
                                tupleList[index] = 
                                    (part, tupleList[index].fl.Update(frameOfFace, lengthOfFace));
                            else
                            {
                                tupleList.Add((part, new (frameOfFace, lengthOfFace)));
                                tagLookup[part.TagName] = lengthOfParts++;  // タグ辞書をここでも更新しないと、表情アイテムの多段重ねの機能を実装できない。
                            }
                        }
                    }
                }
            }

            ///////////////

            /*int numOfNodes = tupleList.Count;
            var indexedBusNums = new (int busNum, int originalIndex)[numOfNodes];
            Dictionary<int, int> countOfBusNum = [];
            for (int i = 0; i < numOfNodes; i++)
            {
                var busNum = tupleList[i].busNum;
                if (countOfBusNum.TryGetValue(busNum, out int value)) countOfBusNum[busNum] = ++value;
                else countOfBusNum[busNum] = 1;
                indexedBusNums[i] = (busNum, i);
            }

            Array.Sort(indexedBusNums, (a, b) => a.busNum.CompareTo(b.busNum));
            
            for (int pos = 0; pos < numOfNodes;)
            {
                int count = countOfBusNum[indexedBusNums[pos].busNum];
                Array.Sort(indexedBusNums, pos, count,
                    Comparer<(int busNum, int originalIndex)>.Create((a, b) =>
                    a.originalIndex.CompareTo(b.originalIndex)));
                pos += count;
            }

            SortedTupleList.Clear();
            foreach (var originalIndex in indexedBusNums)
                SortedTupleList.Add(tupleList[originalIndex.originalIndex]);

            // そもそもパーツ数が異なる場合
            if (this.numOfNodes != numOfNodes || isFirst)
            {
                int numOfReloadNodes;
                if (this.numOfNodes < numOfNodes)
                {
                    numOfReloadNodes = this.numOfNodes < 0 ? 0 : this.numOfNodes;
                    foreach (var tpuple in SortedTupleList)
                        PartNodes.Add(new PartNode(devices, tpuple.block, tpuple.fl, fps, voiceVolume));
                }
                else
                {
                    RemoveNodes(numOfNodes);
                    numOfReloadNodes = numOfNodes;
                }

                for (int i = 0; i < numOfReloadNodes; i++)
                    PartNodes[i].UpdateParams(SortedTupleList[i].fl, fps, voiceVolume);
                this.numOfNodes = numOfNodes;

                isFirst = false;
                return true;
            }


            for (int i = 0; i < numOfNodes; i++)
                isOld |= PartNodes[i].UpdateParams(SortedTupleList[i].fl, fps, voiceVolume);*/
            return UpdateNodeList(tupleList, fps, voiceVolume);
        }
    }
}
