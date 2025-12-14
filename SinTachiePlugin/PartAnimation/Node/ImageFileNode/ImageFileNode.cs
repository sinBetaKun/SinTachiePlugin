using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.PartAnimation.Node.ImageFileNode
{
    internal class ImageFileNode
    {
        private readonly string? path;
        public int Index = -1;
        public int Depth = -1;

        public readonly List<ImageFileNode> Children = [];

        public ImageFileNode()
        {
        }

        public ImageFileNode(string path)
        {
            this.path = path;
        }

        public string? GetValue(List<double> values, List<PartAnimationNormalizationMode> outers)
        {
            if (Depth < 0)
                return null;

            if (values.Count <= Depth)
                return path;

            try
            {
                if (values[Depth] < 0 || 2 <= values[Depth])
                {
                    throw new Exception(TextResource.ImageFileNode_Error_InvalidValue + $"({values[Depth]})");
                }

                string? ret;
                int num, num2;
                double value;

                if (values[Depth] > 1)
                {
                    if (outers[Depth] != PartAnimationNormalizationMode.Shuttle)
                    {
                        throw new Exception(TextResource.ImageFileNode_Error_InvalidValue + $"({values[Depth]} (OuterLayerValueMode:Shuttle))");
                    }

                    value = 2 - values[Depth];
                    num = 0;
                    num2 = 1;
                }
                else
                {
                    value = values[Depth];
                    num = outers[Depth] == PartAnimationNormalizationMode.Loop ? 1 : 0;
                    num2 = 0;
                }

                if (Children.Any(child => child.Index < 0))
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
