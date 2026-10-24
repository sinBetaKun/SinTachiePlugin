//using SinTachiePlugin.Control.PartValueList.SourceSelectorControl;
using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Control.PartValueList
{
    /// <summary>
    /// PartValueView.xaml の相互作用ロジック
    /// </summary>
    public partial class PartValueView : UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public PartValueView()
        {
            InitializeComponent();
        }

        private void PropertiesEditor_BeginEdit(object? sender, EventArgs e)
        {
            BeginEdit?.Invoke(this, e);
        }

        private void PropertiesEditor_EndEdit(object? sender, EventArgs e)
        {
            EndEdit?.Invoke(this, e);
        }

        private void Switch_Hide(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartValue pv)
            {
                BeginEdit?.Invoke(this, e);
                pv.ControlledParameters.Hide = !pv.ControlledParameters.Hide;
                EndEdit?.Invoke(this, e);
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                nameof(IsSelected),
                typeof(bool),
                typeof(PartValueView),
                new PropertyMetadata(false, OnStatusChanged));

        private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PartValueView control)
            {
                if (control.IsSelected)
                    control.SelectedSign.Visibility = Visibility.Visible;
                else
                    control.SelectedSign.Visibility = Visibility.Hidden;
            }
        }

        public SourceSelectArgBase? SourceSelectArg
        {
            get => (SourceSelectArgBase)GetValue(SourceSelectGroupProperty);
            set => SetValue(SourceSelectGroupProperty, value);
        }

        public static readonly DependencyProperty SourceSelectGroupProperty =
            DependencyProperty.Register(
                nameof(SourceSelectArg),
                typeof(SourceSelectArgBase),
                typeof(PartValueView),
                new PropertyMetadata(null, OnThisLayerTypeChanged));

        private static void OnThisLayerTypeChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is PartValueView control)
            {
                control.UpdateGroupPullDownButton();
            }
        }

        public event EventHandler? GroupPullDownChanged;

        private void UpdateGroupPullDownButton(object sender, RoutedEventArgs e)
        {
            if (SourceSelectArg is not GroupParameter gp)
                return;

            gp.IsOpened = !gp.IsOpened;
            UpdateGroupPullDownButton();
        }

        private void UpdateGroupPullDownButton()
        {
            if (SourceSelectArg is not GroupParameter gp)
                return;

            StreamGeometry geometry = new();

            using (var context = geometry.Open())
            {
                if (gp.IsOpened)
                {
                    context.BeginFigure(
                        new Point(8, 8),
                        isFilled: true,
                        isClosed: true);

                    context.LineTo(new Point(16, 0), true, false);
                    context.LineTo(new Point(0, 0), true, false);
                }
                else
                {
                    context.BeginFigure(
                        new Point(8, 0),
                        isFilled: true,
                        isClosed: true);

                    context.LineTo(new Point(16, 8), true, false);
                    context.LineTo(new Point(0, 8), true, false);
                }
            }

            geometry.Freeze();
            GroupPullDownButtonPath.Data = geometry;
            GroupPullDownChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
