using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using System.IO;
using System.Numerics;
using System.Reflection;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin;

namespace SinTachiePlugin.LayerValueListController
{
    public class LayerNode
    {
        readonly string path = string.Empty;
        public int? Index { get; set; } = null;
        public int Depth { get; set; } = -1;
        public List<LayerNode> Children { get; set; } = [];

        public LayerNode() { }

        public LayerNode(string path)
        {
            this.path = path;
        }

        internal LayerNode? GetChildByIndex(int? index)
        {
            var muchs = from node in Children
                        where node.Index == index
                        select node;
            if (!muchs.Any()) return null;
            if (muchs.Count() > 1)
            {
                SinTachieDialog.ShowError(new Exception("インデックスが重複しているLayerNode2を検出しました。"));
                throw new Exception("インデックスが重複しているLayerNode2を検出しました。");
            }
            return muchs.First();
        }

        public string? GetValue(List<double> values, List<OuterLayerValueMode> outers)
        {
            if (Depth < 0)
            {
                return null;
            }

            if (values.Count <= Depth)
            {
                return path;
            }

            try
            {
                if (values[Depth] < 0 || 2 <= values[Depth])
                {
                    throw new Exception($"無効な差分指定({values[Depth]})");
                }

                string? ret;
                int num, num2;
                double value;

                if (values[Depth] > 1)
                {
                    if (outers[Depth] != OuterLayerValueMode.Shuttle)
                    {
                        throw new Exception($"無効な差分指定({values[Depth]} (OuterLayerValueMode:Shuttle))");
                    }

                    value = 2 - values[Depth];
                    num = 0;
                    num2 = 1;
                }
                else
                {
                    value = values[Depth];
                    num = outers[Depth] == OuterLayerValueMode.Loop ? 1 : 0;
                    num2 = 0;
                }

                if (Children.Any(child => child.Index == null))
                {
                    int layerIndex = (int)(value * (Children.Count - 1 + num)) + num2;
                    ret = Children[layerIndex].GetValue(values, outers);
                }
                else
                {
                    int layerIndex = (int)(value * (Children.Count + num)) + num2;
                    if (layerIndex == Children.Count) return path;
                    ret = Children[layerIndex].GetValue(values, outers);
                }

                return ret ?? path;
            }
            catch (Exception ex)
            {
                SinTachieDialog.ShowError(ex);
                throw new(ex.Message);
            }
        }
    }
}
