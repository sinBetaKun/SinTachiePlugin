using SinTachiePlugin.Enums;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg
{
    internal static class PartAnimationNormalizationModeEx
    {
        /// <summary>
        /// ｱﾆﾒｰｼｮﾝﾚｲﾔｰの第一次出力値を正規化し、姉妹識別に必要な情報（第二次出力値）にして返す。
        /// </summary>
        /// <param name="mode">正規化モード</param>
        /// <param name="result">第一次出力値</param>
        /// <param name="indexCount">ｱﾆﾒｰｼｮﾝﾚｲﾔｰの子ノード（インデックス識別）の数</param>
        /// <returns>第二次出力値</returns>
        /// <exception cref="Exception"></exception>
        public static PartAnimationResultB Normalize(this PartAnimationNormalizationMode mode, PartAnimationResultA result, int indexCount)
        {
            // 第一次出力値が数値を示す場合
            if (result.IsVolume)
            {
                // ノードに子孫がいない場合 -1 を出力
                if (indexCount == 0)
                    return PartAnimationResultB.FromIndex(-1);

                // 正規化後の出力数値
                double d_0 = result.Volume;

                switch (mode)
                {
                    case PartAnimationNormalizationMode.Limit:
                        // 出力インデックス
                        int i_0;

                        // 右端・右端外部
                        if (d_0 >= 1)
                            i_0 = -1;
                        // 左端・左端外部
                        else if (d_0 <= 0)
                            i_0 = 0;
                        // 区間内
                        else
                            i_0 = (int)Math.Floor(d_0 * indexCount);

                        return PartAnimationResultB.FromIndex(i_0);

                    case PartAnimationNormalizationMode.Shuttle:
                        // 第一次出力を２で割ったときの正の剰余
                        double d_1 = (d_0 % 2.0 + 2.0) % 2.0;
                        // 出力インデックス
                        int i_1;

                        // 往復運動の後半、折り返し後。
                        if (d_1 >= 1)
                        {
                            i_1 = 2 * indexCount - (int)Math.Floor(d_1 * indexCount);

                            if (i_1 == indexCount)
                                i_1 = -1;
                        }
                        // 往復運動の前半、折り返し前。
                        else
                        {
                            i_1 = (int)Math.Floor(d_1 * indexCount);
                        }

                        return PartAnimationResultB.FromIndex(i_1);

                    case PartAnimationNormalizationMode.Loop:
                        // 第一次出力を１で割ったときの正の剰余で正規化した値
                        double d_2 = d_0 - Math.Floor(d_0);
                        // 出力インデックス
                        int i_2;

                        // ループの原点（小数点以下も0の場合。）
                        if (d_2 == 0)
                        {
                            i_2 = -1;
                        }
                        // 原点以外の場所
                        else
                        {
                            // 量子化（インデックス化）
                            i_2 = (int)Math.Floor(d_0 * indexCount);

                            // 量子化後の値が0、ループの原点を示す場合
                            if (i_2 == indexCount)
                                i_2 = -1;
                        }

                        return PartAnimationResultB.FromIndex(i_2);

                    default:
                        throw new Exception(TextResource.ImageFileNode_Error_InvalidNormalizationMode + $":({nameof(mode)})");
                }
            }
            // 第一次出力値が文字列を示す場合
            else
            {
                return PartAnimationResultB.FromText(result.Text);
            }
        }
    }
}
