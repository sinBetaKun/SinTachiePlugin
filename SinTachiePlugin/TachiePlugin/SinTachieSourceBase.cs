using SinTachiePlugin.Enums;
using SinTachiePlugin.Parts;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using System.Collections.Immutable;
using System.Windows.Documents;

namespace SinTachiePlugin.TachiePlugin
{
    internal abstract class SinTachieSourceBase : IDisposable
    {
        protected readonly DisposeCollector disposer = new();
        protected IGraphicsDevicesAndContext devices;

        //Listを再生成せずにClear()して使いまわす
        readonly List<PartNode> independent = [];
        readonly List<PartNode> parents = [];
        readonly List<PartNode> children = [];
        readonly List<PartNode> preDrawList = [];
        readonly List<PartNode> drawlist = [];

        protected ID2D1CommandList? commandList = null;
        protected readonly ID2D1Bitmap empty;

        protected bool isFirst = true;
        protected int numOfNodes = -1;

        public SinTachieSourceBase(IGraphicsDevicesAndContext devices)
        {
            this.devices = devices;
            empty = devices.DeviceContext.CreateEmptyBitmap();
            disposer.Collect(empty);
        }

        protected void UpdateParentPaths()
        {
            independent.Clear();
            parents.Clear();
            children.Clear();

            foreach (var partNode in preDrawList)
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

                    if (matched.Any())
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

        protected UpdateCase UpdateNodeList(PartBlock[] blAr, FrameAndLength[] flAr, int arCount, int fps, double voiceVolume)
        {
            // タグの重複がない整理後のパーツ
            List<PartBlock> blLi = [];

            // タグの重複がない整理後の FL
            List<FrameAndLength> flLi = [];

            // 名前が重複するパーツの整理
            for (int i = 0; i < arCount; i++)
            {
                PartBlock block = blAr[i];
                FrameAndLength fl = flAr[i];

                // タグが空っぽのパーツは常に重複を許す。
                if (!string.IsNullOrEmpty(block.TagName))
                {
                    int index = blLi.FindIndex(block2 => block2.TagName == block.TagName);
                    if (index >= 0)
                    {
                        blLi[index] = block;
                        flLi[index] = fl;
                        continue;
                    }
                }

                blLi.Add(block);
                flLi.Add(fl);
            }

            // タグの重複がない整理後のパーツの数
            int liCount = blLi.Count;

            // Key: バス番号;   Value: バス内に存在するパーツ群
            Dictionary<int, List<PartNode>> BusNumDictionary = [];

            if (isFirst)
            {
                // 最初にこのメソッドが実行される時

                if (liCount > 0)
                {
                    // パーツが 1 つ以上あるとき

                    for (int i = 0; i < liCount; i++)
                    {
                        PartNode newNode = new(devices, blLi[i], flLi[i], fps, voiceVolume);

                        if (BusNumDictionary.TryGetValue(newNode.BusNum, out var nodes))
                        {
                            nodes.Add(newNode);
                        }
                        else
                        {
                            BusNumDictionary[newNode.BusNum] = [newNode];
                        }
                    }

                    foreach (var busNum in BusNumDictionary.Keys.OrderBy(n => n))
                    {
                        preDrawList.AddRange(BusNumDictionary[busNum]);
                    }
                        
                }

                isFirst = false;
                return UpdateCase.BitmapParams | UpdateCase.BlendMode;
            }

            if (preDrawList.Count > 0 && liCount == 0)
            {
                preDrawList.ForEach(node => node.Dispose());
                preDrawList.Clear();
                return UpdateCase.BlendMode;
            }

            UpdateCase updateCase = UpdateCase.None;
            List<PartNode> disposedNodes = [.. preDrawList];

            for (int i = 0; i < liCount; i++)
            {
                PartNode newNode;

                if (preDrawList.Find(x => x.block == blLi[i]) is PartNode node)
                {
                    disposedNodes.Remove(node);
                    updateCase |= node.UpdateParams(flLi[i], fps, voiceVolume);
                    newNode = node;
                }
                else
                {
                    newNode = new(devices, blLi[i], flLi[i], fps, voiceVolume);
                    updateCase |= UpdateCase.BitmapParams;
                }

                if (BusNumDictionary.TryGetValue(newNode.BusNum, out var nodes))
                {
                    nodes.Add(newNode);
                }
                else
                {
                    BusNumDictionary[newNode.BusNum] = [newNode];
                }
            }
            
            if (disposedNodes.Count > 0)
            {
                disposedNodes.ForEach(node => node.Dispose());
            }

            List<PartNode> newPartNodes = [];

            foreach (int busNum in BusNumDictionary.Keys.OrderBy(n => n))
            {
                newPartNodes.AddRange(BusNumDictionary[busNum]);
            }
                

            if (preDrawList.Count == liCount)
            {
                for (int i = 0; i < liCount; i++)
                {
                    if (preDrawList[i] != newPartNodes[i])
                    {
                        updateCase |= UpdateCase.BlendMode;
                        break;
                    }
                }
            }
            else
            {
                updateCase |= UpdateCase.BlendMode;
            }

            if (updateCase != UpdateCase.None)
            {
                preDrawList.Clear();
                preDrawList.AddRange(newPartNodes);
            }

            return updateCase;
        }

        protected void UpdateOutputs(TimelineItemSourceDescription description)
        {
            IEnumerable<PartNode> sortedPartNodes = [.. preDrawList.OrderBy(node => node.ParentPath.Count)];
            foreach (var cloneNode in sortedPartNodes)
            {
                cloneNode.UpdateOutput(description);
            }   
        }

        protected void SetCommandList(UpdateCase updateCase)
        {
            if (!(UpdateDrawList() || updateCase.HasFlag(UpdateCase.BlendMode))) return;
            commandList?.Dispose();
            commandList = devices.DeviceContext.CreateCommandList();
            ID2D1DeviceContext6 dc = devices.DeviceContext;
            dc.Target = commandList;
            dc.BeginDraw();
            dc.Clear(null);
            if (preDrawList.Count == 0)
            {
                dc.DrawImage(empty, compositeMode: CompositeMode.SourceOver);
            }
            else
            {
                foreach (var partNode in drawlist)
                    if (partNode.Output is ID2D1Image output)
                        DrawOrBlend(dc, partNode.BlendMode, output);
                
            }
            dc.EndDraw();
            commandList.Close();//CommandListはEndDraw()の後に必ずClose()を呼んで閉じる必要がある
        }

        private bool UpdateDrawList()
        {
            if (preDrawList.Count == 0)
            {
                if (this.drawlist.Count == 0)
                {
                    return false;
                }
                else
                {
                    this.drawlist.Clear();
                    return true;
                }
            }

            List<PartNode> drawlist = [];

            var glb = preDrawList.Where(x => x.Appear).ToLookup(
                x => x.ZSortMode == ZSortMode.GlobalSpace && x.M43 != 0f ? x.M43 < 0f ? -1 : 1 : 0);

            foreach (var partNode in glb[-1].OrderBy(x => x.M43))
                if (partNode.Output is ID2D1Image output)
                    drawlist.Add(partNode);

            foreach (var bus in glb[0].ToLookup(x => x.BusNum))
            {
                var sub = bus.ToLookup(x => x.ZSortMode != ZSortMode.Ignore && x.M43 != 0f ? x.M43 < 0f ? -1 : 1 : 0);
                foreach (var partNode in (ImmutableArray<PartNode>)[.. sub[-1], .. sub[0], .. sub[1]])
                    if (partNode.Output is ID2D1Image output)
                        drawlist.Add(partNode);
            }

            foreach (var partNode in glb[1].OrderBy(x => x.M43))
                if (partNode.Output is ID2D1Image output)
                    drawlist.Add(partNode);

            if (isFirst || preDrawList.Count != this.drawlist.Count)
            {
                this.drawlist.Clear();
                this.drawlist.AddRange(drawlist);
                return true;
            }

            for (var i = 0; i < this.drawlist.Count; i++)
            {
                if (this.drawlist[i] != drawlist[i])
                {
                    this.drawlist.Clear();
                    this.drawlist.AddRange(drawlist);
                    return true;
                }
            }

            return false;
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
            while (preDrawList.Count > count)
            {
                preDrawList[count].Dispose();
                preDrawList.RemoveAt(count);
            }
        }

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // マネージド状態を破棄します (マネージド オブジェクト)
                    foreach (var partnode in preDrawList)
                        partnode.Dispose();
                    commandList?.Dispose();
                    disposer.Dispose();
                }

                // アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
