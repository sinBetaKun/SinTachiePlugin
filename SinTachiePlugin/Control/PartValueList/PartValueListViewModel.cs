using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using SinTachiePlugin.Parts;
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

        public List<PartValue> PartValues { get => _parts; set => Set(ref _parts, value); }
        private List<PartValue> _parts = [];

        public List<PartValue> Source { get => _source; set => Set(ref _source, value); }
        private List<PartValue> _source = [];

        public int SelectedIndex { get => selectedIndex; set => Set(ref selectedIndex, value); }
        int selectedIndex = -1;

        private readonly List<PartValue> _openeds = [];

        public string Root { get => _root; set => Set(ref _root, value); }
        string _root = string.Empty;

        public SinTachieCharacterParameter? CharacterParameter
        {
            get => _characterParameter;
            set
            {
                var oldCP = _characterParameter;
                if (Set(ref _characterParameter, value))
                {
                    if (oldCP != null)
                    {
                        oldCP.PropertyChanged -= CharacterParameterChanged;
                    }
                    if (_characterParameter != null)
                    {
                        _characterParameter.PropertyChanged += CharacterParameterChanged;
                    }
                    SetRootFromCharacterParameter();
                }
            }
        }
        private SinTachieCharacterParameter? _characterParameter;

        private PartValuesAndRoot? _partValuesAndRoot;

        private void CharacterParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            SetRootFromCharacterParameter();
        }

        private void SetRootFromCharacterParameter()
        {
            Root = _characterParameter?.Directory ?? string.Empty;
        }

        public PartValueListViewModel(ItemProperty[] properties, PartValuesAndRoot? partValuesAndRoot = null)
        {
            _properties = properties;

            _item = (INotifyPropertyChanged)properties[0].PropertyOwner;
            _item.PropertyChanged += Item_PropertyChanged;

            if (partValuesAndRoot is not null)
            {
                _partValuesAndRoot = partValuesAndRoot;
                _partValuesAndRoot.RootChanged += RootOfShapeChanged;
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
            Root = _partValuesAndRoot!.Root;
        }

        private void UpdateParts()
        {
            if (_isShape)
            {
                PartValuesAndRoot par = _properties[0].GetValue<PartValuesAndRoot>() ?? new();

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
            FindOpendGroup();
            UpdateSource();
            SelectedIndex = index;
        }

        private void FindOpendGroup()
        {
            _openeds.Clear();

            foreach (PartValue pv in PartValues)
                if (pv.ControlledParameters.SourceSelectArg is GroupParameter gp && gp.IsOpened)
                    _openeds.Add(pv);
        }

        public void UpdateSource()
        {
            Source = [.. GetVisibleItem()];
        }

        public List<PartValue> GetVisibleItem(int parentIndex = -1, int depth = 0)
        {
            List<PartValue> list = [];

            for (int i = 0; i < PartValues.Count; i++)
            {
                PartValue pv = PartValues[i];

                if (pv.ParentIndex == parentIndex)
                {
                    list.Add(pv);
                    pv.Depth = depth;

                    if (pv.ControlledParameters.SourceSelectArg is GroupParameter gp && gp.IsOpened)
                        list.AddRange(GetVisibleItem(i, depth + 1));
                }
            }

            return list;
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

        public void AddOpenedGroup(PartValue pv)
        {
            if (pv.ControlledParameters.SourceSelectArg is not GroupParameter gp)
                return;

            gp.IsOpened = true;
            _openeds.Add(pv);
            UpdateSource();
        }

        public void RemoveOpenedGroup(PartValue pv)
        {
            if (pv.ControlledParameters.SourceSelectArg is not GroupParameter gp)
                return;

            gp.IsOpened = false;
            _openeds.Remove(pv);
            UpdateSource();
        }

        public IEnumerable<PartValue> FindChildren(PartValue pv)
        {
            return PartValues.Where(pv => pv.ParentIndex == PartValues.IndexOf(pv));
        }

        public void InsertItems(List<PartValue> items)
        {
            if (SelectedIndex < -1 || SelectedIndex >= Source.Count)
                return;

            PartValue first = items.First();
            int index = SelectedIndex;
            bool flag = false;

            BeginEdit?.Invoke(this, EventArgs.Empty);

            if (index < 0)
            {
                foreach (PartValue pv1 in items)
                    if (pv1.ParentIndex > -1)
                        pv1.ParentIndex += PartValues.Count;

                PartValues.AddRange(items);
                flag = true;
            }
            else if (Source[index].ControlledParameters.SourceSelectArg is GroupParameter)
            {
                PartValue pv1 = Source[index];
                PartValue[] children = [.. FindChildren(pv1)];

                if (children.Length == 0)
                {
                    int index2 = PartValues.IndexOf(pv1);

                    foreach (PartValue pv in PartValues)
                        if (pv.ParentIndex > index2)
                            pv.ParentIndex += items.Count;

                    foreach (PartValue pv2 in items)
                    {
                        if (pv2.ParentIndex < 0)
                            pv2.ParentIndex = index2;
                        else
                            pv2.ParentIndex += index2 + 1;
                    }

                    int index3 = PartValues.IndexOf(pv1) + 1;
                    if (index3 < PartValues.Count)
                        PartValues.InsertRange(index3, items);
                    else
                        PartValues.AddRange(items);


                    flag = true;
                }
            }

            if (!flag)
            {
                PartValue target = Source[index];
                int index2 = PartValues.IndexOf(target);
                int index3 = index2 + GetDescendants(target).Count + 1;

                foreach (PartValue pv in items)
                    if (pv.ParentIndex > -1)
                        pv.ParentIndex += index3;

                if (target.ParentIndex > -1)
                    foreach (PartValue pv in items)
                        if (pv.ParentIndex < 0)
                            pv.ParentIndex = target.ParentIndex;

                for (int i = index3; i < PartValues.Count; i++)
                    if (PartValues[i].ParentIndex > -1)
                        PartValues[i].ParentIndex += items.Count;

                if (index2 + 1 == PartValues.Count)
                    PartValues.AddRange(items);
                else
                    PartValues.InsertRange(index3, items);
            }

            UpdateSource();
            int index4 = Source.IndexOf(first);
            SetProperties();
            UpdateSource();
            EndEdit?.Invoke(this, EventArgs.Empty);
            SelectedIndex = index4;
        }

        public void RemoveItems(IEnumerable<PartValue> items)
        {
            int index = SelectedIndex;

            BeginEdit?.Invoke(this, EventArgs.Empty);

            HashSet<PartValue> hash = [];

            foreach (PartValue pv1 in items)
            {
                List<PartValue> list1 = [pv1];
                List<PartValue> list2 = [];

                while (list1.Count > 0)
                {
                    foreach (PartValue pv2 in list1)
                    {
                        if (pv2.ControlledParameters.SourceSelectArg is GroupParameter)
                            list2.AddRange(FindChildren(pv2));

                        hash.Add(pv2);
                    }

                    list1 = list2;
                    list2 = [];
                }
            }

            foreach (PartValue pv1 in hash)
            {
                int index2 = PartValues.IndexOf(pv1);

                for (int i = index2; i < PartValues.Count; i++)
                {
                    PartValue pv2 = PartValues[i];

                    if (pv2.ParentIndex > index2)
                        pv2.ParentIndex--;
                }

                PartValues.Remove(pv1);
            }

            UpdateSource();
            int index3 = (index < Source.Count - 1) ? index : -1;
            SetProperties();
            UpdateSource();
            EndEdit?.Invoke(this, EventArgs.Empty);
            SelectedIndex = index3;
        }

        public bool CanMoveUpItem()
        {
            int index = SelectedIndex;
            PartValue target = Source[index];
            List<PartValue> list1 = [.. PartValues.Where(pv => pv.ParentIndex == target.ParentIndex)];
            return list1.IndexOf(target) > 0;
        }

        public void MoveUpItem()
        {
            int index1 = SelectedIndex;
            PartValue target = Source[index1];

            if (target.ParentIndex > -1 && target.ParentIndex < PartValues.Count)
            {
                PartValue pv = PartValues[target.ParentIndex];

                if (pv.ControlledParameters.SourceSelectArg is GroupParameter)
                    if (FindChildren(pv).ToList().IndexOf(target) < 1)
                        return;
            }
            else
            {
                if (PartValues.IndexOf(target) < 1)
                    return;
            }

            int index2 = PartValues.IndexOf(target);
            List<PartValue> list1 = [target, .. GetDescendants(target)];
            List<PartValue> list2 = [.. PartValues.Where(pv => pv.ParentIndex == target.ParentIndex)];
            PartValue pv1 = list2[list2.IndexOf(target) - 1];
            List<PartValue> list3 = [pv1, .. GetDescendants(pv1)];
            int index3 = PartValues.IndexOf(pv1);

            for (int i = 1; i < list1.Count; i++)
                list1[i].ParentIndex -= list3.Count;

            for (int i = 1; i < list3.Count; i++)
                list3[i].ParentIndex += list1.Count;

            BeginEdit?.Invoke(this, EventArgs.Empty);

            foreach (PartValue pv2 in list3)
                PartValues.Remove(pv2);

            foreach (PartValue pv2 in list1)
                PartValues.Remove(pv2);

            if (PartValues.Count > index3)
                PartValues.InsertRange(index3, [.. list1, .. list3]);
            else
                PartValues.AddRange([.. list1, .. list3]);

            UpdateSource();
            int index4 = Source.IndexOf(target);
            SetProperties();
            UpdateSource();
            EndEdit?.Invoke(this, EventArgs.Empty);
            SelectedIndex = index4;
        }

        public bool CanMoveDownItem()
        {
            int index = SelectedIndex;
            PartValue target = Source[index];
            List<PartValue> list1 = [.. PartValues.Where(pv => pv.ParentIndex == target.ParentIndex)];
            return list1.IndexOf(target) < list1.Count - 1;
        }

        public void MoveDownItem()
        {
            int index1 = SelectedIndex;
            PartValue target = Source[index1];

            if (target.ParentIndex > -1 && target.ParentIndex < PartValues.Count)
            {
                PartValue pv = PartValues[target.ParentIndex];

                if (pv.ControlledParameters.SourceSelectArg is GroupParameter)
                {
                    List<PartValue> list = [.. FindChildren(pv)];
                    if (list.IndexOf(target) > list.Count - 2)
                        return;
                }
            }
            else
            {
                if (PartValues.IndexOf(target) > PartValues.Count - 2)
                    return;
            }

            int index2 = PartValues.IndexOf(target);
            List<PartValue> list1 = [target, .. GetDescendants(target)];
            List<PartValue> list2 = [.. PartValues.Where(pv => pv.ParentIndex == target.ParentIndex)];
            PartValue pv1 = list2[list2.IndexOf(target) + 1];
            List<PartValue> list3 = [pv1, .. GetDescendants(pv1)];
            int index3 = PartValues.IndexOf(pv1);

            for (int i = 1; i < list1.Count; i++)
                list1[i].ParentIndex += list3.Count;

            for (int i = 1; i < list3.Count; i++)
                list3[i].ParentIndex -= list1.Count;

            BeginEdit?.Invoke(this, EventArgs.Empty);


            foreach (PartValue pv2 in list1)
                PartValues.Remove(pv2);

            foreach (PartValue pv2 in list3)
                PartValues.Remove(pv2);

            if (PartValues.Count > index2)
                PartValues.InsertRange(index2, [.. list3, .. list1]);
            else
                PartValues.AddRange([.. list3, .. list1]);

            UpdateSource();
            int index4 = Source.IndexOf(target);
            SetProperties();
            UpdateSource();
            EndEdit?.Invoke(this, EventArgs.Empty);
            SelectedIndex = index4;
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

            if (_partValuesAndRoot is not null)
                _partValuesAndRoot.RootChanged -= RootOfShapeChanged;
        }
    }
}
