using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinTachiePlugin.PartAnimation.Node.ImageFileNode
{
    internal class ImageFileNode
    {
        private readonly string? path;

        public List<ImageFileNode> Children { get; init; } = [];

        public ImageFileNode()
        {
        }

        public ImageFileNode(string path)
        {
            this.path = path;
        }
    }
}
