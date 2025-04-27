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

        public LayerNode(string path, IGraphicsDevicesAndContext devices)
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
                string clsName = GetType().Name;
                string? mthName = MethodBase.GetCurrentMethod()?.Name;
                SinTachieDialog.ShowError("インデックスが重複しているLayerNode2を検出しました。", clsName, mthName);
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

            if (values.Count() <= Depth)
            {
                return path;
            }
            
            if (values[Depth] < 0 || 1 < values[Depth])
            {
                string clsName = GetType().Name;
                string? mthName = MethodBase.GetCurrentMethod()?.Name;
                SinTachieDialog.ShowError($"無効な差分指定({values[Depth]})", clsName, mthName);
                throw new Exception($"無効な差分指定({values[Depth]})");
            }

            string? ret;
            int num = outers[Depth] == OuterLayerValueMode.Loop && values[Depth] < 1 ? 1 : 0;

            if ((from child in Children select child.Index).Contains(null))
            {
                int layerIndex = (int)(values[Depth] * (Children.Count - 1 + num));
                ret = Children[layerIndex].GetValue(values, outers);
            }
            else
            {
                int layerIndex = (int)(values[Depth] * (Children.Count + num));
                if (layerIndex == Children.Count) return path;
                ret = Children[layerIndex].GetValue(values, outers);
            }

            return ret ?? path;
        }
    }
}
