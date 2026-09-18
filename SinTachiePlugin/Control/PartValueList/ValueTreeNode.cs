using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;

namespace SinTachiePlugin.Control.PartValueList
{
    /// <summary>
    /// 描画順序リストコントローラに並べるパーツ達のフォルダ構造を整理する、木構造のノード。
    /// </summary>
    /// <param name="v">このノードに対応させるパーツ</param>
    internal class ValueTreeNode(PartValue v)
    {
        /// <summary>
        /// このノードが対応するパーツ
        /// </summary>
        public readonly PartValue Value = v;

        /// <summary>
        /// 親ノード
        /// </summary>
        public ValueTreeNode? Parent = null;
        
        /// <summary>
        /// 子ノード
        /// </summary>
        public readonly List<ValueTreeNode> Children = [];

        /// <summary>
        /// 構造内からパーツに対応するノードを探し出す。
        /// </summary>
        /// <param name="v">このパーツに対応するノードを探す。</param>
        /// <returns>見つけたノード</returns>
        public ValueTreeNode? Find(PartValue v)
        {
            // 子ノードのいずれかが該当するか確かめる。
            if (Children.FirstOrDefault(x => x.Value == v) is ValueTreeNode n_0)
            {
                return n_0;
            }

            // 子ノードの子孫を探索する。
            for (int i_0 = 0; i_0 < Children.Count; i_0++)
            {
                // 見つけた場合
                if (Children[i_0].Find(v) is ValueTreeNode n_1)
                    return n_1;
            }

            // 見つからなかった場合
            return null;
        }

        /// <summary>
        /// 木構造をパーツリストに戻す。
        /// </summary>
        /// <param name="nodes">リストにしたい木構造</param>
        /// <returns>木構造をリスト化したもの</returns>
        public static List<PartValue> GetList(IEnumerable<ValueTreeNode> nodes)
        {
            // 返すリスト
            List<PartValue> pv_l0 = [];

            // 渡されたノードを1つずつ
            foreach (ValueTreeNode n_0 in nodes)
            {
                // ノードに対応するパーツ
                PartValue pv_0 = n_0.Value;
                pv_0.ParentIndex = -1;
                pv_0.Depth = 0;
                pv_l0.Add(pv_0);

                // 子孫をリストにしたもの
                List<PartValue> pv_l1 = GetList(n_0.Children);

                // 1つずつ深度と親インデックスを書き換える。
                foreach (PartValue pv_1 in pv_l1)
                {
                    pv_1.Depth++;
                    pv_1.ParentIndex += pv_l0.Count;
                }

                pv_l0.AddRange(pv_l1);
            }

            return pv_l0;
        }

        /// <summary>
        /// 木構造のうち表示するパーツのリストを作る。
        /// </summary>
        /// <param name="nodes">リストにしたい木構造</param>
        /// <param name="count">全て表示する場合のリストの長さ</param>
        /// <returns>木構造をリスト化したもの</returns>
        public static List<PartValue> GetVisibleList(IEnumerable<ValueTreeNode> nodes, out int count)
        {
            // 返すリスト
            List<PartValue> pv_l0 = [];

            // 全て表示する場合のリストの長さ
            int i_0 = 0;

            // 渡されたノードを1つずつ
            foreach (ValueTreeNode n_0 in nodes)
            {
                // ノードに対応するパーツ
                PartValue pv_0 = n_0.Value;
                pv_0.ParentIndex = -1;
                pv_0.Depth = 0;
                pv_l0.Add(pv_0);
                i_0++;

                // 子孫をリストにしたもの
                List<PartValue> pv_l1 = GetVisibleList(n_0.Children, out int i_1);

                // 1つずつ深度と親インデックスを書き換える。
                foreach (PartValue pv_1 in pv_l1)
                {
                    pv_1.Depth++;
                    pv_1.ParentIndex += i_0;
                }

                if (pv_0.ControlledParameters.SourceSelectArg is GroupParameter gp
                    && gp.IsOpened)
                {
                    pv_l0.AddRange(pv_l1);
                }

                i_0 += i_1;
            }

            count = i_0;
            return pv_l0;
        }

        /// <summary>
        /// このノードの隣にノード群を挿入することを試す。
        /// </summary>
        /// <param name="list">挿入するノード群</param>
        /// <returns>挿入に失敗した場合、真。</returns>
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

        /// <summary>
        /// パーツリストを木構造に変換する。
        /// </summary>
        /// <param name="list">変換したいリスト</param>
        /// <returns>リストを木構造に変換したもの</returns>
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
