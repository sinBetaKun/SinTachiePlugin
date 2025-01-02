using SinTachiePlugin.Informations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.LayerValueListController.Controller
{
    public class LayerValueListControllerViewModel : Bindable, INotifyPropertyChanged, IPropertyEditorControl, IDisposable
    {
        readonly INotifyPropertyChanged item;
        readonly ItemProperty[] properties;

        ImmutableList<LayerValue> blocks = [];

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public ImmutableList<LayerValue> LayerValues { get => blocks; set => Set(ref blocks, value); }
        public int SelectedIndex { get => selectedIndex; set => Set(ref selectedIndex, value); }
        int selectedIndex = -1;

        public ActionCommand AddCommand { get; }
        public ActionCommand RemoveCommand { get; }
        public ActionCommand MoveUpCommand { get; }
        public ActionCommand MoveDownCommand { get; }

        public LayerValueListControllerViewModel(ItemProperty[] properties)
        {
            this.properties = properties;

            item = (INotifyPropertyChanged)properties[0].PropertyOwner;
            item.PropertyChanged += Item_PropertyChanged;

            AddCommand = new ActionCommand(
                _ => true,
                _ =>
                {
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    LayerValues = LayerValues.Insert(tmpSelectedIndex + 1, new LayerValue());
                    foreach (var property in properties)
                        property.SetValue(LayerValues);
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex + 1;
                });

            RemoveCommand = new ActionCommand(
                _ => blocks.Count > 0 && SelectedIndex > -1,
                _ =>
                {
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    LayerValues = LayerValues.RemoveAt(tmpSelectedIndex);
                    foreach (var property in properties)
                        property.SetValue(LayerValues);
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    if (blocks.Count > 0) SelectedIndex = Math.Min(tmpSelectedIndex, blocks.Count - 1);
                });

            MoveUpCommand = new ActionCommand(
                _ => SelectedIndex > 0,
                _ =>
                {
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var clone = LayerValues[SelectedIndex];
                    LayerValues = LayerValues.RemoveAt(tmpSelectedIndex);
                    LayerValues = LayerValues.Insert(tmpSelectedIndex - 1, clone);
                    foreach (var property in properties)
                        property.SetValue(LayerValues);
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex - 1;
                });

            MoveDownCommand = new ActionCommand(
                _ => SelectedIndex < blocks.Count - 1 && SelectedIndex > -1,
                _ =>
                {
                    var tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    var clone = blocks[SelectedIndex];
                    LayerValues = LayerValues.RemoveAt(tmpSelectedIndex);
                    LayerValues = LayerValues.Insert(tmpSelectedIndex + 1, clone);
                    foreach (var property in properties)
                        property.SetValue(LayerValues);
                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex + 1;
                });

            UpdateBlocks();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == properties[0].PropertyInfo.Name)
                UpdateBlocks();
        }

        void UpdateBlocks()
        {
            var values = properties[0].GetValue<ImmutableList<LayerValue>>() ?? [];
            if (!LayerValues.SequenceEqual(values))
            {
                LayerValues = [.. values];
            }

            var commands = new[] { AddCommand, RemoveCommand, MoveUpCommand, MoveDownCommand };
            foreach (var command in commands)
                command.RaiseCanExecuteChanged();
        }

        public void CopyToOtherItems()
        {
            //現在のアイテムの内容を他のアイテムにコピーする
            var otherProperties = properties.Skip(1);
            foreach (var property in otherProperties)
                property.SetValue(LayerValues);
        }

        public void Dispose()
        {
            item.PropertyChanged -= Item_PropertyChanged;
        }
    }
}
