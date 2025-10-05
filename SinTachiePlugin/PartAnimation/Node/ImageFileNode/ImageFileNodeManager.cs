using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinTachiePlugin.PartAnimation.Node.ImageFileNode
{
    internal class ImageFileNodeManager
    {
        private ImageFileNode? root;

        public ImageFileNodeManager(string path)
        {
            SetRoot(path);
        }

        private void SetRoot(string path)
        {
            if (string.IsNullOrEmpty(path))
                // パスが空文字列の場合
                return;

            if (!Path.Exists(path))
                // 指定されたパスを持つファイルがない場合
                return;

            // ルートノードを作成
            root = new(path);
        }
    }
}
