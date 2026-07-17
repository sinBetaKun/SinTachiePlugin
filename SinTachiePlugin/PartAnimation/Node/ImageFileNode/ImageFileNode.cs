using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.PartAnimation.Node.ImageFileNode
{
    internal class ImageFileNode
    {
        public readonly string Path;
        public int Depth = -1;
        public string Text = string.Empty;

        public readonly List<ImageFileNode> VolumeChildren = [];
        public readonly List<ImageFileNode> OtherChildren = [];

        public ImageFileNode(string path)
        {
            Path = path;
        }

        public string? GetValue(List<PartAnimationResultA> values, List<PartAnimationNormalizationMode> nmlzs)
        {
            if (Depth < 0)
                return null;

            if (values.Count <= Depth)
                return Path;

            try
            {
                PartAnimationResultB _0 = nmlzs[Depth].Normalize(values[Depth], VolumeChildren.Count);
                string? ret;

                if (_0.IsIndex)
                {
                    if (_0.Index < 0) // 実は VolumeChildren.Count == 0 だったらここが処理されるんだよね。
                    {
                        if (OtherChildren.FirstOrDefault(x => x.Text == "_") is ImageFileNode _1)
                        {
                            ret = _1.GetValue(values, nmlzs);
                        }
                        else
                        {
                            return Path;
                        }
                    }
                    else if (_0.Index < VolumeChildren.Count)
                    {
                        ret = VolumeChildren[_0.Index].GetValue(values, nmlzs);
                    }
                    else
                    {
                        throw new Exception(TextResource.ImageFileNodeManager_Error_InvalidIndex);
                    }
                }
                else
                {
                    if (OtherChildren.FirstOrDefault(x => x.Text == _0.Text) is ImageFileNode _1)
                    {
                        ret = _1.GetValue(values, nmlzs);
                    }
                    else
                    {
                        return Path;
                    }
                }

                return ret ?? Path;
            }
            catch (Exception ex)
            {
                SinTachieDialog.ShowError(ex);
                throw new(ex.Message);
            }
        }
    }
}
