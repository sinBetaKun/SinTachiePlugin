using SinTachiePlugin.Parts;
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

namespace SinTachiePlugin.LayerValueListController.Controller
{
    /// <summary>
    /// LayerValueUI.xaml の相互作用ロジック
    /// </summary>
    public partial class LayerValueUI : System.Windows.Controls.UserControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public LayerValueUI()
        {
            InitializeComponent();
        }

        private void PropertiesEditor_BeginEdit(object? sender, EventArgs e)
        {
            BeginEdit?.Invoke(this, e);
        }

        private void PropertiesEditor_EndEdit(object? sender, EventArgs e)
        {
            //Part内のAnimationを変更した際にPartsを更新する
            //複数のアイテムを選択している場合にすべてのアイテムを更新するために必要
            var vm = DataContext as LayerValue;
            EndEdit?.Invoke(this, e);
        }
    }
}
