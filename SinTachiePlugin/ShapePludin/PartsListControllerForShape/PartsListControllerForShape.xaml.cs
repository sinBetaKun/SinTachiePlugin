﻿using SinTachiePlugin.Parts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.SelectedTreeViewItem = e.NewValue; // ViewModelに設定
            }
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
                viewModel.CheckAll();
            }
        }

        private void CheckOnlyOne_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerForShapeViewModel viewModel)
            {
                viewModel.CheckOnlyOne();
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

        private List<PartBlock> GetSelecteds()
        {
            List<(PartBlock, int) > selecteds = [];
            foreach (var selected in list.SelectedItems)
                if (selected is PartBlock block)
                    selecteds.Add((block, list.Items.IndexOf(block)));
            return selecteds.OrderBy(x => x.Item2).Select(x => x.Item1).ToList();
        }
    }
}
