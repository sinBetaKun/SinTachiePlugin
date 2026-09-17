namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg
{
    /// <summary>
    /// ｱﾆﾒｰｼｮﾝﾚｲﾔｰの第二次出力値。
    /// <br/>
    /// 第一次出力値を正規化し、姉妹識別に必要な情報にしたもの。
    /// <br/>
    /// コンストラクタによる生成は非推奨。
    /// </summary>
    /// <param name="IsIndex">インデックスによる姉妹識別を行う場合は真、文字列の場合は偽。</param>
    /// <param name="Text">文字列の出力。</param>
    /// <param name="Index">インデックスの出力。-1の場合もある。</param>
    internal record PartAnimationResultB(bool IsIndex, string Text, int Index)
    {
        /// <summary>
        /// </summary>
        /// <param name="text">第二次出力文字列</param>
        /// <returns>ｱﾆﾒｰｼｮﾝﾚｲﾔｰ第二次出力値（文字列）</returns>
        public static PartAnimationResultB FromText(string text) => new(false, text, 0);

        /// <summary>
        /// </summary>
        /// <param name="text">第二次出力インデックス</param>
        /// <returns>ｱﾆﾒｰｼｮﾝﾚｲﾔｰ第二次出力値（インデックス）</returns>
        public static PartAnimationResultB FromIndex(int index) => new(true, string.Empty, index);
    }
}
