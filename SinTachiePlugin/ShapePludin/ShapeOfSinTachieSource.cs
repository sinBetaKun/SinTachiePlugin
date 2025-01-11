using SinTachiePlugin.Enums;
using SinTachiePlugin.Parts;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.ShapePludin
{
    internal class ShapeOfSinTachieSource : IShapeSource
    {
        readonly IGraphicsDevicesAndContext devices;
        readonly ShapeParameterOfSinTachie param;

        public ID2D1Image Output => commandList ?? throw new InvalidOperationException("commandList is null");
        readonly ID2D1Bitmap empty; 

        int numOfNodes = -1;
        bool isFirst = true;
        List<PartNode> partNodes = [];
        List<PartNode> independent = [];
        List<PartNode> parents = [];
        List<PartNode> children = [];


        ID2D1CommandList? commandList;

        public ShapeOfSinTachieSource(IGraphicsDevicesAndContext devices, ShapeParameterOfSinTachie param)
        {
            this.devices = devices;
            this.param = param;
            empty = devices.DeviceContext.CreateEmptyBitmap();
        }

        public void Dispose()
        {
            // パーツノードを破棄
            foreach (var partnode in partNodes)
                partnode.Dispose();
            // 最後のUpdateで作成したCommandListを破棄
            commandList?.Dispose();
            empty.Dispose();
        }

        public void Update(TimelineItemSourceDescription description)
        {
            if (updateValuesOfNodes(description))
            {
                commandList?.Dispose();

                UpdateParentPaths();

                UpdateOutputs(description);

                SetCommandList();
            }
        }

        private bool updateValuesOfNodes(TimelineItemSourceDescription description)
        {
            var parts = param.PartsAndRoot.Parts.ToList();
            var length = description.ItemDuration.Frame;
            var frame = description.ItemPosition.Frame;
            var fps = description.FPS;

            // 立ち絵アイテムがnullの時
            if (parts == null)
            {
                if (this.numOfNodes < 0)
                    return false;

                RemoveNodes(0);
                this.numOfNodes = -1;
                return true;
            }

            int numOfNodes = parts.Count;
            List<int> busNums = (from block in parts
                                 select (int)block.BusNum.GetValue(length, frame, fps)).ToList();
            int[] sortedBusNums = busNums.ToArray();
            Array.Sort(sortedBusNums);
            List<PartBlock> sortedparts = [];
            for (int i = 0; i < numOfNodes; i++)
            {
                var busNum = sortedBusNums[i];
                var index = busNums.IndexOf(busNum);
                sortedparts.Add(parts[index]);
                busNums.RemoveAt(index);
                parts.RemoveAt(index);
            }

            // そもそもパーツ数が異なる場合
            if (this.numOfNodes != numOfNodes || isFirst)
            {
                int numOfReloadNodes;
                if (this.numOfNodes < numOfNodes)
                {
                    numOfReloadNodes = this.numOfNodes < 0 ? 0 : this.numOfNodes;
                    for (int i = numOfReloadNodes; i < numOfNodes; i++)
                        partNodes.Add(new PartNode(devices, sortedparts[i], length, frame, fps, 0));
                }
                else
                {
                    RemoveNodes(numOfNodes);
                    numOfReloadNodes = numOfNodes;
                }

                for (int i = 0; i < numOfReloadNodes; i++)
                    partNodes[i].UpdateParams(sortedparts[i], length, frame, fps, 0);
                this.numOfNodes = numOfNodes;

                isFirst = false;
                return true;
            }

            bool isOld = false;

            for (int i = 0; i < numOfNodes; i++)
            {
                if (partNodes[i].UpdateParams(sortedparts[i], length, frame, fps, 0))
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

        private void UpdateOutputs(TimelineItemSourceDescription description)
        {
            var sortedPartNodes = partNodes.OrderBy(node => node.ParentPath.Count);
            foreach (var cloneNode in sortedPartNodes)
                cloneNode.UpdateOutput(description);
        }

        private void SetCommandList()
        {
            commandList = devices.DeviceContext.CreateCommandList();
            var dc = devices.DeviceContext;
            dc.Target = commandList;
            dc.BeginDraw();
            dc.Clear(null);

            if (numOfNodes == 0)
            {
                dc.DrawImage(empty, compositeMode: CompositeMode.SourceOver);
            }
            else
            {
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
            }

            dc.EndDraw();
            commandList.Close();//CommandListはEndDraw()の後に必ずClose()を呼んで閉じる必要がある
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
