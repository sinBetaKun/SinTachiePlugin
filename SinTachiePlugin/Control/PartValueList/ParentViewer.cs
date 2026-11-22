using SinTachiePlugin.Part.LayerInformation.SourceSelectArg;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Parent;
using SinTachiePlugin.Properties;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SinTachiePlugin.Control.PartValueList
{
    internal class ParentViewer : UserControl
    {
        private readonly StackPanel _panel = new();
        private readonly TextBlock _valueText = new();
        private SourceSelectArgBase? _mainArg = null;
        private string _parent = string.Empty;

        public ParentViewer()
        {
            InitializeUI();
            DataContextChanged += MainArgChanged;
        }

        private void InitializeUI()
        {
            _panel.Margin = new(10, 0, 0, 0);
            _panel.VerticalAlignment = VerticalAlignment.Center;
            _panel.Orientation = Orientation.Horizontal;
            _valueText.VerticalAlignment = VerticalAlignment.Center;
            TextBlock label = new()
            {
                Text = TextResource.PartParam_LayerInfo_SourceSelectArg_Parent,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold
            };
            Border border = new()
            {
                Child = _valueText,
                BorderBrush = SystemColors.ActiveBorderBrush,
                BorderThickness = new(1),
                Padding = new(4, 0, 4, 0),
                Margin = new(2, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            _panel.Children.Add(label);
            _panel.Children.Add(border);
            _panel.Visibility = Visibility.Collapsed;
            Content = _panel;
        }

        private void MainArgChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _mainArg?.PropertyChanged -= SourceSelectPropChanged;

            if (DataContext is not SourceSelectArgBase param_0)
            {
                _mainArg = null;
                return;
            }

            _mainArg = param_0;
            param_0.PropertyChanged += SourceSelectPropChanged;
            UpdateParentText();
        }

        private void SourceSelectPropChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateParentText();
        }

        private void UpdateParentText()
        {
            if (_mainArg is null)
                return;

            if (_mainArg is not IParentParameter param_0)
                return;

            if (_parent != param_0.Parent)
            {
                _parent = param_0.Parent;

                if (string.IsNullOrEmpty(_parent))
                {
                    _panel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _panel.Visibility = Visibility.Visible;
                    _valueText.Text = _parent;
                }
            }
        }
    }
}
