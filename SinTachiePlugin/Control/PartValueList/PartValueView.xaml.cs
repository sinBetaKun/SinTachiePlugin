using SinTachiePlugin.Control.PartValueList.SourceSelectorControl;
using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using System.Windows;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Control.PartValueList
{
    /// <summary>
    /// PartValueView.xaml の相互作用ロジック
    /// </summary>
    public partial class PartValueView : System.Windows.Controls.UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public PartValueView()
        {
            InitializeComponent();
            DataContextChanged += PartValueView_DataContextChanged;
        }

        private void PartValueView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PartValue oldVm)
            {
                oldVm.ControlledParameters.LayerTypeChanged -= ChangeSourceSelector;
            }
            if (e.NewValue is PartValue newVm)
            {
                newVm.ControlledParameters.LayerTypeChanged += ChangeSourceSelector;
                ChangeSourceSelector(null, EventArgs.Empty);
            }
        }

        private IPropertyEditorControl? sourceSelector = null;

        private void ChangeSourceSelector(object? sender, EventArgs e)
        {
            if (sourceSelector != null)
            {
                sourceSelector.BeginEdit -= PropertiesEditor_BeginEdit;
                sourceSelector.EndEdit -= PropertiesEditor_EndEdit;
            }

            sourceSelector = null;
            SourceSelectorGrid.Children.Clear();

            if (DataContext is not PartValue pv)
                return;

            if (pv.ControlledParameters.SourceSelectArg is ImageFileParameter ip)
            {
                ImageFileSelector ifs = new()
                {
                    DataContext = ip
                };
                sourceSelector = ifs;
                SourceSelectorGrid.Children.Add(ifs);
            }
            else if (pv.ControlledParameters.SourceSelectArg is PsdFileParameter pp)
            {
                PsdFileSelector pfs = new()
                {
                    DataContext = pp
                };
                sourceSelector = pfs;
                SourceSelectorGrid.Children.Add(pfs);
            }
            else if (pv.ControlledParameters.SourceSelectArg is VideoFileParameter vp)
            {
                VideoFileSelector vfs = new()
                {
                    DataContext = vp
                };
                sourceSelector = vfs;
                SourceSelectorGrid.Children.Add(vfs);
            }
            else if (pv.ControlledParameters.SourceSelectArg is SceneParameter sp)
            {
                SceneSelector ss = new()
                {
                    DataContext = sp
                };
                sourceSelector = ss;
                SourceSelectorGrid.Children.Add(ss);
            }

            if (sourceSelector != null)
            {
                sourceSelector.BeginEdit += PropertiesEditor_BeginEdit;
                sourceSelector.EndEdit += PropertiesEditor_EndEdit;
            }
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
                control.UpdateUIStyle();
            }
        }

        private void UpdateUIStyle()
        {
            if (IsSelected)
                SelectedSign.Visibility = Visibility.Visible;
            else
                SelectedSign.Visibility = Visibility.Hidden;
        }

        public event EventHandler? GroupOpened;
        public event EventHandler? GroupClosed;

        private void GroupViewChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is not PartValue pv)
                return;

            if (pv.ControlledParameters.SourceSelectArg is not GroupParameter gp)
                return;

            gp.IsOpened = !gp.IsOpened;

            if (gp.IsOpened)
            {
                GroupOpened?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                GroupClosed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
