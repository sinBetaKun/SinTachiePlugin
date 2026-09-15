using SinTachiePlugin.Part;

namespace SinTachiePlugin.Control.PartValueList
{
    internal class ValueTreeNode
    {
        public readonly PartValue Value;
        public ValueTreeNode? Parent = null;
        public readonly List<ValueTreeNode> Children = [];

        public ValueTreeNode(PartValue v)
        {
            Value = v;
        }

        public ValueTreeNode? Find(PartValue v)
        {
            if (Children.FirstOrDefault(x => x.Value == v) is ValueTreeNode n_0)
            {
                return n_0;
            }

            for (int i_0 = 0; i_0 < Children.Count; i_0++)
            {
                if (Children[i_0].Find(v) is ValueTreeNode n_1)
                    return n_1;
            }

            return null;
        }

        public static List<PartValue> GetList(IEnumerable<ValueTreeNode> nodes)
        {
            List<PartValue> pv_l0 = [];

            foreach (ValueTreeNode n_0 in nodes)
            {
                PartValue pv_0 = n_0.Value;
                pv_0.ParentIndex = -1;
                pv_0.Depth = 0;
                pv_l0.Add(pv_0);
                int i_0 = pv_l0.Count;
                List<PartValue> pv_l1 = GetList(n_0.Children);

                foreach (PartValue pv_1 in pv_l1)
                {
                    pv_1.Depth++;
                    pv_1.ParentIndex += i_0;
                }

                pv_l0.AddRange(pv_l1);
            }

            return pv_l0;
        }

        public bool InsertToHere(List<ValueTreeNode> list)
        {
            if (Value.ControlledParameters.ThisLayerType == Enums.LayerType.Group
                && Children.Count == 0)
            {
                Children.AddRange(list);

                foreach (ValueTreeNode n_0 in list)
                {
                    n_0.Parent = this;
                }

                return false;
            }

            if (Parent is null)
                return true;

            if (Parent.Children.Last() == this)
            {
                Parent.Children.AddRange(list);
            }
            else
            {
                int i_0 = Parent.Children.IndexOf(this);
                Parent.Children.InsertRange(i_0 + 1, list);
            }

            return false;
        }

        public static List<ValueTreeNode> GetTree(IEnumerable<PartValue> list)
        {
            List<ValueTreeNode> n_l0 = [];
            List<ValueTreeNode> n_l1 = [];

            foreach (PartValue pv_0 in list)
            {
                ValueTreeNode n_0 = new(pv_0);
                n_l0.Add(n_0);
                int i_1 = pv_0.ParentIndex;

                if (i_1 >= 0)
                {
                    n_l0[i_1].Children.Add(n_0);
                    n_l1.Add(n_0);
                    n_0.Parent = n_l0[i_1];
                }
            }

            foreach (ValueTreeNode n_0 in n_l1)
                n_l0.Remove(n_0);

            return n_l0;
        }
    }
}
