using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using System.IO;
using System.Numerics;
using System.Reflection;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Settings;
using Path = System.IO.Path;

namespace SinTachiePlugin.LayerValueListController
{
    public class LayerNodeManager : IDisposable
    {
        public IGraphicsDevicesAndContext devices;
        public ID2D1Image Output => centeringEffect.Output;
        readonly DisposeCollector disposer = new();
        readonly ID2D1Bitmap empty;
        private readonly AffineTransform2D centeringEffect;
        private LayerNode root;
        bool isEmpty = true;
        int width = 0;
        int height = 0;
        
        public LayerNodeManager(string path, IGraphicsDevicesAndContext devices)
        {
            this.devices = devices;
            
            empty = devices.DeviceContext.CreateEmptyBitmap();
            disposer.Collect(empty);

            centeringEffect = new(devices.DeviceContext);
            disposer.Collect(centeringEffect);

            centeringEffect.SetInput(0, empty, true);
            disposer.Collect(centeringEffect.Output);

            SetRoot(path);
        }

        void SetRoot(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                // パスが空文字列の場合
                root = new();
                disposer.Collect(root);
                return;
            }

            if (!Path.Exists(path))
            {
                // 指定されたパスを持つファイルがない場合
                root = new();
                disposer.Collect(root);
                return;
            }

            // ルートノードを作成
            root = new(path, devices)
            {
                Depth = 0
            };
            disposer.Collect(root);

            if (FileSettings.Default.FileExtensions.GetFileType(path) is not FileType.画像)
            {
                // 指定されたファイルが画像ファイルでない場合
                return;
            }

            var dirName = Path.GetDirectoryName(path);

            if (dirName == null)
            {
                // ディレクトリのパスを取得できなかった場合
                return;
            }

            // パーツ画像をまとめているディレクトリ
            DirectoryInfo dir = new(dirName);

            // 指定されたファイルの名前から拡張子を取り除く
            string name = Path.GetFileNameWithoutExtension(path);

            // 指定されたファイルの拡張子を取得する
            string ext = Path.GetExtension(path);

            // dir直下の画像ファイルでnameから始まるものを取得する
            string[] files =
                [..
                from name2 in dir.GetFiles().Select(file => file.Name)
                where FileSettings.Default.FileExtensions.GetFileType(name2) is FileType.画像
                where name2.StartsWith(name)
                select name2
                ];

            // ファイル名とその連番を配列に変換したものの辞書
            Dictionary<string, int?[]> layerNumsDict = [];

            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetFileNameWithoutExtension(files[i]) is string fileName)
                {
                    // ドットで分割
                    string[] devideds = fileName.Split(".");
                    int?[] layerNums = new int?[devideds.Length - 1];
                    bool isLayer = true;

                    // 連番を配列に変換する
                    for (int j = 1; j < devideds.Length; j++)
                    {
                        if (int.TryParse(devideds[j], out int n))
                        {
                            // 連番が数値である場合はそのまま
                            layerNums[j - 1] = n;
                        }
                        else if (devideds[j] == "_")
                        {
                            // 連番がアンダースコアである場合はnullにする
                            layerNums[j - 1] = null;
                        }
                        else
                        {
                            // 正しい連番でない場合
                            isLayer = false;
                            break;
                        }
                    }

                    if (isLayer)
                    {
                        // 連番が正しい場合は辞書に登録
                        layerNumsDict.Add(files[i], layerNums);
                    }
                }
            }

            // 各ファイル名の連番の数を取得
            int[] numsOfIndexs = [.. layerNumsDict.Values.Select(indexs => indexs.Length)];

            // 解決対象の連番画像の連番の数
            int targetLength = -1;

            // 連番画像の解決
            while (true)
            {
                IEnumerable<int> unsolvedNums = numsOfIndexs.Where(num => num > targetLength);

                // 全ての連番画像が解決した場合
                if (!unsolvedNums.Any())
                {
                    break;
                }

                // 次に短い連番の数を取得
                targetLength = unsolvedNums.Min();

                var kvs = layerNumsDict.Where(preNode => preNode.Value.Length == targetLength && preNode.Value.Length > 0);

                foreach (var kv in kvs)
                {
                    var tmp = new LayerNode(Path.Combine(dirName, kv.Key), devices);
                    AddLeaf(tmp, kv.Value);
                }
            }
        }
        public void ChangeImageFile(string path)
        {
            root.Dispose();
            SetRoot(path);
        }
        void AddLeaf(LayerNode node, int?[] indexs)
        {
            var len = indexs.Length;
            if (len < 1)
            {
                node.Depth = 0;
                string clsName = GetType().Name;
                string? mthName = MethodBase.GetCurrentMethod()?.Name;
                SinTachieDialog.ShowError("インデックスが正しく指定されていないリーフをLayerNodeManagerに足そうとしました。ごめんなさい。", clsName, mthName);
                return;
            }

            disposer.Collect(node);

            LayerNode tmp = root;

            for (int i = 0; i < len - 1; i++)
            {
                var tmp2 = tmp.GetChildByIndex(indexs[i]);

                if (tmp2 is null)
                {
                    for (int j = i; j < len - 1; j++)
                    {
                        // 空のノードを追加
                        tmp2 = new LayerNode() { Depth = i + 1, Index = indexs[j] };
                        tmp.Children.Add(tmp2);
                        tmp = tmp2;
                    }

                    break;
                }
                else
                {
                    tmp = tmp2;
                }
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

        public void UpdateSource(List<double> values, List<OuterLayerValueMode> outers)
        {
            if (root is not null)
            {
                if (root.GetSource(values, outers) is IImageFileSource source)
                {
                    centeringEffect.SetInput(0, source.Output, true);
                    isEmpty = false;
                    SizeI size = source.Output.PixelSize;
                    if (width != size.Width || height != size.Height)
                    {
                        width = size.Width;
                        height = size.Height;
                        centeringEffect.TransformMatrix = Matrix3x2.CreateTranslation(-width / 2, -height / 2);
                    }
                    return;
                }
            }

            if (!isEmpty)
            {
                centeringEffect.SetInput(0, empty, true);
                isEmpty = true;
                width = 0;
                height = 0;
            }
        }

        void ClearEffectChain()
        {
            centeringEffect.SetInput(0, null, true);
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
                    ClearEffectChain();
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
