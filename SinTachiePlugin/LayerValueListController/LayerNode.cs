using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Windows;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin;
using Path = System.IO.Path;

namespace SinTachiePlugin.LayerValueListController
{
    public class LayerNode : IDisposable
    {
        public IGraphicsDevicesAndContext devices;
        public ID2D1Image Output;
        readonly DisposeCollector disposer = new();
        readonly ID2D1Bitmap empty;
        IImageFileSource? source;
        bool wasEmpty = true;
        bool disposedValue = false;
        private readonly AffineTransform2D centeringEffect;
        public int? Index { get; set; } = null;
        public int Depth { get; set; } = -1;
        public List<LayerNode> Children { get; set; } = [];

        public LayerNode(IGraphicsDevicesAndContext devices)
        {
            this.devices = devices;
            centeringEffect = new AffineTransform2D(devices.DeviceContext);
            disposer.Collect(centeringEffect);
            empty = devices.DeviceContext.CreateEmptyBitmap();
            disposer.Collect(empty);
            centeringEffect.SetInput(0, empty, true);
            Output = centeringEffect.Output;
            disposer.Collect(Output);
        }

        public LayerNode(string root, IGraphicsDevicesAndContext devices) : this(devices)
        {
            if (string.IsNullOrEmpty(root)) return;
            if (!Path.Exists(root)) return;

            const YukkuriMovieMaker.Settings.FileType fileType = YukkuriMovieMaker.Settings.FileType.画像;
            if (!(YukkuriMovieMaker.Settings.FileSettings.Default.FileExtensions.GetFileType(root) is fileType)) return;

            var dirName = Path.GetDirectoryName(root);
            if (dirName == null) return;

            Depth = 0;
            var dir = new DirectoryInfo(dirName);
            if (dir is null) return;
            var name = Path.GetFileNameWithoutExtension(root);
            var ext = Path.GetExtension(root);
            string[] files = (from imageFile in dir.GetFiles().Select(file => file.Name)
                              where imageFile.StartsWith(name)
                              where YukkuriMovieMaker.Settings.FileSettings.Default.FileExtensions.GetFileType(imageFile) is fileType
                              select imageFile).ToArray();

            Dictionary<string, int?[]> layerNumsDictionary = [];
            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetFileNameWithoutExtension(files[i]) is string fileName)
                {
                    string[] devideds = fileName.Split(".");
                    List<int?> layerNumsList = [];
                    bool isLayer = true;
                    for (int j = 1; j < devideds.Length; j++)
                    {
                        int n;
                        if (int.TryParse(devideds[j], out n))
                            layerNumsList.Add(n);

                        else if (devideds[j] == "_")
                            layerNumsList.Add(null);

                        else
                        {
                            isLayer = false;
                            break;
                        }
                    }
                    if (isLayer) layerNumsDictionary.Add(files[i], layerNumsList.ToArray());
                }
            }
            int[] numsOfIndexs = layerNumsDictionary.Values.Select(indexs => indexs.Length).ToArray();
            Array.Sort(numsOfIndexs);
            while (numsOfIndexs.Length > 0)
            {
                var targetLength = numsOfIndexs.First();
                var kvs = layerNumsDictionary.Where(preNode => preNode.Value.Length == targetLength && preNode.Value.Length > 0);

                foreach (var kv in kvs)
                {
                    var tmp = new LayerNode(devices);
                    tmp.source = ImageFileSourceFactory.Create(devices, Path.Combine(dirName, kv.Key));
                    if (tmp.source != null)
                    {
                        tmp.disposer.Collect(tmp.source);
                        tmp.centeringEffect.SetInput(0, tmp.source.Output, true);
                        tmp.centeringEffect.TransformMatrix = Matrix3x2.CreateTranslation(-tmp.source.Output.Size.Width / 2, -tmp.source.Output.Size.Height / 2);
                        tmp.wasEmpty = false;
                    }
                    AddLeaf(tmp, kv.Value);
                }

                numsOfIndexs = numsOfIndexs.Where(numOfIndexs => numOfIndexs != targetLength).ToArray();
            }
            source = ImageFileSourceFactory.Create(devices, root);
            if (source != null)
            {
                disposer.Collect(source);
                centeringEffect.SetInput(0, source.Output, true);
                centeringEffect.TransformMatrix = Matrix3x2.CreateTranslation(-source.Output.Size.Width / 2, -source.Output.Size.Height / 2);
                wasEmpty = false;
            }
        }

        void AddLeaf(LayerNode node, int?[] indexs)
        {
            var len = indexs.Length;
            if (len < 1)
            {
                node.Depth = 0;
                string clsName = GetType().Name;
                string? mthName = MethodBase.GetCurrentMethod()?.Name;
                SinTachieDialog.ShowError("インデックスが正しく指定されていないリーフをLayerNodeに足そうとしました。ごめんなさい。", clsName, mthName);
                return;
            }
            disposer.Collect(node);
            var tmp = this;
            for (int i = 0; i < len - 1; i++)
            {
                var tmp2 = tmp.GetChildByIndex(indexs[i]);
                if (tmp2 is null)
                {
                    for (int j = i; j < len - 1; j++)
                    {
                        tmp2 = new LayerNode(devices) { Depth = i + 1, Index = indexs[j] };
                        tmp.Children.Add(tmp2);
                        tmp = tmp2;
                    }
                    break;
                }
                else tmp = tmp2;
            }
            var indexs2 = (from child in tmp.Children select child.Index).ToList();
            var index = indexs2.IndexOf(indexs.Last());
            if (index < 0)
            {
                node.Depth = len;
                node.Index = indexs.Last();
                if (node.Index is null)
                {
                    tmp.Children.Add(node);
                    return;
                }
                int num = tmp.Children.Count - (indexs2.Contains(null) ? 1 : 0);
                if (node.Index < 0)
                {
                    for (int i = num - 1; i > -1; i--)
                    {
                        if (tmp.Children[i].Index > 0)
                        {
                            tmp.Children.Add(node);
                            return;
                        }
                        if (tmp.Children[i].Index < node.Index)
                        {
                            tmp.Children.Insert(i + 1, node);
                            return;
                        }
                    }
                    tmp.Children.Insert(num, node);
                    return;
                }
                for (int i = 0; i < num; i++)
                {
                    if (tmp.Children[i].Index < 0)
                    {
                        tmp.Children.Insert(i, node);
                        return;
                    }
                    if (tmp.Children[i].Index > node.Index)
                    {
                        tmp.Children.Insert(i, node);
                        return;
                    }
                }
                tmp.Children.Insert(num, node);
                return;
            }
        }

        LayerNode? GetChildByIndex(int? index)
        {
            var muchs = from node in Children
                        where node.Index == index
                        select node;
            if (!muchs.Any()) return null;
            if (muchs.Count() > 1)
            {
                string clsName = GetType().Name;
                string? mthName = MethodBase.GetCurrentMethod()?.Name;
                SinTachieDialog.ShowError("インデックスが重複しているLayerNodeを検出しました。", clsName, mthName);
                throw new Exception("インデックスが重複しているLayerNodeを検出しました。");
            }
            return muchs.First();
        }

        public ID2D1Image GetSource(List<double> values, List<OuterLayerValueMode> outers)
        {
            if (Depth < 0)
            {
                return empty;
            }

            if (values.Count() <= Depth)
                return Output;

            if (values[Depth] < 0 || 1 < values[Depth])
            {
                string clsName = GetType().Name;
                string? mthName = MethodBase.GetCurrentMethod()?.Name;
                SinTachieDialog.ShowError($"無効な差分指定({values[Depth]})", clsName, mthName);
                throw new Exception($"無効な差分指定({values[Depth]})");
            }

            ID2D1Image? output;
            int num = outers[Depth] == OuterLayerValueMode.Loop && values[Depth] < 1 ? 1 : 0;

            if ((from child in Children select child.Index).Contains(null))
            {
                int layerIndex = (int)(values[Depth] * (Children.Count - 1 + num));
                output = Children[layerIndex].GetSource(values, outers);
            }
            else
            {
                int layerIndex = (int)(values[Depth] * (Children.Count + num));
                if (layerIndex == Children.Count) return Output;
                output = Children[layerIndex].GetSource(values, outers);
            }
            if (output != null) return output;
            return Output;
        }

        void ClearEffectChain()
        {
            centeringEffect.SetInput(0, null, true);
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                ClearEffectChain();
                disposer.Dispose();
                GC.SuppressFinalize(this);
                disposedValue = true;
            }
        }
    }
}
