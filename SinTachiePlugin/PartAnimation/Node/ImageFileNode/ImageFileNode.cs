using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Properties;
using YukkuriMovieMaker.Plugin.Voice;

namespace SinTachiePlugin.PartAnimation.Node.ImageFileNode
{
    internal class ImageFileNode
    {
        private readonly string? path;
        public int Index = -1;
        public int Depth = -1;

        public readonly List<ImageFileNode> VolumeChildren = [];
        public readonly ImageFileNode?[] VowelChildren = new ImageFileNode?[6];

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

                if (VolumeChildren.Any(child => child.Index < 0))
                {
                    int layerIndex = (int)(value * (VolumeChildren.Count - 1 + num)) + num2;
                    ret = VolumeChildren[layerIndex].GetValue(values, outers);
                }
                else
                {
                    int layerIndex = (int)(value * (VolumeChildren.Count + num)) + num2;
                    if (layerIndex == VolumeChildren.Count) return path;
                    ret = VolumeChildren[layerIndex].GetValue(values, outers);
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
