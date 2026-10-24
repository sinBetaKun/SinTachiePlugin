using SinTachiePlugin.CharacterAsset;
using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Part;
using SinTachiePlugin.Parts;
using SinTachiePlugin.Properties;
using SinTachiePlugin.ShapePludin;
using System.Collections.Immutable;
using System.ComponentModel;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Control.PartValueList
{
    internal class PartValueListViewModel : Bindable, IPropertyEditorControl, IDisposable
    {
        private readonly INotifyPropertyChanged _item;
        private readonly ItemProperty[] _properties;
        private readonly bool _isShape;

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public List<PartValue> PartValues
        {
            get => _parts;
            set
            {
                PartValue[] pv_a0 = [.. _parts.Where(x => !value.Contains(x))];
                PartValue[] pv_a1 = [.. value.Where(x => !_parts.Contains(x))];
                Set(ref _parts, value);

                foreach (PartValue pv_0 in pv_a0)
                    pv_0.ControlledParameters.LayerTypeChanged -= ExtractFromBrokenGroup;

                foreach (PartValue pv_0 in pv_a1)
                    pv_0.ControlledParameters.LayerTypeChanged += ExtractFromBrokenGroup;
            }
        }
        private List<PartValue> _parts = [];

        private void ExtractFromBrokenGroup(object? sender, EventArgs e)
        {
            if (sender is not ControlledParametersOfPart cp_0)
                return;

            if (cp_0.ThisLayerType == LayerType.Group)
                return;

            if (PartValues.FirstOrDefault(x => x.ControlledParameters == cp_0) is not PartValue pv_0)
                return;

            List<ValueTreeNode> n_l0 = ValueTreeNode.GetTree(new(PartValues, null, []));

            if (n_l0.FirstOrDefault(x => x.Value == pv_0) is ValueTreeNode n_0)
            {
                if (n_0.Children.Count == 0)
                    return;

                if (n_l0.Last() == n_0)
                {
                    n_l0.AddRange(n_0.Children);
                }
                else
                {
                    int i_0 = n_l0.IndexOf(n_0);
                    n_l0.InsertRange(i_0 + 1, n_0.Children);
                }

                n_0.Children.Clear();
            }
            else
            {
                foreach (ValueTreeNode n_1 in n_l0)
                {
                    if (n_1.Find(pv_0) is null)
                        continue;

                    if (n_1.Children.Count == 0)
                        return;

                    if (n_1.Parent is not ValueTreeNode n_2)
                        return;

                    if (n_2.Children.Last() == n_1)
                    {
                        n_2.Children.AddRange(n_1.Children);
                    }
                    else
                    {
                        int i_0 = n_2.Children.IndexOf(n_1);
                        n_2.Children.InsertRange(i_0 + 1, n_1.Children);
                    }

                    n_1.Children.Clear();
                }
            }

            PartValues = ValueTreeNode.GetList(n_l0).List;
            PartValue pv_1 = Source[SelectedIndex];
            UpdateSource();
            SetProperties();
            SelectedIndex = Source.IndexOf(pv_1);
        }

        public List<PartValue> Source { get => _source; set => Set(ref _source, value); }
        private List<PartValue> _source = [];

        public int SelectedIndex
        {
            get => selectedIndex;
            set => Set(ref selectedIndex, value);
        }
        int selectedIndex = -1;

        public ImmutableList<int> SelectedIndexes { get; set => Set(ref field, value); } = [];

        //private readonly List<PartValue> _openeds = [];

        public ImmutableList<CharacterAssetsData> Assets { get => _assets; set => Set(ref _assets, value); }
        private ImmutableList<CharacterAssetsData> _assets = [];

        public SinTachieCharacterParameter? CharacterParameter
        {
            get => _characterParameter;
            set
            {
                var oldCP = _characterParameter;
                if (Set(ref _characterParameter, value))
                {
                    oldCP?.PropertyChanged -= CharacterParameterChanged;
                    _characterParameter?.PropertyChanged += CharacterParameterChanged;
                    SetAssetsFromCharacterParameter();
                }
            }
        }
        private SinTachieCharacterParameter? _characterParameter;

        private readonly PartValuesAndAssets? _partValuesAndAssets;

        private void CharacterParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            SetAssetsFromCharacterParameter();
        }

        private void SetAssetsFromCharacterParameter()
        {
            Assets = _characterParameter?.Assets ?? [];
        }

        public PartValueListViewModel(ItemProperty[] properties, PartValuesAndAssets? partValuesAndRoot = null)
        {
            _properties = properties;

            _item = (INotifyPropertyChanged)properties[0].PropertyOwner;
            _item.PropertyChanged += Item_PropertyChanged;

            if (partValuesAndRoot is not null)
            {
                _partValuesAndAssets = partValuesAndRoot;
                _partValuesAndAssets.AssetsChanged += RootOfShapeChanged;
            }

            _isShape = partValuesAndRoot is not null;

            UpdateParts();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _properties[0].PropertyInfo.Name)
                UpdateParts();
        }

        private void RootOfShapeChanged(object? sender, EventArgs e)
        {
            Assets = _partValuesAndAssets!.Assets;
        }

        private void UpdateParts()
        {
            if (_isShape)
            {
                PartValuesAndAssets par = _properties[0].GetValue<PartValuesAndAssets>() ?? new();

                if (!PartValues.SequenceEqual(par.PartValues))
                    PartValues = [.. par.PartValues];
            }
            else
            {
                ImmutableList<PartValue> list = _properties[0].GetValue<ImmutableList<PartValue>>() ?? [];

                if (!PartValues.SequenceEqual(list))
                    PartValues = [.. list];
            }

            int index = SelectedIndex;
            UpdateSource();
            SelectedIndex = index;
        }

        public void UpdateSource()
        {
            Source = [.. ValueTreeNode.GetVisibleList(ValueTreeNode.GetTree(new(PartValues, null, [])), out int _)];
        }

        public void UpdateSource(ValueTreeArgs vta)
        {
            Source = [.. ValueTreeNode.GetVisibleList(ValueTreeNode.GetTree(vta), out int _)];
            SelectedIndexes = [.. vta.AdditionalSelecteds.Prepend(vta.Selected).Select(x => Source.IndexOf(x!))];
        }

        public void CopyToOtherItems()
        {
            var otherProperties = _properties.Skip(1);
            foreach (var property in otherProperties)
                property.SetValue(PartValues.Select(pv => new PartValue(pv)).ToImmutableList());
        }

        public void SetProperties()
        {
            foreach (var property in _properties)
                property.SetValue(PartValues.Select(pv => new PartValue(pv)).ToImmutableList());
        }

        public List<PartValue> GetDescendants(PartValue pv1)
        {
            if (!PartValues.Contains(pv1))
                return [];

            int index1 = PartValues.IndexOf(pv1);
            int index2 = index1 + 1;

            if (index2 == PartValues.Count)
                return [];

            List<PartValue> list1 = [];

            while (PartValues[index2].ParentIndex == index1)
            {
                PartValue pv2 = PartValues[index2];
                list1.Add(pv2);
                List<PartValue> list2 = GetDescendants(pv2);
                list1.AddRange(list2);
                index2 += list2.Count + 1;

                if (index2 == PartValues.Count)
                    break;
            }

            return list1;
        }

        private (List<ValueTreeNode>, List<ValueTreeNode>) FindSelectedNodes(List<PartValue> items, PartValue? lastSelected)
        {
            List<ValueTreeNode> n_l0 = ValueTreeNode.GetTree(new(PartValues, lastSelected, [.. items.Where(x => x != lastSelected)]));
            List<ValueTreeNode> n_l1 = [];

            foreach (PartValue pv_0 in items)
            {
                if (n_l0.FirstOrDefault(x => x.Value == pv_0) is ValueTreeNode n_0)
                    n_l1.Add(n_0);
                else
                    foreach (ValueTreeNode n_1 in n_l0)
                        if (n_1.Find(pv_0) is ValueTreeNode n_2)
                            n_l1.Add(n_2);
            }

            return (n_l0, n_l1);
        }

        public void InsertItems(List<PartValue> items, PartValue lastSelected)
        {
            ValueTreeArgs vta_0 = new(items, lastSelected, [.. items.Except([lastSelected])]);

            if (SelectedIndex < -1 || SelectedIndex >= Source.Count)
                return;

            List<ValueTreeNode> n_l0 = ValueTreeNode.GetTree(new(PartValues, null, []));
            List<ValueTreeNode> n_l1 = ValueTreeNode.GetTree(vta_0);

            if (SelectedIndex < 0)
            {
                n_l0.AddRange(n_l1);
            }
            else
            {
                PartValue pv_0 = Source[SelectedIndex];

                if (n_l0.FirstOrDefault(x => x.Value == pv_0) is ValueTreeNode n_0)
                {
                    if (n_0.InsertToHere(n_l1))
                    {
                        if (n_l0.Last() == n_0)
                        {
                            n_l0.AddRange(n_l1);
                        }
                        else
                        {
                            int i_0 = n_l0.IndexOf(n_0);
                            n_l0.InsertRange(i_0 + 1, n_l1);
                        }
                    }
                }
                else
                {
                    ValueTreeNode? n_1 = null;

                    foreach (ValueTreeNode n_2 in n_l0)
                    {
                        if (n_2.Find(pv_0) is ValueTreeNode n_3)
                        {
                            n_1 = n_3;
                            break;
                        }
                    }

                    if (n_1 is null)
                    {
                        SinTachieDialog.ShowError(new(TextResource.PartValueListViewModel_Error_FailToGenerateValueTree_0));
                        throw new Exception(TextResource.PartValueListViewModel_Error_FailToGenerateValueTree_0);
                    }

                    if (n_1.InsertToHere(n_l1))
                    {
                        SinTachieDialog.ShowError(new(TextResource.PartValueListViewModel_Error_FailToGenerateValueTree_1));
                        throw new Exception(TextResource.PartValueListViewModel_Error_FailToGenerateValueTree_1);
                    }
                }
            }

            BeginEdit?.Invoke(this, EventArgs.Empty);
            ValueTreeArgs vta_1 = ValueTreeNode.GetList(n_l0);
            PartValues = vta_1.List;
            UpdateSource(vta_1);
            SetProperties();
            EndEdit?.Invoke(this, EventArgs.Empty);
            return;
        }

        public void RemoveItems(IEnumerable<PartValue> items)
        {
            if (SelectedIndex < -1 || SelectedIndex >= Source.Count)
                return;

            (List<ValueTreeNode> n_l0, List<ValueTreeNode> n_l1) = FindSelectedNodes([.. items], null);

            foreach (ValueTreeNode n_0 in n_l1)
            {
                if (n_0.Parent is ValueTreeNode n_1)
                {
                    n_1.Children.Remove(n_0);
                }
                else
                {
                    n_l0.Remove(n_0);
                }
            }

            BeginEdit?.Invoke(this, EventArgs.Empty);
            PartValues = ValueTreeNode.GetList(n_l0).List;
            UpdateSource();
            SetProperties();
            EndEdit?.Invoke(this, EventArgs.Empty);
        }

        public ItemMovable CanMoveItem(IEnumerable<PartValue> items)
        {
            (List<ValueTreeNode> n_l0, List<ValueTreeNode> n_l1) = FindSelectedNodes([.. items], null);

            if (n_l1.Count == 0)
                return ItemMovable.Neither;

            if (n_l1.Select(x => x.Parent).Distinct().Count() > 1)
                return ItemMovable.Hazard;

            List<ValueTreeNode> n_l2 = n_l1.First().Parent?.Children ?? n_l0;

            if (n_l1.Contains(n_l2.First()))
            {
                if (n_l1.Contains(n_l2.Last()))
                {
                    return ItemMovable.Neither;
                }
                else
                {
                    return ItemMovable.OnlyDown;
                }
            }
            else
            {
                if (n_l1.Contains(n_l2.Last()))
                {
                    return ItemMovable.OnlyUp;
                }
                else
                {
                    return ItemMovable.Both;
                }
            }
        }

        public void MoveItems(IEnumerable<PartValue> items, PartValue lastSelected, bool isUp)
        {
            PartValue pv_0 = Source[SelectedIndex];
            (List<ValueTreeNode> n_l0, List<ValueTreeNode> n_l1) = FindSelectedNodes([.. items], lastSelected); // CanMoveUp/DownItemがtrueを返す前提
            List<ValueTreeNode> n_l2 = n_l1.First().Parent?.Children ?? n_l0;
            (int, ValueTreeNode)[] tp_a0 = [.. n_l1.Select(x => (n_l2.IndexOf(x), x)).OrderBy(x => x.Item1)];
            (int, ValueTreeNode)[] tp_a1 = new (int, ValueTreeNode)[tp_a0.Length];
            tp_a1[0] = tp_a0[0];

            for (int i_0 = 1; i_0 < tp_a0.Length; i_0++)
                tp_a1[i_0] = (tp_a0[i_0].Item1 - tp_a0[i_0 - 1].Item1 - 1, tp_a0[i_0].Item2);

            List<ValueTreeNode> n_l3 = [.. n_l2.Except(n_l1)];
            List<ValueTreeNode> n_l4 = [.. n_l3[..(tp_a1[0].Item1 + (isUp ? -1 : 1))]];
            n_l3.RemoveRange(0, n_l4.Count);
            n_l4.Add(tp_a1[0].Item2);

            foreach ((int i_0, ValueTreeNode n_0) in tp_a1.Skip(1))
            {
                IEnumerable<ValueTreeNode> n_e0 = n_l3.Take(i_0);
                n_l3.RemoveRange(0, i_0);
                n_l4.AddRange(n_e0);
                n_l4.Add(n_0);
            }

            n_l4.AddRange(n_l3);
            n_l2.Clear();
            n_l2.AddRange(n_l4);

            BeginEdit?.Invoke(this, EventArgs.Empty);
            ValueTreeArgs vta_0 = ValueTreeNode.GetList(n_l0);
            PartValues = vta_0.List;
            UpdateSource(vta_0);
            SetProperties();
            EndEdit?.Invoke(this, EventArgs.Empty);
            return;
        }

        public void ShowOnlySelectedParts(List<PartValue> items)
        {
            foreach (PartValue pv in PartValues)
                pv.ControlledParameters.Hide = !items.Contains(pv);
        }

        public void ShowAllParts()
        {
            foreach (PartValue pv in PartValues)
                pv.ControlledParameters.Hide = false;
        }

        public void Dispose()
        {
            _item.PropertyChanged -= Item_PropertyChanged;

            if (_partValuesAndAssets is not null)
                _partValuesAndAssets.AssetsChanged -= RootOfShapeChanged;
        }
    }
}
