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
using SinTachiePlugin.LayerValueListController;
using SinTachiePlugin.Parts;
using YukkuriMovieMaker.Commons;
using UserControl = System.Windows.Controls.UserControl;

namespace SinTachiePlugin.LayerValueListController
{
    /// <summary>
    /// LayerValueListController.xaml の相互作用ロジック
    /// </summary>
    public partial class LayerValueListController : UserControl, IPropertyEditorControl
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
            var vm = DataContext as LayerValueListControllerViewModel;
            vm?.CopyToOtherItems();
            EndEdit?.Invoke(this, e);
        }
    }
}
