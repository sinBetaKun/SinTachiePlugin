using Vortice.Direct2D1.Effects;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.Tachie;
using SinTachiePlugin.Enums;
using System.Windows.Controls;
using System.Collections.Generic;
using SinTachiePlugin.Informations;
using YukkuriMovieMaker.Player.Video;
using System.Reflection.PortableExecutable;
using FrameTime = YukkuriMovieMaker.Commons.FrameTime;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace SinTachiePlugin.Parts
{
    internal class SinTachieSource : ITachieSource2
    {
        private IGraphicsDevicesAndContext devices;

        //Listを再生成せずにClear()して使いまわす
        private readonly List<PartNode> independent = [];
        private readonly List<PartNode> parents = [];
        private readonly List<PartNode> children = [];

        public ID2D1Image Output => output;

        ID2D1CommandList? commandList = null;
        List<PartNode> partNodes = [];
        readonly AffineTransform2D transformEffect;
        readonly ID2D1Image output;
        readonly ID2D1Bitmap empty;

        bool isFirst = true;

        int numOfNodes = -1;

        public SinTachieSource(IGraphicsDevicesAndContext devices)
        {
            this.devices = devices;

            empty = devices.DeviceContext.CreateEmptyBitmap();
            transformEffect = new AffineTransform2D(devices.DeviceContext);
            transformEffect.SetInput(0, empty, true);
            output = transformEffect.Output;//EffectからgetしたOutputは必ずDisposeする必要がある。Effect側では開放されない。
        }

        /// <summary>
        /// 表示を更新する
        /// </summary>
        public void Update(TachieSourceDescription description)
        {
            if (UpdateValuesOfNodes(description))
            {
                commandList?.Dispose();

                UpdateParentPaths();

                UpdateOutputs(description);

                SetCommandList();
            }
        }

        public void Dispose()
        {
            // パーツノードを破棄
            foreach (var partnode in partNodes)
                partnode.Dispose();
            // 最後のUpdateで作成したCommandListを破棄
            commandList?.Dispose();

            transformEffect.SetInput(0, null, true); // EffectのInputは必ずnullに戻す。
            output.Dispose(); // EffectからgetしたOutputは必ずDisposeする必要がある。Effect側では開放されない。
            transformEffect.Dispose();
            empty.Dispose();
        }

        private bool UpdateValuesOfNodes(TachieSourceDescription description)
        {
            var cp = description.Tachie.CharacterParameter as SinTachieCharacterParameter;
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


            List<PartBlock> partBlocks = ip.Parts.ToList();
            List<long> lengthList = new(partBlocks.Count);
            List<long> frameList = new(partBlocks.Count);
            List<int> busNums = new(partBlocks.Count);
            long lengthOfItem = description.ItemDuration.Frame;
            long frameOfItem = description.ItemPosition.Frame;
            int fps = description.FPS;
            double voiceVolume = description.VoiceVolume;
            if (voiceVolume < 0) voiceVolume = 0;

            for (int i = 0; i < partBlocks.Count; i++)
            {
                lengthList.Add(lengthOfItem);
                frameList.Add(frameOfItem);
                busNums.Add((int)partBlocks[i].BusNum.GetValue(lengthOfItem, frameOfItem, fps));
            }
            // 表情アイテムがnullでない時
            if (fdList != null)
            {
                // ディクショナリを使う
                var tagLookup = new Dictionary<string, int>();
                for (int i = 0; i < partBlocks.Count; i++)
                {
                    if (!string.IsNullOrEmpty(partBlocks[i].TagName))
                    {
                        tagLookup[partBlocks[i].TagName] = i;
                    }
                }
                int lengthOfParts = partBlocks.Count;
                foreach (var fd in fdList)
                {
                    if (fd.FaceParameter is SinTachieFaceParameter fp)
                    {
                        long lengthOfFace = fd.ItemDuration.Frame;
                        long frameOfFace = fd.ItemPosition.Frame;
                        // 表情アイテムのパーツで立ち絵アイテムと名前が重複するパーツを上書き・補足する
                        var tagsOfFace = (from node in fp.Parts select node.TagName).ToList();
                        for (int i = 0; i < tagsOfFace.Count; i++)
                        {
                            var part = fp.Parts[i];
                            if (string.IsNullOrEmpty(part.TagName)) continue;

                            string tag = tagsOfFace[i];
                            if (tagLookup.TryGetValue(part.TagName, out int index))
                            {
                                lengthList[index] = lengthOfFace;
                                frameList[index] = frameOfFace;
                                partBlocks[index] = part;
                            }
                            else
                            {
                                lengthList.Add(lengthOfFace);
                                frameList.Add(frameOfFace);
                                partBlocks.Add(part);
                                busNums.Add((int)part.BusNum.GetValue(lengthOfFace, frameOfFace, description.FPS));
                                tagLookup[part.TagName] = lengthOfParts++;  // タグ辞書をここでも更新しないと、表情アイテムの多段重ねの機能を実装できない。
                            }
                        }
                    }
                }
            }
            int numOfNodes = partBlocks.Count;
            var indexedBusNums = new (int busNum, int originalIndex)[numOfNodes];
            var countOfBusNum = new Dictionary<int, int>();
            for (int i = 0; i < numOfNodes; i++)
            {
                var busNum = busNums[i];
                if (countOfBusNum.ContainsKey(busNum)) countOfBusNum[busNum]++;
                else countOfBusNum[busNum] = 1;
                indexedBusNums[i] = (busNum, i);
            }

            Array.Sort(indexedBusNums, (a, b) => a.busNum.CompareTo(b.busNum));
            
            // ここのソートが不足していた。
            for (int pos = 0; pos < numOfNodes;)
            {
                int count = countOfBusNum[indexedBusNums[pos].busNum];
                Array.Sort(indexedBusNums, pos, count,
                    Comparer<(int busNum, int originalIndex)>.Create((a, b) =>
                    a.originalIndex.CompareTo(b.originalIndex)));
                pos += count;
            }

            List<long> sortedLengthList = new List<long>(numOfNodes);
            List<long> sortedFrameList = new List<long>(numOfNodes);
            List<PartBlock> sortedPartBlocks = new List<PartBlock>(numOfNodes);

            for (int i = 0; i < numOfNodes; i++)
            {
                int originalIndex = indexedBusNums[i].originalIndex;
                sortedLengthList.Add(lengthList[originalIndex]);
                sortedFrameList.Add(frameList[originalIndex]);
                sortedPartBlocks.Add(partBlocks[originalIndex]);
            }

            // そもそもパーツ数が異なる場合
            if (this.numOfNodes != numOfNodes || isFirst)
            {
                int numOfReloadNodes;
                if (this.numOfNodes < numOfNodes)
                {
                    //var frame = new FrameTime();
                    numOfReloadNodes = this.numOfNodes < 0 ? 0 : this.numOfNodes;
                    for (int i = numOfReloadNodes; i < numOfNodes; i++)
                        partNodes.Add(new PartNode(devices, /*description,*/ sortedPartBlocks[i],
                            sortedLengthList[i], sortedFrameList[i], fps, voiceVolume));
                }
                else
                {
                    RemoveNodes(numOfNodes);
                    numOfReloadNodes = numOfNodes;
                }

                for (int i = 0; i < numOfReloadNodes; i++)
                    partNodes[i].UpdateParams(sortedPartBlocks[i], sortedLengthList[i], sortedFrameList[i], fps, voiceVolume);
                this.numOfNodes = numOfNodes;

                isFirst = false;
                return true;
            }

            bool isOld = false;

            for (int i = 0; i < numOfNodes; i++)
            {
                if (partNodes[i].UpdateParams(sortedPartBlocks[i], sortedLengthList[i],
                    sortedFrameList[i], fps, voiceVolume))
                {
                    isOld = true;
                }
            }
            return isOld;
        }

        private void UpdateParentPaths()
        {
            independent.Clear();
            parents.Clear();
            children.Clear();

            foreach (var partNode in partNodes)
            {
                partNode.ParentPath = [partNode];
                if (partNode.TagName == partNode.Parent) independent.Add(partNode);
                else if (partNode.Parent == string.Empty) parents.Add(partNode);
                else children.Add(partNode);
            }
            int numOfChildren = children.Count;

            while (numOfChildren > 0)
            {

                for (int i = 0; i < children.Count;)
                {
                    var matched = from x in parents
                                  where x.TagName == children[i].Parent
                                  select x;

                    if (matched.Count() > 0)
                    {
                        children[i].ParentPath = children[i].ParentPath.AddRange(matched.First().ParentPath);
                        parents.Add(children[i]);
                        children.RemoveAt(i);
                    }
                    else i++;
                }

                if (numOfChildren == children.Count) break;
                numOfChildren = children.Count;
            }
        }

        private void UpdateOutputs(TachieSourceDescription description)
        {
            var sortedPartNodes = partNodes.OrderBy(node => node.ParentPath.Count);
            foreach (var cloneNode in sortedPartNodes)
                cloneNode.UpdateOutput(description);
        }

        private void SetCommandList()
        {
            if (numOfNodes == 0)
            {
                transformEffect.SetInput(0, empty, true);
                return;
            }

            commandList = devices.DeviceContext.CreateCommandList();
            var dc = devices.DeviceContext;
            dc.Target = commandList;
            dc.BeginDraw();
            dc.Clear(null);

            for (int i = 0; i < numOfNodes; i++)
            {
                if (partNodes[i].Appear)
                {
                    if (partNodes[i].Output is ID2D1Image output)
                    {
                        switch (partNodes[i].BlendMode)
                        {
                            case BlendSTP.SourceOver:
                                dc.DrawImage(output, compositeMode: CompositeMode.SourceOver);
                                break;

                            case BlendSTP.Plus:
                                dc.DrawImage(output, compositeMode: CompositeMode.Plus);
                                break;

                            case BlendSTP.DestinationOver:
                                dc.DrawImage(output, compositeMode: CompositeMode.DestinationOver);
                                break;

                            case BlendSTP.DestinationOut:
                                dc.DrawImage(output, compositeMode: CompositeMode.DestinationOut);
                                break;

                            case BlendSTP.SourceAtop:
                                dc.DrawImage(output, compositeMode: CompositeMode.SourceAtop);
                                break;

                            case BlendSTP.XOR:
                                dc.DrawImage(output, compositeMode: CompositeMode.Xor);
                                break;

                            case BlendSTP.MaskInverseErt:
                                dc.DrawImage(output, compositeMode: CompositeMode.MaskInverseErt);
                                break;

                            case BlendSTP.Multiply:
                                dc.BlendImage(output, BlendMode.Multiply, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Screen:
                                dc.BlendImage(output, BlendMode.Screen, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Darken:
                                dc.BlendImage(output, BlendMode.Darken, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Lighten:
                                dc.BlendImage(output, BlendMode.Lighten, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Dissolve:
                                dc.BlendImage(output, BlendMode.Dissolve, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.ColorBurn:
                                dc.BlendImage(output, BlendMode.ColorBurn, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.LinearBurn:
                                dc.BlendImage(output, BlendMode.LinearBurn, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.DarkerColor:
                                dc.BlendImage(output, BlendMode.DarkerColor, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.LighterColor:
                                dc.BlendImage(output, BlendMode.LighterColor, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.ColorDodge:
                                dc.BlendImage(output, BlendMode.ColorDodge, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.LinearDodge:
                                dc.BlendImage(output, BlendMode.LinearDodge, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Overlay:
                                dc.BlendImage(output, BlendMode.Overlay, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.SoftLight:
                                dc.BlendImage(output, BlendMode.SoftLight, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.HardLight:
                                dc.BlendImage(output, BlendMode.HardLight, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.VividLight:
                                dc.BlendImage(output, BlendMode.VividLight, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.LinearLight:
                                dc.BlendImage(output, BlendMode.LinearLight, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.PinLight:
                                dc.BlendImage(output, BlendMode.PinLight, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.HardMix:
                                dc.BlendImage(output, BlendMode.HardMix, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Difference:
                                dc.BlendImage(output, BlendMode.Difference, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Exclusion:
                                dc.BlendImage(output, BlendMode.Exclusion, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Hue:
                                dc.BlendImage(output, BlendMode.Hue, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Saturation:
                                dc.BlendImage(output, BlendMode.Saturation, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Color:
                                dc.BlendImage(output, BlendMode.Color, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Luminosity:
                                dc.BlendImage(output, BlendMode.Luminosity, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Subtract:
                                dc.BlendImage(output, BlendMode.Subtract, null, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Division:
                                dc.BlendImage(output, BlendMode.Division, null, null, InterpolationMode.MultiSampleLinear);
                                break;
                        }

                    }
                }
            }

            dc.EndDraw();
            commandList.Close();//CommandListはEndDraw()の後に必ずClose()を呼んで閉じる必要がある
            transformEffect.SetInput(0, commandList, true);
        }

        private void RemoveNodes(int count)
        {
            while (partNodes.Count > count)
            {
                partNodes[count].Dispose();
                partNodes.RemoveAt(count);
            }
        }
    }
}
