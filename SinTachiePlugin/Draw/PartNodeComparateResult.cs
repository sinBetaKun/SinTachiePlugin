namespace SinTachiePlugin.Draw
{
    /// <summary>
    /// パーツのパラメータが変化したとき、何を更新するべきかをまとめるレコード
    /// </summary>
    /// <param name="Order">描画順序に影響する変化があったか(更新が確定しているわけではない)</param>
    /// <param name="Blend">合成モードに変化があったか</param>
    /// <param name="PartNetwork">親子関係に影響する値の変化があったか</param>
    /// <param name="Value">そのパーツ自身を更新する必要のある値の変化があったか</param>
    /// <param name="Exception">更新時に例外が発生したか</param>
    internal record PartNodeComparateResult(
        bool Order = false,
        bool Blend = false,
        bool PartNetwork = false,
        bool Value = false,
        bool Exception = false)
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
            bool b_4o, b_4b, b_4n, b_4v, b_4e;
            b_4o = b_4b = b_4n = b_4v = b_4e = false;

            foreach (PartNodeComparateResult r_0 in results)
            {
                b_4o |= r_0.Order;
                b_4b |= r_0.Blend;
                b_4n |= r_0.PartNetwork;
                b_4v |= r_0.Value;
                b_4e |= r_0.Exception;
            }

            return new(
                Order: b_4o,
                Blend: b_4b,
                PartNetwork: b_4n,
                Value: b_4v,
                Exception: b_4e);
        }

        public bool AnyParamChanged() => Order | Blend | PartNetwork | Value;

        public static PartNodeComparateResult AllChanged()
            => new(
                Order: true,
                Blend: true,
                PartNetwork: true,
                Value: true);
    }
}
