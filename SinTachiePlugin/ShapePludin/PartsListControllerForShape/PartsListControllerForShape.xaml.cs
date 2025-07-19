using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Parts;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.ShapePludin.PartsListControllerForShape
{
    /// <summary>
    /// PartsListControllerForShape.xaml の相互作用ロジック
    /// </summary>
    public partial class PartsListControllerForShape : System.Windows.Controls.UserControl, IPropertyEditorControl2
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public PartsListControllerForShape()
        {
            InitializeComponent();
            DataContextChanged += PartsEditor_DataContextChanged;
        }

        private void PartsEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PartsListControllerForShapeViewModel oldVm)
            {
                oldVm.BeginEdit -= PropertiesEditor_BeginEdit;
                oldVm.EndEdit -= PropertiesEditor_EndEdit;
            }
            if (e.NewValue is PartsListControllerForShapeViewModel newVm)
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
            var vm = DataContext as PartsListControllerForShapeViewModel;
            vm?.CopyToOtherItems();
            EndEdit?.Invoke(this, e);
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                if (e.Delta > 0)
                    scrollViewer.LineUp();
                else
                    scrollViewer.LineDown();

                e.Handled = true;
            }
        }

        private void Add_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.AddPart(PartNameTree2);
            }
        }

        private void Cut_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.CutFunc(GetSelecteds());
            }
        }

        private void Copy_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.CopyFunc(GetSelecteds());
            }
        }

        private void Paste_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.PasteFunc();
            }
        }

        private void Duplication_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.DuplicationFunc(GetSelecteds());
            }
        }

        private void CheckAll_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                if (viewModel.Parts.Count < 1)
                {
                    SinTachieDialog.ShowInformation("ブロックが１つもありません。");
                    return;
                }
                if (viewModel.Parts.Where(part => !part.Appear).Any())
                {
                    viewModel.CheckAll();
                    return;
                }
                SinTachieDialog.ShowInformation("非表示のブロックが１つもありません。");
            }
        }

        private void CheckOnlyOne_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.CheckOnlySelected(GetSelecteds());
            }
        }

        private void Remove_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.RemoveFunc(GetSelecteds());
            }
        }

        private void DirectorySelector_EndEdit(object sender, EventArgs e)
        {
            var vm = DataContext as PartsListControllerForShapeViewModel;
            vm?.SetProperties();
            EndEdit?.Invoke(this, e);
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.MoveUpSelected(GetSelecteds());
            }
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.MoveDownSelected(GetSelecteds());
            }
        }

        private void ReloadDefault_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.ReloadFunc(GetSelecteds());
            }
        }

        private void HeightSlider_EndEdit(object sender, EventArgs e)
        {
            var vm = DataContext as PartsListControllerForShapeViewModel;
            vm?.SetProperties();
            EndEdit?.Invoke(this, e);
        }

        public void SetEditorInfo(IEditorInfo info)
        {
            propertiesEditor.SetEditorInfo(info);
        }

        private List<PartBlock> GetSelecteds()
        {
            List<(PartBlock, int) > selecteds = [];
            foreach (var selected in list.SelectedItems)
                if (selected is PartBlock block)
                    selecteds.Add((block, list.Items.IndexOf(block)));
            return selecteds.OrderBy(x => x.Item2).Select(x => x.Item1).ToList();
        }

        public void SetSelecteds(List<PartBlock> selecteds)
        {
            list.SelectedItems.Clear();
            foreach (var selected in list.SelectedItems)
                list.SelectedItems.Add(selected);
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.UpdateButtonEnables(GetSelecteds());
            }
        }

        private void List_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = FindVisualChild<ScrollViewer>(list);
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

        private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t)
                    return t;
                else
                {
                    T? result = FindVisualChild<T>(child);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }
    }
}
