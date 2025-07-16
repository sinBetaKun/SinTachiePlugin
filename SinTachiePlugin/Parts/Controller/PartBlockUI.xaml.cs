using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;

namespace SinTachiePlugin.Parts
{
    /// <summary>
    /// PartBlockUI.xaml の相互作用ロジック
    /// </summary>
    public partial class PartBlockUI : System.Windows.Controls.UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public PartBlockUI()
        {
            InitializeComponent();
            selector.ListupFilter = (x) => (!(from c in Path.GetFileName(x)
                                              where c == '.'
                                              select c).Skip(1).Any())
                                              && (Path.GetDirectoryName(x) == Path.GetDirectoryName(selector.Value));
        }

        private void PropertiesEditor_BeginEdit(object? sender, EventArgs e)
        {
            BeginEdit?.Invoke(this, e);
        }

        private void PropertiesEditor_EndEdit(object? sender, EventArgs e)
        {
            //Part内のAnimationを変更した際にPartsを更新する
            //複数のアイテムを選択している場合にすべてのアイテムを更新するために必要
            var vm = DataContext as PartBlock;
            EndEdit?.Invoke(this, e);
        }

        private void Switch_Appear(object sender, RoutedEventArgs e)
        {
            if (DataContext is PartBlock vm)
            {
                BeginEdit?.Invoke(this, e);
                vm.Appear = !vm.Appear;
                EndEdit?.Invoke(this, e);
            }
        }

        private void Update_CheckBox(object sender, RoutedEventArgs e)
        {
            if(DataContext is PartBlock vm && sender is System.Windows.Controls.CheckBox checkBox)
            {
                if(checkBox.IsChecked is bool isChecked)
                {
                    if(vm.Appear != isChecked)
                    {
                        BeginEdit?.Invoke(this, e);
                        vm.Appear = isChecked;
                        EndEdit?.Invoke(this, e);
                    }
                }
            }
        }
    }
}
