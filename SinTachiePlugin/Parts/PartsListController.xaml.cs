using SinTachiePlugin.Informations;
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

        //private void ItemListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var vm = DataContext as PartsListControllerViewModel;
        //    if (vm.SelectedAddingPart != null)
        //    {
        //        if (String.IsNullOrWhiteSpace(vm.SelectedAddingPart))
        //        {
        //            SinTachieDialog.ShowWarning("「追加するパーツ」が選択されていないため、パーツを追加できません。");
        //            return;
        //        }

        //        var tmpSelectedIndex = vm.SelectedIndex;
        //        BeginEdit?.Invoke(this, EventArgs.Empty);
        //        string partImagePath = vm.FindFirstImageOfPart(vm.SelectedAddingPart);
        //        string tag = vm.SelectedAddingPart;
        //        var tags = from part in vm.Parts select part.TagName;
        //        if (tags.Contains(tag))
        //        {
        //            int sideNum = 1;
        //            while (tags.Contains($"{tag}({sideNum})")) sideNum++;
        //            tag += $"({sideNum})";
        //        }
        //        vm.Parts = vm.Parts.Insert(tmpSelectedIndex + 1, new PartBlock(partImagePath, tag));
        //        foreach (var property in vm.properties)
        //            property.SetValue(vm.Parts);

        //        vm.AddPopupOpen = false;
        //        EndEdit?.Invoke(this, EventArgs.Empty);
        //        vm.SelectedIndex = tmpSelectedIndex + 1;
        //    }
        //}
    }
}
