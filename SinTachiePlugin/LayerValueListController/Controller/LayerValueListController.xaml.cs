using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SinTachiePlugin.LayerValueListController.Controller;
using SinTachiePlugin.Parts;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace SinTachiePlugin.LayerValueListController
{
    /// <summary>
    /// LayerValueListController.xaml の相互作用ロジック
    /// </summary>
    public partial class LayerValueListController : UserControl, IPropertyEditorControl2
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public LayerValueListController()
        {
            InitializeComponent();
            DataContextChanged += PartsEditor_DataContextChanged;
        }

        private void PartsEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is LayerValueListControllerViewModel oldVm)
            {
                oldVm.BeginEdit -= PropertiesEditor_BeginEdit;
                oldVm.EndEdit -= PropertiesEditor_EndEdit;
            }   
            if (e.NewValue is LayerValueListControllerViewModel newVm)
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
            //Part内のAnimationを変更した際にPartsを更新する
            //複数のアイテムを選択している場合にすべてのアイテムを更新するために必要
            var vm = DataContext as LayerValueListControllerViewModel;
            vm?.CopyToOtherItems();
            EndEdit?.Invoke(this, e);
        }

        public void SetEditorInfo(IEditorInfo info)
        {
            propertiesEditor.SetEditorInfo(info);
        }

        private void List_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var scrollViewer = PartsListControllerViewModelBase.FindVisualChild<ScrollViewer>(list);
            if (scrollViewer == null) return;

            e.Handled = true;
            bool scrollingUp = e.Delta > 0;

            if ((scrollingUp && scrollViewer.VerticalOffset == 0) ||
                (!scrollingUp && scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight))
            {
                // 端に到達 → スクロールイベントを親に渡す

                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.MouseWheelEvent,
                    Source = sender
                };

                // 親要素を取得してイベント再発火
                var parent = ((System.Windows.Controls.Control)sender).Parent as UIElement;
                parent?.RaiseEvent(eventArg);
            }
            else
            {
                // まだスクロール可能 → 自分で処理
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            }
        }
    }
}
