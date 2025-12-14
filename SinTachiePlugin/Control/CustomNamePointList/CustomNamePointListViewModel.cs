using SinTachiePlugin.Part.CustomPoint.CustomPointArg;
using SinTachiePlugin.Properties;
using System.Collections.Immutable;
using System.ComponentModel;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Control.CustomNamePointList
{
    internal class CustomNamePointListViewModel : Bindable, INotifyPropertyChanged, IPropertyEditorControl, IDisposable
    {
        private readonly INotifyPropertyChanged _item;
        private readonly ItemProperty[] _properties;

        public List<CustomNamePoint> Points { get => _points; set => Set(ref _points, value); }
        private List<CustomNamePoint> _points = [];

        public int SelectedIndex { get => _selectedIndex; set => Set(ref _selectedIndex, value); }
        private int _selectedIndex = 0;

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public ActionCommand AddCommand { get; }
        public ActionCommand RemoveCommand { get; }
        public ActionCommand MoveUpCommand { get; }
        public ActionCommand MoveDownCommand { get; }

        public CustomNamePointListViewModel(ItemProperty[] properties)
        {
            _properties = properties;
            _item = (INotifyPropertyChanged)properties[0].PropertyOwner;
            _item.PropertyChanged += Item_PropertyChanged;

            AddCommand = new ActionCommand(
                _ => true,
                _ =>
                {
                    int tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    int number = 1;
                    
                    while (Points.Any(p => p.Name == TextResource.CustomNamePoint_DefaultName + $"({number})"))
                        number++;

                    string name = TextResource.CustomNamePoint_DefaultName + $"({number})";
                    List<CustomNamePoint> points = [.. Points];
                    points.Insert(tmpSelectedIndex + 1, new CustomNamePoint(name, 0, 0));

                    foreach (ItemProperty property in properties)
                        property.SetValue(points.Select(x => new CustomNamePoint(x)).ToImmutableList());

                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex + 1;
                });

            RemoveCommand = new ActionCommand(
                _ => _points.Count > 0,
                _ =>
                {
                    int tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    List<CustomNamePoint> points = [.. Points];
                    points.RemoveAt(SelectedIndex);

                    foreach (ItemProperty property in properties)
                        property.SetValue(points.Select(x => new CustomNamePoint(x)).ToImmutableList());

                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = Math.Min(tmpSelectedIndex, points.Count - 1);
                });

            MoveUpCommand = new ActionCommand(
                _ => SelectedIndex > 0,
                _ =>
                {
                    int tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    List<CustomNamePoint> points = [.. Points];
                    CustomNamePoint point = points[tmpSelectedIndex];
                    points.RemoveAt(tmpSelectedIndex);
                    points.Insert(tmpSelectedIndex - 1, point);

                    foreach (ItemProperty property in properties)
                        property.SetValue(points.Select(x => new CustomNamePoint(x)).ToImmutableList());

                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex - 1;
                });

            MoveDownCommand = new ActionCommand(
                _ => SelectedIndex < _points.Count - 1,
                _ =>
                {
                    int tmpSelectedIndex = SelectedIndex;
                    BeginEdit?.Invoke(this, EventArgs.Empty);
                    List<CustomNamePoint> points = [.. Points];
                    CustomNamePoint point = points[tmpSelectedIndex];
                    points.RemoveAt(tmpSelectedIndex);
                    points.Insert(tmpSelectedIndex + 1, point);

                    foreach (ItemProperty property in properties)
                        property.SetValue(points.Select(x => new CustomNamePoint(x)).ToImmutableList());

                    EndEdit?.Invoke(this, EventArgs.Empty);
                    SelectedIndex = tmpSelectedIndex + 1;
                });

            UpdatePoints();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _properties[0].PropertyInfo.Name)
                UpdatePoints();
        }

        private void UpdatePoints()
        {
            ImmutableList<CustomNamePoint> values = _properties[0].GetValue<ImmutableList<CustomNamePoint>>() ?? [];

            if (!Points.SequenceEqual(values))
                Points = [.. values];

            ActionCommand[] commands = [AddCommand, RemoveCommand, MoveUpCommand, MoveDownCommand];

            foreach (ActionCommand command in commands)
                command.RaiseCanExecuteChanged();
        }

        public void CopyToOtherItems()
        {
            IEnumerable<ItemProperty> otherProperties = _properties.Skip(1);

            foreach (var property in otherProperties)
                property.SetValue(Points.Select(x => new CustomNamePoint(x)).ToImmutableList());
        }

        public void Dispose()
        {
            _item.PropertyChanged -= Item_PropertyChanged;
        }
    }
}
