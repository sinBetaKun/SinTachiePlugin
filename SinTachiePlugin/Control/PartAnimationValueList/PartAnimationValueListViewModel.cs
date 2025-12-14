using SinTachiePlugin.PartAnimation;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Control.PartAnimationValueList
{
    class PartAnimationValueListViewModel : Bindable, IPropertyEditorControl, IDisposable
    {
        private readonly INotifyPropertyChanged _item;
        private readonly ItemProperty[] _properties;

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public List<PartAnimationValue> Values { get => _values; set => Set(ref _values, value); }
        private List<PartAnimationValue> _values = [];

        public int SelectedIndex { get => _selectedIndex; set => Set(ref _selectedIndex, value); }
        int _selectedIndex = -1;

        public PartAnimationValueListViewModel(ItemProperty[] properties)
        {
            _properties = properties;

            _item = (INotifyPropertyChanged)properties[0].PropertyOwner;
            _item.PropertyChanged += Item_PropertyChanged;

            UpdateValues();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _properties[0].PropertyInfo.Name)
                UpdateValues();
        }

        public void CopyToOtherItems()
        {
            var otherProperties = _properties.Skip(1);
            foreach (var property in otherProperties)
                property.SetValue(Values.Select(x => new PartAnimationValue(x)).ToImmutableList());
        }

        private void UpdateValues()
        {
            ImmutableList<PartAnimationValue> list = _properties[0].GetValue<ImmutableList<PartAnimationValue>>() ?? [];

            if (!Values.SequenceEqual(list))
                Values = [.. list];

            int index = SelectedIndex;
            UpdateValues();
            SelectedIndex = index;
        }

        public void SetProperties(List<PartAnimationValue> list)
        {
            foreach (var property in _properties)
                property.SetValue(list.Select(x => new PartAnimationValue(x)).ToImmutableList());
        }

        public void InsertItems(List<PartAnimationValue> items)
        {
            if (SelectedIndex < -1 || SelectedIndex >= Values.Count)
                return;

            int index = SelectedIndex;
            BeginEdit?.Invoke(this, EventArgs.Empty);
            List<PartAnimationValue> values = [.. Values];
            values.InsertRange(index + 1, items);
            SetProperties(values);
            EndEdit?.Invoke(this, EventArgs.Empty);
            SelectedIndex = index + 1;
        }

        public void RemoveItems(List<PartAnimationValue> items)
        {
            if (items.Count == 0)
                return;

            int index = SelectedIndex;
            BeginEdit?.Invoke(this, EventArgs.Empty);
            List<PartAnimationValue> values = [.. Values];

            foreach (PartAnimationValue value in items)
                values.Remove(value);

            SetProperties(values);
            EndEdit?.Invoke(this, EventArgs.Empty);
            SelectedIndex = Math.Min(index, Values.Count - 1);
        }

        public bool CanMoveUpItem()
        {
            return SelectedIndex > 0;
        }

        public void MoveUpItem()
        {
            if (SelectedIndex < 1)
                return;

            int index = SelectedIndex;
            BeginEdit?.Invoke(this, EventArgs.Empty);
            List<PartAnimationValue> values = [.. Values];
            PartAnimationValue value = values[index];
            values.Remove(value);
            values.Insert(index - 1, value);
            SetProperties(values);
            EndEdit?.Invoke(this, EventArgs.Empty);
            SelectedIndex = index - 1;
        }

        public bool CanMoveDownItem()
        {
            return SelectedIndex < Values.Count;
        }

        public void MoveDownItem()
        {
            if (SelectedIndex >= Values.Count)
                return;

            int index = SelectedIndex;
            BeginEdit?.Invoke(this, EventArgs.Empty);
            List<PartAnimationValue> values = [.. Values];
            PartAnimationValue value = values[index];
            values.Remove(value);
            values.Insert(index + 1, value);
            SetProperties(values);
            EndEdit?.Invoke(this, EventArgs.Empty);
            SelectedIndex = index + 1;
        }

        public void Dispose()
        {
            _item.PropertyChanged -= Item_PropertyChanged;
        }
    }
}
