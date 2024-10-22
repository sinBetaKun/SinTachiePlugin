using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        public ID2D1Image? Output;
        readonly ID2D1Bitmap empty;
        IImageFileSource? source;

        AffineTransform2D offset;
        public int? Index { get; set; } = null;
        public int Depth { get; set; } = -1;
        public List<LayerNode> Children { get; set; } = [];

        public LayerNode(IGraphicsDevicesAndContext devices)
        {
            this.devices = devices;
            offset = new AffineTransform2D(devices.DeviceContext);
            empty = devices.DeviceContext.CreateEmptyBitmap();
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
                var kvs = layerNumsDictionary.Where(preNode => preNode.Value.Length == targetLength);

                foreach (var kv in kvs)
                {
                    var tmp = new LayerNode(devices);
                    tmp.source = ImageFileSourceFactory.Create(devices, Path.Combine(dirName, kv.Key));
                    if (tmp.source != null)
                    {
                        tmp.offset.SetInput(0, tmp.source.Output, true);
                        tmp.offset.TransformMatrix = Matrix3x2.CreateTranslation(-tmp.source.Output.Size.Width / 2, -tmp.source.Output.Size.Height / 2);
                        tmp.Output = tmp.offset.Output;
                    }
                    AddLeaf(tmp, kv.Value);
                }

                numsOfIndexs = numsOfIndexs.Where(numOfIndexs => numOfIndexs != targetLength).ToArray();
            }
            source = ImageFileSourceFactory.Create(devices, root);
            if (source != null)
            {
                offset.SetInput(0, source.Output, true);
                offset.TransformMatrix = Matrix3x2.CreateTranslation(-source.Output.Size.Width / 2, -source.Output.Size.Height / 2);
                Output = offset.Output;
            }
        }

        void AddLeaf(LayerNode node, int?[] indexs)
        {
            var len = indexs.Length;
            if (len < 1)
            {
                node.Depth = 0;
                //string clsName = GetType().Name;
                //string? mthName = MethodBase.GetCurrentMethod()?.Name;
                //SinTachieDialog.ShowError("インデックスが正しく指定されていないリーフをLayerNodeに足そうとしました。ごめんなさい。", clsName, mthName);
                return;
            }
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

        public ID2D1Image? GetSource(List<double> values, List<OuterLayerValueMode> outers)
        {
            if (Depth < 0)
            {
                return empty;
                //string clsName = GetType().Name;
                //string? mthName = MethodBase.GetCurrentMethod()?.Name;
                //SinTachieDialog.ShowError("無効なLayerNode", clsName, mthName);
                //throw new Exception("無効なLayerNode");
            }

            if (values.Count() <= Depth)
                return Output;

            if (values[Depth] < 0 || 1 < values[Depth])
            {
                string clsName = GetType().Name;
                string? mthName = MethodBase.GetCurrentMethod()?.Name;
                SinTachieDialog.ShowError($"無効なレイヤー値({values[Depth]})", clsName, mthName);
                throw new Exception($"無効なレイヤー値({values[Depth]})");
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

        public void Dispose()
        {
            offset.SetInput(0, null, true);
            offset.Dispose();
            source?.Dispose();
            empty?.Dispose();
            foreach (var child in Children) child.Dispose();
        }
    }
}
