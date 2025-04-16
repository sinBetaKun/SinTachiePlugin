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
    public class LayerNode : IDisposable
    {
        readonly IImageFileSource? source;
        public int? Index { get; set; } = null;
        public int Depth { get; set; } = -1;
        public List<LayerNode> Children { get; set; } = [];

        public LayerNode() { }
        public LayerNode(string path, IGraphicsDevicesAndContext devices)
        {
            source = ImageFileSourceFactory.Create(devices, path);
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

        public IImageFileSource? GetSource(List<double> values, List<OuterLayerValueMode> outers)
        {
            if (Depth < 0)
            {
                return null;
            }

            if (values.Count() <= Depth)
            {
                return source;
            }
            
            if (values[Depth] < 0 || 1 < values[Depth])
            {
                string clsName = GetType().Name;
                string? mthName = MethodBase.GetCurrentMethod()?.Name;
                SinTachieDialog.ShowError($"無効な差分指定({values[Depth]})", clsName, mthName);
                throw new Exception($"無効な差分指定({values[Depth]})");
            }

            IImageFileSource? output;
            int num = outers[Depth] == OuterLayerValueMode.Loop && values[Depth] < 1 ? 1 : 0;

            if ((from child in Children select child.Index).Contains(null))
            {
                int layerIndex = (int)(values[Depth] * (Children.Count - 1 + num));
                output = Children[layerIndex].GetSource(values, outers);
            }
            else
            {
                int layerIndex = (int)(values[Depth] * (Children.Count + num));
                if (layerIndex == Children.Count) return source;
                output = Children[layerIndex].GetSource(values, outers);
            }

            return output ?? source;
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
                    source?.Dispose();
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
