using System.Windows;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Control.CustomNamePointList
{
    /// <summary>
    /// CustomNamePointList.xaml の相互作用ロジック
    /// </summary>
    public partial class CustomNamePointList : System.Windows.Controls.UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public CustomNamePointList()
        {
            InitializeComponent();
            DataContextChanged += PointsEditor_DataContextChanged;
        }

        private void PointsEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is CustomNamePointListViewModel oldVm)
            {
                oldVm.BeginEdit -= PropertiesEditor_BeginEdit;
                oldVm.EndEdit -= PropertiesEditor_EndEdit;
            }

            if (e.NewValue is CustomNamePointListViewModel newVm)
            {
                newVm.BeginEdit += PropertiesEditor_BeginEdit;
                newVm.EndEdit += PropertiesEditor_EndEdit;
            }
        }

        private void PropertiesEditor_BeginEdit(object? sender, EventArgs e)
        {
            BeginEdit?.Invoke(this, e);
        }

        private void PropertiesEditor_EndEdit(object? sender, EventArgs e)
        {
            if (DataContext is CustomNamePointListViewModel vm)
                vm.CopyToOtherItems();

            EndEdit?.Invoke(this, e);
        }
    }
}
