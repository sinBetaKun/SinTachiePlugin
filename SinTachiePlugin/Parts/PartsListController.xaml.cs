﻿using SinTachiePlugin.Informations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using YukkuriMovieMaker.Commons;
using UserControl = System.Windows.Controls.UserControl;
using Path = System.IO.Path;
using YukkuriMovieMaker.Controls;
using System.Windows.Forms;
using Clipboard = System.Windows.Clipboard;
using Newtonsoft.Json;
using System.Reflection;
using System.Windows.Controls.Primitives;
using Point = System.Windows.Point;
using System.Reflection.Emit;

namespace SinTachiePlugin.Parts
{
    /// <summary>
    /// PartsListController.xaml の相互作用ロジック
    /// </summary>
    public partial class PartsListController : UserControl, IPropertyEditorControl
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
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.SelectedTreeViewItem = e.NewValue; // ViewModelに設定
            }
        }

        private void Scissors_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.ScissorsFunc();
            }
        }

        private void Copy_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.CopyFunc();
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
                viewModel.DuplicationFunc();
            }
        }

        private void Remove_Clicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartsListControllerViewModel viewModel)
            {
                viewModel.RemoveFunc();
            }
        }
    }
}
