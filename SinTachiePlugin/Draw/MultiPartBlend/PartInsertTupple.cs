namespace SinTachiePlugin.Draw.MultiPartBlend
{
    internal class PartInsertTupple(MultiPartBlendTupple target)
    {
        public readonly MultiPartBlendTupple Target = target;
        public readonly List<PartInsertTupple> Back = [];
        public readonly List<PartInsertTupple> Front = [];

        public PartInsertTupple? Find(MultiPartBlendTupple t)
        {
            if (Back.FirstOrDefault(x => x.Target == t) is PartInsertTupple pit_0)
                return pit_0;

            if (Front.FirstOrDefault(x => x.Target == t) is PartInsertTupple pit_1)
                return pit_1;

            foreach (PartInsertTupple pit_2 in Back)
                if (pit_2.Find(t) is PartInsertTupple pit_3)
                    return pit_3;

            foreach (PartInsertTupple pit_2 in Front)
                if (pit_2.Find(t) is PartInsertTupple pit_3)
                    return pit_3;

            return null;
        }

        public List<MultiPartBlendTupple> GetList()
        {
            List<MultiPartBlendTupple> ret = [];

            foreach (PartInsertTupple _0 in Back)
                ret.AddRange(_0.GetList());

            ret.Add(Target);

            foreach (PartInsertTupple _0 in Front)
                ret.AddRange(_0.GetList());

            return ret;
        }
    }
}
