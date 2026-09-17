namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg
{
    /// <summary>
    /// 軸一本分のｱﾆﾒｰｼｮﾝﾚｲﾔｰの第一次出力値。
    /// <br/>
    /// 数値か文字列の出力を示す。
    /// <br/>
    /// コンストラクタでの生成は非推奨。
    /// </summary>
    /// <param name="IsVolume">数値を示していれば真、文字列であれば偽</param>
    /// <param name="Text">文字列の出力値</param>
    /// <param name="Volume">数値の出力値</param>
    internal record PartAnimationResultA(bool IsVolume, string Text, double Volume)
    {
        /// <summary>
        /// </summary>
        /// <param name="text">第一次出力文字列</param>
        /// <returns>ｱﾆﾒｰｼｮﾝﾚｲﾔｰ第一次出力値（文字列）</returns>
        public static PartAnimationResultA FromText(string text) => new(false, text, 0);

        /// <summary>
        /// </summary>
        /// <param name="volume">第一次出力数値</param>
        /// <returns>ｱﾆﾒｰｼｮﾝﾚｲﾔｰ第一次出力値（数値）</returns>
        public static PartAnimationResultA FromVolume(double volume) => new(true, string.Empty, volume);
    }
}
