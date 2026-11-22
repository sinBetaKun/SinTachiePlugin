using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.Drawing.DrawingArg.Parameter;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument.Argument.CustomMode;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument.Argument.Values;
using SinTachiePlugin.Properties;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SinTachiePlugin.Control.PartValueList
{
    internal class PriorityViewer : UserControl
    {
        private readonly StackPanel _panel = new();
        private readonly TextBlock _valueText = new();
        private readonly TextBlock _modeText = new();
        private HavingSourceDrawingParameter? _mainArg;
        private DrawingSubArgBase? _subArg;
        private IValuesParameter? _valueParam;
        private DrawingValueModeMaster _modeMaster = DrawingValueModeMaster.DontOverride;
        private DrawingValueMode? _priorityMode = null;
        private double _priority = 0.0;

        public PriorityViewer()
        {
            InitializeUI();
            DataContextChanged += ArgsChanged;
        }

        private void InitializeUI()
        {
            _panel.Margin = new(10, 0, 0, 0);
            _panel.VerticalAlignment = VerticalAlignment.Center;
            _panel.Orientation = Orientation.Horizontal;
            _valueText.VerticalAlignment = VerticalAlignment.Center;
            TextBlock label = new()
            {
                Text = TextResource.PartParam_Drawing_Value_Priority,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold
            };
            _modeText.Text = TextResource.PartParam_Drawing_Mode_Priority_Add;
            _modeText.VerticalAlignment = VerticalAlignment.Center;
            _modeText.FontWeight = FontWeights.Bold;
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
            _panel.Children.Add(_modeText);
            _panel.Children.Add(border);
            _panel.Visibility = Visibility.Collapsed;
            Content = _panel;
        }

        private void ArgsChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _mainArg?.PropertyChanged -= MainArgPropChanged;

            if (DataContext is not HavingSourceDrawingParameter param_0)
            {
                _subArg?.PropertyChanged -= SubArgPropChanged;
                _modeMaster = DrawingValueModeMaster.DontOverride;
                _modeText.Visibility = Visibility.Collapsed;
                _valueParam?.Priority.Values.First().PropertyChanged -= PriorityChanged;
                _mainArg = null;
                _subArg = null;
                _valueParam = null;
                _panel.Visibility = Visibility.Collapsed;
                return;
            }

            param_0.PropertyChanged += MainArgPropChanged;
            _mainArg = param_0;
            UpdateForMainArgs();
        }

        private void MainArgPropChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateForMainArgs();
        }

        private void UpdateForMainArgs()
        {
            if (_mainArg is null)
                return;

            if (_modeMaster != _mainArg.ModeMaster)
            {
                _modeMaster = _mainArg.ModeMaster;

                switch (_modeMaster)
                {
                    case DrawingValueModeMaster.Override:
                        _modeText.Visibility = Visibility.Collapsed;
                        break;

                    case DrawingValueModeMaster.Compose:
                        _modeText.Visibility = Visibility.Visible;
                        break;
                }
            }

            if (_subArg != _mainArg.SubArg)
            {
                _subArg?.PropertyChanged -= SubArgPropChanged;
                _subArg = _mainArg.SubArg;

                if (_subArg is not IValuesParameter param_1)
                {
                    _priorityMode = null;
                    _modeText.Visibility = Visibility.Collapsed;
                    _valueParam?.Priority.Values.First().PropertyChanged -= PriorityChanged;
                    _valueParam = null;
                    _panel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _subArg.PropertyChanged += SubArgPropChanged;
                    UpdateForSubArg();
                    _valueParam?.Priority.Values.First().PropertyChanged -= PriorityChanged;
                    param_1.Priority.Values.First().PropertyChanged += PriorityChanged;
                    _valueParam = param_1;
                    UpdatePriorityTextBlock();
                }
            }
        }

        private void SubArgPropChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateForSubArg();
        }

        private void UpdateForSubArg()
        {
            if (_subArg is not ICustomModeParameter param_0)
                return;

            if (_priorityMode != param_0.PriorityMode)
            {
                _priorityMode = param_0.PriorityMode;

                switch (_priorityMode)
                {
                    case DrawingValueMode.Override:
                        _modeText.Visibility = Visibility.Collapsed;
                        break;

                    case DrawingValueMode.Compose:
                        _modeText.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void PriorityChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdatePriorityTextBlock();
        }

        private void UpdatePriorityTextBlock()
        {
            if (_valueParam is null)
                return;

            double priority = _valueParam.Priority.Values.First().Value;

            if (_priority != priority)
            {
                _priority = priority;

                if (priority == 0.0)
                {
                    _panel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    _valueText.Text = string.Format("{0:f1}", priority);
                    _panel.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
