using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;

namespace SinTachiePlugin.Control.PartValueList
{
    /// <summary>
    /// 描画順序リストコントローラに並べるパーツ達のフォルダ構造を整理する、木構造のノード。
    /// </summary>
    /// <param name="v">このノードに対応させるパーツ</param>
    internal class ValueTreeNode(PartValue v, sbyte selected)
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
        /// 選択状態。最初に選択されたものなら1, 追加選択なら2, 未選択なら0
        /// </summary>
        public readonly sbyte Selected = selected;

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
        /// <returns>木構造をリスト化したものと選択された状態のパーツ</returns>
        public static ValueTreeArgs GetList(IEnumerable<ValueTreeNode> nodes)
        {
            // 返すリスト
            List<PartValue> pv_l0 = [];
            List<PartValue> pv_l1 = [];
            PartValue? pv_0 = null;

            // 渡されたノードを1つずつ
            foreach (ValueTreeNode n_0 in nodes)
            {
                // ノードに対応するパーツ
                PartValue pv_1 = n_0.Value;
                pv_1.ParentIndex = -1;
                pv_1.Depth = 0;
                pv_l0.Add(pv_1);

                switch (n_0.Selected)
                {
                    case 1:
                        pv_0 = pv_1;
                        break;

                    case 2:
                        pv_l1.Add(pv_1);
                        break;
                }

                // 子孫をリストにしたもの
                ValueTreeArgs vta_0 = GetList(n_0.Children);

                // 1つずつ深度と親インデックスを書き換える。
                foreach (PartValue pv_2 in vta_0.List)
                {
                    pv_2.Depth++;
                    pv_2.ParentIndex += pv_l0.Count;
                }

                pv_l0.AddRange(vta_0.List);
                pv_l1.AddRange(vta_0.AdditionalSelecteds);

                if (vta_0.Selected is not null)
                    pv_0 = vta_0.Selected;
            }

            return new(pv_l0, pv_0, pv_l1);
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
        /// <param name="args">変換したいリスト情報</param>
        /// <returns>リストを木構造に変換したもの</returns>
        public static List<ValueTreeNode> GetTree(ValueTreeArgs args)
        {
            List<ValueTreeNode> n_l0 = [];
            List<ValueTreeNode> n_l1 = [];

            foreach (PartValue pv_0 in args.List)
            {
                sbyte sb_0;

                if (args.Selected == pv_0)
                    sb_0 = 1;
                else if (args.AdditionalSelecteds.Contains(pv_0))
                    sb_0 = 2;
                else
                    sb_0 = 0;

                ValueTreeNode n_0 = new(pv_0, sb_0);
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
