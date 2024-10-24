﻿using Vortice.Direct2D1.Effects;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.Tachie;
using SinTachiePlugin.Parts;
using SinTachiePlugin.Enums;
using Windows.Win32.UI.KeyboardAndMouseInput;
using System.Windows.Controls;
using System.Collections.Generic;
using SinTachiePlugin.Informations;
using YukkuriMovieMaker.Player.Video;
using System.Reflection.PortableExecutable;

namespace SinTachiePlugin.Parts
{
    internal class SinTachieSource : ITachieSource2
    {
        private IGraphicsDevicesAndContext devices;

        public ID2D1Image Output => output;

        ID2D1CommandList? commandList = null;
        List<PartNode> partNodes = [];
        readonly AffineTransform2D transformEffect;
        readonly ID2D1Image output;

        bool isFirst = true;

        int numOfNodes = -1;
        int numOfParts_Face = 0;

        public SinTachieSource(IGraphicsDevicesAndContext devices)
        {
            this.devices = devices;

            //Outputのインスタンスを固定するために、間にエフェクトを挟む
            transformEffect = new AffineTransform2D(devices.DeviceContext);
            output = transformEffect.Output;//EffectからgetしたOutputは必ずDisposeする必要がある。Effect側では開放されない。
        }

        /// <summary>
        /// 表示を更新する
        /// </summary>
        public void Update(TachieSourceDescription description)
        {

            if (updateValuesOfNodes(description))
            {
                commandList?.Dispose();

                UpdateParentPaths();

                updateOutputs();

                setCommandList();
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
        }

        private bool updateValuesOfNodes(TachieSourceDescription description)
        {
            var cp = description.Tachie.CharacterParameter as SinTachieCharacterParameter;
            var ip = description.Tachie.ItemParameter as SinTachieItemParameter;
            var fdList = description.Tachie.Faces.ToList();
            fdList.Reverse();
            List<long> lengthList = [];
            List<long> frameList = [];
            long lengthOfItem = description.ItemDuration.Frame;
            long frameOfItem = description.ItemPosition.Frame;
            int fps = description.FPS;
            double voiceVolume = description.VoiceVolume;
            if (voiceVolume < 0) voiceVolume = 0;

            // 立ち絵アイテムがnullの時
            if (ip?.Parts == null)
            {
                if (this.numOfNodes < 0)
                    return false;

                RemoveNodes(0);
                this.numOfNodes = -1;
                return true;
            }

            List<PartBlock> partBlocks = ip.Parts.ToList();
            for (int i = 0; i < partBlocks.Count; i++)
            {
                lengthList.Add(lengthOfItem);
                frameList.Add(frameOfItem);
            }
            // 表情アイテムがnullでない時
            if (fdList != null)
            {
                foreach (var fd in fdList)
                {
                    long lengthOfFace = fd.ItemDuration.Frame;
                    long frameOfFace = fd.ItemPosition.Frame;
                    var tagsOfTmp = (from node in partBlocks select node.TagName).ToList();
                    if (fd.FaceParameter is SinTachieFaceParameter fp)
                    {
                        // 表情アイテムのパーツで立ち絵アイテムと名前が重複するパーツを上書き・補足する
                        var tagsOfFace = (from node in fp.Parts select node.TagName).ToList();
                        for (int i = 0; i < tagsOfFace.Count; i++)
                        {
                            string tag = tagsOfFace[i];
                            int index = tagsOfTmp.IndexOf(tag);
                            if (index < 0 || string.IsNullOrEmpty(tag))
                            {
                                lengthList.Add(lengthOfFace);
                                frameList.Add(frameOfFace);
                                partBlocks.Add(fp.Parts[i]);
                            }
                            else
                            {
                                lengthList[index] = lengthOfFace;
                                frameList[index] = frameOfFace;
                                partBlocks[index] = fp.Parts[i];
                                tagsOfTmp[index] = null;
                            }
                        }
                    }
                }
            }
            int numOfNodes = partBlocks.Count;
            /*List<int> busNums = (from block in partBlocks
                             select block.GetBusNum(lengthOfItem, frameOfItem, fps)).ToList();*/
            List<int> busNums = (from block in partBlocks
                                 select (int)block.BusNum.GetValue(lengthOfItem, frameOfItem, fps)).ToList();
            int[] sortedBusNums = busNums.ToArray();
            Array.Sort(sortedBusNums);
            List<long> sortedLengthList = [];
            List<long> sortedFrameList = [];
            List<PartBlock> sortedPartBlocks = [];
            for (int i = 0; i < numOfNodes; i++)
            {
                var busNum = sortedBusNums[i];
                var index = busNums.IndexOf(busNum);
                sortedLengthList.Add(lengthList[index]);
                sortedFrameList.Add(frameList[index]);
                sortedPartBlocks.Add(partBlocks[index]);
                busNums.RemoveAt(index);
                lengthList.RemoveAt(index);
                frameList.RemoveAt(index);
                partBlocks.RemoveAt(index);
            }

            // そもそもパーツ数が異なる場合
            if (this.numOfNodes != numOfNodes || isFirst)
            {
                int numOfReloadNodes;
                if (this.numOfNodes < numOfNodes)
                {
                    numOfReloadNodes = this.numOfNodes < 0 ? 0 : this.numOfNodes;
                    for (int i = numOfReloadNodes; i < numOfNodes; i++)
                        partNodes.Add(new PartNode(devices, sortedPartBlocks[i], sortedLengthList[i], sortedFrameList[i], fps, voiceVolume));
                }
                else
                {
                    RemoveNodes(numOfNodes);
                    numOfReloadNodes = numOfNodes;
                }

                for (int i = 0; i < numOfReloadNodes; i++)
                    partNodes[i].Update(sortedPartBlocks[i], sortedLengthList[i], sortedFrameList[i], fps, voiceVolume);
                this.numOfNodes = numOfNodes;

                isFirst = false;
                return true;
            }

            bool isOld = false;

            for (int i = 0; i < numOfNodes; i++)
            {
                if (partNodes[i].Update(sortedPartBlocks[i], sortedLengthList[i], sortedFrameList[i], fps, voiceVolume))
                {
                    isOld = true;
                }
            }
            return isOld;
        }

        private void UpdateParentPaths()
        {
            List<PartNode> independent = [];
            List<PartNode> parents = [];
            List<PartNode> children = [];
            int numOfChildren = 0;

            foreach (var PartNode in partNodes)
            {
                PartNode.ParentPath = [];
                if (PartNode.Parent == string.Empty)
                {
                    if (PartNode.TagName == string.Empty || PartNode.TagName == PartNode.Parent) independent.Add(PartNode);
                    else parents.Add(PartNode);
                }
                else
                {
                    children.Add(PartNode);
                    numOfChildren++;
                }
            }

            while (numOfChildren > 0)
            {

                for (int i = 0; i < children.Count;)
                {
                    var matched = from x in parents
                                  where x.TagName == children[i].Parent
                                  select x;

                    if (matched.Count() > 0)
                    {
                        children[i].ParentPath = matched.First().ParentPath.Add(matched.First());
                        parents.Add(children[i]);
                        children.RemoveAt(i);
                    }
                    else i++;
                }

                if (numOfChildren == children.Count) break;
                numOfChildren = children.Count;
            }
        }

        private void updateOutputs()
        {
            foreach (var PartNode in partNodes)
                PartNode.CommitOutput();

        }

        private void setCommandList()
        {
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
                        var vec2 = partNodes[i].Shift;
                        switch (partNodes[i].BlendMode)
                        {
                            case BlendSTP.SourceOver:
                                dc.DrawImage(output, vec2, compositeMode: CompositeMode.SourceOver);
                                break;

                            case BlendSTP.Plus:
                                dc.DrawImage(output, vec2, compositeMode: CompositeMode.Plus);
                                break;

                            case BlendSTP.DestinationOver:
                                dc.DrawImage(output, vec2, compositeMode: CompositeMode.DestinationOver);
                                break;

                            case BlendSTP.DestinationOut:
                                dc.DrawImage(output, vec2, compositeMode: CompositeMode.DestinationOut);
                                break;

                            case BlendSTP.SourceAtop:
                                dc.DrawImage(output, vec2, compositeMode: CompositeMode.SourceAtop);
                                break;

                            case BlendSTP.XOR:
                                dc.DrawImage(output, vec2, compositeMode: CompositeMode.Xor);
                                break;

                            case BlendSTP.MaskInverseErt:
                                dc.DrawImage(output, vec2, compositeMode: CompositeMode.MaskInverseErt);
                                break;

                            case BlendSTP.Multiply:
                                dc.BlendImage(output, BlendMode.Multiply, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Screen:
                                dc.BlendImage(output, BlendMode.Screen, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Darken:
                                dc.BlendImage(output, BlendMode.Darken, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Lighten:
                                dc.BlendImage(output, BlendMode.Lighten, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Dissolve:
                                dc.BlendImage(output, BlendMode.Dissolve, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.ColorBurn:
                                dc.BlendImage(output, BlendMode.ColorBurn, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.LinearBurn:
                                dc.BlendImage(output, BlendMode.LinearBurn, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.DarkerColor:
                                dc.BlendImage(output, BlendMode.DarkerColor, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.LighterColor:
                                dc.BlendImage(output, BlendMode.LighterColor, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.ColorDodge:
                                dc.BlendImage(output, BlendMode.ColorDodge, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.LinearDodge:
                                dc.BlendImage(output, BlendMode.LinearDodge, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Overlay:
                                dc.BlendImage(output, BlendMode.Overlay, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.SoftLight:
                                dc.BlendImage(output, BlendMode.SoftLight, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.HardLight:
                                dc.BlendImage(output, BlendMode.HardLight, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.VividLight:
                                dc.BlendImage(output, BlendMode.VividLight, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.LinearLight:
                                dc.BlendImage(output, BlendMode.LinearLight, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.PinLight:
                                dc.BlendImage(output, BlendMode.PinLight, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.HardMix:
                                dc.BlendImage(output, BlendMode.HardMix, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Difference:
                                dc.BlendImage(output, BlendMode.Difference, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Exclusion:
                                dc.BlendImage(output, BlendMode.Exclusion, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Hue:
                                dc.BlendImage(output, BlendMode.Hue, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Saturation:
                                dc.BlendImage(output, BlendMode.Saturation, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Color:
                                dc.BlendImage(output, BlendMode.Color, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Luminosity:
                                dc.BlendImage(output, BlendMode.Luminosity, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Subtract:
                                dc.BlendImage(output, BlendMode.Subtract, vec2, null, InterpolationMode.MultiSampleLinear);
                                break;

                            case BlendSTP.Division:
                                dc.BlendImage(output, BlendMode.Division, vec2, null, InterpolationMode.MultiSampleLinear);
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
