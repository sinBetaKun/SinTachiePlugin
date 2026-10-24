namespace SinTachiePlugin.Draw.MultiPartBlend
{
    /// <summary>
    /// パーツの挿入を行うためのデータを処理するクラス
    /// </summary>
    /// <param name="target">挿入する先となるパーツ</param>
    internal class PartInsertTuple(MultiPartBlendTuple target)
    {
        /// <summary>
        /// ターゲットとなる MultiPartBlendTupple を格納する読み取り専用フィールド。
        /// </summary>
        public readonly MultiPartBlendTuple Target = target;

        /// <summary>
        /// Targetの奥に描画するパーツたち
        /// </summary>
        public readonly List<PartInsertTuple> Back = [];

        /// <summary>
        /// Targetの手前に描画するパーツたち
        /// </summary>
        public readonly List<PartInsertTuple> Front = [];

        public PartInsertTuple? Find(MultiPartBlendTuple t)
        {
            if (Back.FirstOrDefault(x => x.Target == t) is PartInsertTuple pit_0)
                return pit_0;

            if (Front.FirstOrDefault(x => x.Target == t) is PartInsertTuple pit_1)
                return pit_1;

            foreach (PartInsertTuple pit_2 in Back)
                if (pit_2.Find(t) is PartInsertTuple pit_3)
                    return pit_3;

            foreach (PartInsertTuple pit_2 in Front)
                if (pit_2.Find(t) is PartInsertTuple pit_3)
                    return pit_3;

            return null;
        }

        public List<MultiPartBlendTuple> GetList()
        {
            List<MultiPartBlendTuple> ret = [];

            foreach (PartInsertTuple _0 in Back)
                ret.AddRange(_0.GetList());

            ret.Add(Target);

            foreach (PartInsertTuple _0 in Front)
                ret.AddRange(_0.GetList());

            return ret;
        }
    }
}
