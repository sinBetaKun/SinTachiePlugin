using SinTachiePlugin.Enums;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using System.IO;
using YukkuriMovieMaker.Settings;

namespace SinTachiePlugin.PartAnimation.Node.ImageFileNode
{
    internal class ImageFileNodeManager
    {
        public readonly ImageFileNode? Root;
        public string Value { get; private set; } = string.Empty;

        public ImageFileNodeManager(string path)
        {
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
            Root = new(path)
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
            Dictionary<string, string[]> d0 = [];

            foreach(string file in files)
            {
                if (Path.GetFileNameWithoutExtension(file) is string fileName)
                {
                    d0.Add(file, fileName.Split(".")[1..]);
                }
            }

            int n0 = d0.Values.Select(v => v.Length).Max();
            ImageFileNode root = new(path) { Depth = 0 };
            List<ImageFileNode> l0 = [root];

            for (int i = 1; i <= n0; i++)
            {
                foreach (var kv in d0.Where(x => x.Value.Length == i))
                {
                    string s0 = kv.Key;
                    string[] a0 = kv.Value;
                    ImageFileNode _0 = root;
                    bool b0 = false;

                    for (int j = 0; j < i - 1; j++)
                    {
                        string s1 = a0[j];

                        if (_0.OtherChildren.FirstOrDefault(x => x.Text == s1) is ImageFileNode _1)
                        {
                            _0 = _1;
                        }
                        else if (s1 == "_")
                        {
                            bool b1 = true;

                            for (int k = j + 1; k < i - 1; k++)
                                if (a0[k] != "_")
                                    b1 = false;

                            if (b1 && a0.Last() != "_")
                            {
                                while (j < i - 1)
                                {
                                    j++;
                                    ImageFileNode _2 = new(_0.Path) { Depth = j, Text = "_" };
                                    l0.Add(_2);
                                    _0.OtherChildren.Add(_2);
                                    _0 = _2;
                                }
                            }
                            else
                            {
                                b0 = true;
                            }

                            break;
                        }
                        else
                        {
                            b0 = true;
                            break;
                        }
                    }

                    if (b0)
                        break;

                    ImageFileNode _3 = new(s0) { Depth = i, Text = a0.Last() };
                    l0.Add(_3);
                    _0.OtherChildren.Add(_3);
                }
            }

            foreach (ImageFileNode _0 in l0)
            {
                var a0 = _0.OtherChildren.Where(x =>
                {
                    if (int.TryParse(x.Text, out int n1))
                        if (n1 >= 0)
                            return true;

                    return false;
                }).OrderBy(x => int.Parse(x.Text));

                foreach (ImageFileNode _1 in a0)
                {
                    _0.OtherChildren.Remove(_1);
                    _0.VolumeChildren.Add(_1);
                }
            }

            Value = path;
        }

        public void Update(List<PartAnimationResultA> results, List<PartAnimationNormalizationMode> outers)
        {
            if (Root is not null)
            {
                if (Root.GetValue(results, outers) is string path)
                {
                    Value = path;
                }
            }
        }
    }
}
