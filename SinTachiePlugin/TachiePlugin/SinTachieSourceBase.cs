using SinTachiePlugin.Enums;
using SinTachiePlugin.Parts;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using System.Collections.Immutable;

namespace SinTachiePlugin.TachiePlugin
{
    internal abstract class SinTachieSourceBase
    {
        protected readonly DisposeCollector disposer = new();
        protected IGraphicsDevicesAndContext devices;

        //Listを再生成せずにClear()して使いまわす
        readonly List<PartNode> independent = [];
        readonly List<PartNode> parents = [];
        readonly List<PartNode> children = [];

        protected ID2D1CommandList? commandList = null;
        protected List<PartNode> PartNodes = [];
        protected readonly ID2D1Bitmap empty;

        protected bool isFirst = true;
        bool disposedValue = false;

        protected int numOfNodes = -1;

        public SinTachieSourceBase(IGraphicsDevicesAndContext devices)
        {
            this.devices = devices;
            empty = devices.DeviceContext.CreateEmptyBitmap();
            disposer.Collect(empty);
        }

        public void Dispose()
        {
            // 最後のUpdateで作成したCommandListを破棄
            if (!disposedValue)
            {
                // パーツノードを破棄
                foreach (var partnode in PartNodes)
                    partnode.Dispose();
                commandList?.Dispose();
                disposer.Dispose();
                GC.SuppressFinalize(this);
                disposedValue = true;
            }
        }

        protected void UpdateParentPaths()
        {
            independent.Clear();
            parents.Clear();
            children.Clear();

            foreach (var partNode in PartNodes)
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

        protected bool UpdateNodeList(List<(PartBlock block, FrameAndLength fl)> tupleList, int fps, double voiceVolume)
        {
            List<PartNode> disposedNodes = [.. PartNodes];
            Dictionary<int, List<PartNode>> BusNumDictionary = [];
            bool isOld = isFirst;
            foreach (var (block, fl) in tupleList)
            {
                PartNode newNode;
                if (PartNodes.Find(x => x.block == block) is PartNode node)
                {
                    disposedNodes.Remove(node);
                    isOld |= node.UpdateParams(fl, fps, voiceVolume);
                    newNode = node;
                }
                else
                {
                    newNode = new(devices, block, fl, fps, voiceVolume);
                    isOld = true;
                }
                if (BusNumDictionary.TryGetValue(newNode.BusNum, out var nodes)) nodes.Add(newNode);
                else BusNumDictionary[newNode.BusNum] = [newNode];
            }
            if (disposedNodes.Count > 0)
            {
                disposedNodes.ForEach(node => node.Dispose());
                isOld = true;
            }
            var sortedBusNums = BusNumDictionary.Keys.OrderBy(n => n);
            List<PartNode> newPartNodes = [];
            foreach (var busNum in sortedBusNums)
                newPartNodes.AddRange(BusNumDictionary[busNum]);
            if (!(isOld |= newPartNodes.Count != PartNodes.Count))
                for (int i = 0; i < PartNodes.Count; i++)
                    if (newPartNodes[i] != PartNodes[i])
                    {
                        isOld = true;
                        break;
                    }
            if (isOld) PartNodes = [.. newPartNodes];
            isFirst = false;
            return isOld;
        }

        protected void UpdateOutputs(TimelineItemSourceDescription description)
        {
            IEnumerable<PartNode> sortedPartNodes = [.. PartNodes.OrderBy(node => node.ParentPath.Count)];
            foreach (var cloneNode in sortedPartNodes)
                cloneNode.UpdateOutput(description);
        }

        protected void SetCommandList()
        {
            commandList?.Dispose();
            commandList = devices.DeviceContext.CreateCommandList();
            ID2D1DeviceContext6 dc = devices.DeviceContext;
            dc.Target = commandList;
            dc.BeginDraw();
            dc.Clear(null);
            if (PartNodes.Count == 0)
            {
                dc.DrawImage(empty, compositeMode: CompositeMode.SourceOver);
            }
            else
            {
                var glb = PartNodes.Where(x => x.Appear).ToLookup(
                    x => x.ZSortMode == ZSortMode.GlobalSpace && x.M43 != 0f ? x.M43 < 0f ? -1 : 1 : 0);
                foreach (var partNode in glb[-1].OrderBy(x => x.M43))
                    if (partNode.Output is ID2D1Image output)
                        DrawOrBlend(dc, partNode.BlendMode, output);

                foreach (var bus in glb[0].ToLookup(x => x.BusNum))
                {
                    var sub = bus.ToLookup(x => x.ZSortMode != ZSortMode.Ignore && x.M43 != 0f ? x.M43 < 0f ? -1 : 1 : 0);
                    foreach (var partNode in (ImmutableArray<PartNode>)[.. sub[-1], .. sub[0], .. sub[1]])
                        if (partNode.Output is ID2D1Image output)
                            DrawOrBlend(dc, partNode.BlendMode, output);
                }

                foreach (var partNode in glb[1].OrderBy(x => x.M43))
                    if (partNode.Output is ID2D1Image output)
                        DrawOrBlend(dc, partNode.BlendMode, output);
            }
            dc.EndDraw();
            commandList.Close();//CommandListはEndDraw()の後に必ずClose()を呼んで閉じる必要がある
        }

        static private void DrawOrBlend(ID2D1DeviceContext6 dc, BlendSTP blend, ID2D1Image output)
        {
            switch (blend)
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

        protected void RemoveNodes(int count)
        {
            while (PartNodes.Count > count)
            {
                PartNodes[count].Dispose();
                PartNodes.RemoveAt(count);
            }
        }
    }
}
