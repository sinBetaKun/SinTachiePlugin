namespace SinTachiePlugin.Draw
{
    /// <summary>
    /// パーツのパラメータが変化したとき、何を更新するべきかをまとめるレコード
    /// </summary>
    /// <param name="Order">描画順序に影響する変化があったか(更新が確定しているわけではない)</param>
    /// <param name="Blend">合成モードに変化があったか</param>
    /// <param name="PartNetwork">親子関係やクリッピング等、ほかのパーツに影響する値の変化があったか</param>
    /// <param name="Value">そのパーツ自身を更新する必要のある値の変化があったか</param>
    /// <param name="Exception">更新時に例外が発生したか</param>
    internal record PartNodeComparateResult(bool Order = false, bool Blend = false, bool PartNetwork = false, bool Value = false, bool Exception = false)
    {
        public static PartNodeComparateResult operator |(PartNodeComparateResult a, PartNodeComparateResult b)
            => new(
                Order: a.Order | b.Order,
                Blend: a.Blend | b.Blend,
                PartNetwork: a.PartNetwork | b.PartNetwork,
                Value: a.Value | b.Value,
                Exception: a.Exception | b.Exception);

        public static PartNodeComparateResult CalcSigma(IEnumerable<PartNodeComparateResult> results)
        {
            bool b0, b1, b2, b3, b4;
            b0 = b1 = b2 = b3 = b4 = false;

            foreach (PartNodeComparateResult _0 in results)
            {
                b0 |= _0.Order;
                b1 |= _0.Blend;
                b2 |= _0.PartNetwork;
                b3 |= _0.Value;
                b4 |= _0.Exception;
            }

            return new(b0, b1, b2, b3, b4);
        }
    }
}
