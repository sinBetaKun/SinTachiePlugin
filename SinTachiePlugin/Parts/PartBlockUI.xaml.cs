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
using System.IO;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Parts
{
    /// <summary>
    /// PartBlockUI.xaml の相互作用ロジック
    /// </summary>
    public partial class PartBlockUI : System.Windows.Controls.UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;
        public event EventHandler? RightMouseButtonUp;

        public PartBlockUI()
        {
            InitializeComponent();
            selector.ListupFilter = (x) => !(from c in Path.GetFileName(x)
                                             where c == '.'
                                             select c).Skip(1).Any();
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

        private void Update_CheckBox(object sender, RoutedEventArgs e)
        {
            BeginEdit?.Invoke(this, e);
            EndEdit?.Invoke(this, e);
        }

        private void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            RightMouseButtonUp?.Invoke(this, e);
        }
    }
}
