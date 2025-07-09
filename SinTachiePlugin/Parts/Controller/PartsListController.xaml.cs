using SinTachiePlugin.Informations;
using SinTachiePlugin.ShapePludin.PartsListControllerForShape;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using YukkuriMovieMaker.Commons;
using UserControl = System.Windows.Controls.UserControl;

namespace SinTachiePlugin.Parts
{
    /// <summary>
    /// PartsListController.xaml の相互作用ロジック
    /// </summary>
    public partial class PartsListController : UserControl, IPropertyEditorControl2
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public PartsListController()
        {
            InitializeComponent();
            DataContextChanged += PartsEditor_DataContextChanged;
        }

        private void PartsEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is PartsListControllerViewModel oldVm)
            {
                oldVm.BeginEdit -= PropertiesEditor_BeginEdit;
                oldVm.EndEdit -= PropertiesEditor_EndEdit;
            }
            if (e.NewValue is PartsListControllerViewModel newVm)
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
            var vm = DataContext as PartsListControllerViewModel;
            vm?.CopyToOtherItems();
            EndEdit?.Invoke(this, e);
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
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
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.AddPart(PartNameTree2);
            }
        }

        private void Cut_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.CutFunc(GetSelecteds());
            }
        }

        private void Copy_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.CopyFunc(GetSelecteds());
            }
        }

        private void Paste_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.PasteFunc();
            }
        }

        private void Duplication_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.DuplicationFunc(GetSelecteds());
            }
        }

        private void CheckAll_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
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
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.CheckOnlySelected(GetSelecteds());
            }
        }

        private void Remove_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.RemoveFunc(GetSelecteds());
            }
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.MoveUpSelected(GetSelecteds());
            }
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.MoveDownSelected(GetSelecteds());
            }
        }

        private void ReloadDefault_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.ReloadFunc(GetSelecteds());
            }
        }

        private void HeightSlider_EndEdit(object sender, EventArgs e)
        {
            var vm = DataContext as PartsListControllerViewModel;
            vm?.SetProperties();
            EndEdit?.Invoke(this, e);
        }

        public void SetEditorInfo(IEditorInfo info)
        {
            propertiesEditor.SetEditorInfo(info);
        }

        public List<PartBlock> GetSelecteds()
        {
            List<PartBlock> selecteds = [];
            foreach (var selected in list.SelectedItems)
                if (selected is PartBlock block)
                    selecteds.Add(block);
            return selecteds;
        }

        public void SetSelecteds(List<PartBlock> selecteds)
        {
            list.SelectedItems.Clear();
            foreach (var selected in list.SelectedItems)
                list.SelectedItems.Add(selected);
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.UpdateButtonEnables(GetSelecteds());
            }
        }
    }
}
