using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.PartAnimation.Node.ImageFileNode
{
    /// <summary>
    /// 多次元ｱﾆﾒｰｼｮﾝ差分画像の樹形構造の構成単位。
    /// <br/>
    /// １つの差分画像のパス、深度、姉妹識別文字列、子ノードを纏めている。
    /// </summary>
    /// <param name="path">ノードに対応させる画像ファイルのパス。</param>
    internal class ImageFileNode(string path, int depth)
    {
        /// <summary>
        /// ノードが対応する画像ファイルのパス。
        /// </summary>
        public readonly string Path = path;

        /// <summary>
        /// 多次元ｱﾆﾒｰｼｮﾝ構造を木構造としてみたときの深度。
        /// <br/>
        /// 多次元構造における次元の番号を示す。
        /// </summary>
        public readonly int Depth = depth;

        /// <summary>
        /// 姉妹識別子。
        /// <br/>
        /// 多次元ｱﾆﾒｰｼｮﾝ構造を木構造としてみたときの、姉妹を識別するための固有文字列。
        /// <br/>
        /// ファイル名に添えたドット「.」で分割される文字列。
        /// </summary>
        public string Text = string.Empty;

        /// <summary>
        /// 数値でソートされインデックスによって姉妹を識別する子ノード
        /// </summary>
        public readonly List<ImageFileNode> VolumeChildren = [];

        /// <summary>
        /// 文字列によって姉妹を識別する子ノード
        /// <br/>
        /// VolumeChildrenに含まれていないノードでもある。
        /// </summary>
        public readonly List<ImageFileNode> OtherChildren = [];

        /// <summary>
        /// 第一次出力値から、このノードで選出する画像ファイルのパスを決定する。
        /// </summary>
        /// <param name="values">各次元の第一次出力値</param>
        /// <param name="nmlzs">第一次出力値リストに対応する正規化モード</param>
        /// <returns>このノードで選出した画像ファイルのパス</returns>
        public string? GetValue(List<PartAnimationResultA> values, List<PartAnimationNormalizationMode> nmlzs)
        {
            // 本来なら深度が負の数になることは無い。
            if (Depth < 0)
                return null;

            // これより先の子孫に対応する第一次出力値がない場合、このノードが対応する画像のファイルパスを選出する。
            if (values.Count <= Depth)
                return Path;

            try
            {
                // 第一次出力値を正規化して得られる第二次出力値
                PartAnimationResultB parB_0 = nmlzs[Depth].Normalize(values[Depth], VolumeChildren.Count);

                // 選出した画像のファイルパス
                string? s_0;

                // 第二次出力値が数値を示している場合
                if (parB_0.IsIndex)
                {
                    // インデックスが -1 を示している場合
                    // （ VolumeChildren.Count == 0 の場合もここに該当。）
                    if (parB_0.Index < 0)
                    {
                        // 
                        if (OtherChildren.FirstOrDefault(x => x.Text == "_") is ImageFileNode ifn_0)
                        {
                            s_0 = ifn_0.GetValue(values, nmlzs);
                        }
                        else
                        {
                            return Path;
                        }
                    }
                    else if (parB_0.Index < VolumeChildren.Count)
                    {
                        s_0 = VolumeChildren[parB_0.Index].GetValue(values, nmlzs);
                    }
                    else
                    {
                        throw new Exception(TextResource.ImageFileNodeManager_Error_InvalidIndex);
                    }
                }
                // 第二次出力値が文字列を示している場合
                else
                {
                    if (OtherChildren.FirstOrDefault(x => x.Text == parB_0.Text) is ImageFileNode _1)
                    {
                        s_0 = _1.GetValue(values, nmlzs);
                    }
                    else
                    {
                        return Path;
                    }
                }

                return s_0 ?? Path;
            }
            catch (Exception ex)
            {
                SinTachieDialog.ShowError(ex);
                throw new(ex.Message);
            }
        }
    }
}
