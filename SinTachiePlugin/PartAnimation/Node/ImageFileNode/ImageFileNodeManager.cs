using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Properties;
using System.IO;
using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin;
using YukkuriMovieMaker.Settings;

namespace SinTachiePlugin.PartAnimation.Node.ImageFileNode
{
    internal class ImageFileNodeManager : IDisposable
    {
        private readonly IGraphicsDevicesAndContext _devices;
        private readonly ImageFileNode? _root;
        private string _nowPath = string.Empty;
        public IImageFileSource? Source { get; private set; }
        public int Width { get; private set; } = 0;
        public int Height { get; private set; } = 0;

        public ImageFileNodeManager(string path, IGraphicsDevicesAndContext _device)
        {
            _devices = _device;

            if (string.IsNullOrEmpty(path))
                // パスが空文字列の場合
                return;

            if (!Path.Exists(path))
                // 指定されたパスを持つファイルがない場合
                return;

            if (FileSettings.Default.FileExtensions.GetFileType(path) != FileType.画像)
                // 指定されたファイルが画像ファイルでない場合
                return;

            var dirName = Path.GetDirectoryName(path);

            if (dirName == null)
                // ディレクトリのパスを取得できなかった場合
                return;

            // ルートノードを作成
            _root = new(path)
            {
                Depth = 0
            };

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
                where FileSettings.Default.FileExtensions.GetFileType(name2) == FileType.画像
                where name2.StartsWith(name)
                select name2
                ];

            // ファイル名とその連番を配列に変換したものの辞書
            Dictionary<string, int[]> layerNumsDict = [];

            for (int i = 0; i < files.Length; i++)
            {
                if (Path.GetFileNameWithoutExtension(files[i]) is string fileName)
                {
                    // ドットで分割
                    string[] devideds = fileName.Split(".");
                    int[] layerNums = new int[devideds.Length - 1];
                    bool isLayer = true;

                    // 連番を配列に変換する
                    for (int j = 1; j < devideds.Length; j++)
                    {
                        if (int.TryParse(devideds[j], out int n))
                        {
                            if (n < 0)
                            {
                                // 連番が負の数の場合
                                isLayer = false;
                                break;
                            }
                            else
                            {
                                // 連番が数値である場合はそのまま
                                layerNums[j - 1] = n;
                            }
                        }
                        else if (devideds[j] == "_")
                        {
                            // 連番がアンダースコアである場合は-1にする
                            layerNums[j - 1] = -1;
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
                    ImageFileNode tmp = new (Path.Combine(dirName, kv.Key));
                    AddLeaf(tmp, kv.Value);
                }
            }
        }

        private void AddLeaf(ImageFileNode node, int[] indexs)
        {
            if (_root is null)
                return;

            var len = indexs.Length;
            if (len < 1)
            {
                node.Depth = 0;
                SinTachieDialog.ShowError(new(TextResource.ImageFileNodeManager_Error_InvalidIndex));
                return;
            }

            ImageFileNode tmp = _root;

            for (int i = 0; i < len - 1; i++)
            {
                ImageFileNode? tmp2 = tmp.Children.FirstOrDefault(node => node.Index == indexs[i]);

                if (tmp2 is null)
                {
                    for (int j = i; j < len - 1; j++)
                    {
                        // 空のノードを追加
                        tmp2 = new ImageFileNode() { Depth = i + 1, Index = indexs[j] };
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

                if (node.Index < 0)
                {
                    tmp.Children.Add(node);
                    return;
                }

                int num = tmp.Children.Count - (indexs2.Contains(-1) ? 1 : 0);

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

        public void UpdateSource(List<double> values, List<PartAnimationNormalizationMode> outers)
        {
            if (_root is not null)
            {
                if (_root.GetValue(values, outers) is string path)
                {
                    if (_nowPath != path)
                    {
                        _nowPath = path;
                        Source?.Dispose();
                        Source = ImageFileSourceFactory.Create(_devices, path);
                        if (Source is not null)
                        {
                            SizeI size = Source.Output.PixelSize;
                            if (Width != size.Width || Height != size.Height)
                            {
                                Width = size.Width;
                                Height = size.Height;
                            }
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            Source = null;
            Width = 0;
            Height = 0;
        }

        #region IDisposable
        public void Dispose()
        {
            Source?.Dispose();
        }
        #endregion
    }
}
