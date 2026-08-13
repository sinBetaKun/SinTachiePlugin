namespace SinTachiePlugin.Draw
{
    /// <summary>
    /// パーツのパラメータが変化したとき、何を更新するべきかをまとめるレコード
    /// </summary>
    /// <param name="CommandList">描画順序などのCommandListに影響する変化があったか</param>
    /// <param name="PartNetwork">親子関係やクリッピング等、ほかのパーツに影響する値の変化があったか</param>
    /// <param name="Value">そのパーツ自身を更新する必要のある値の変化があったか</param>
    /// <param name="Exception">更新時に例外が発生したか</param>
    internal record PartNodeComparateResult(bool CommandList = false, bool PartNetwork = false, bool Value = false, bool Exception = false)
    {
        public static PartNodeComparateResult operator |(PartNodeComparateResult a, PartNodeComparateResult b)
            => new(
                CommandList: a.CommandList | b.CommandList,
                PartNetwork: a.PartNetwork | b.PartNetwork,
                Value: a.Value | b.Value,
                Exception: a.Exception | b.Exception);

        public static PartNodeComparateResult CalcSigma(IEnumerable<PartNodeComparateResult> results)
        {
            bool b0, b1, b2, b3;
            b0 = b1 = b2 = b3 = false;

            foreach (PartNodeComparateResult _0 in results)
            {
                b0 |= _0.CommandList;
                b1 |= _0.PartNetwork;
                b2 |= _0.Value;
                b3 |= _0.Exception;
            }

            return new(b0, b1, b2, b3);
        }
    }
}
