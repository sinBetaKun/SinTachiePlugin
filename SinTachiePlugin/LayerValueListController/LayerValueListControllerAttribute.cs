using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.LayerValueListController
{
    public class LayerValueListControllerAttribute : PropertyEditorAttribute2
    {
        public override FrameworkElement Create()
        {
            return new LayerValueListController();
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            if (control is not LayerValueListController editor)
                return;
            editor.DataContext = new LayerValueListControllerViewModel(itemProperties);
        }

        public override void ClearBindings(FrameworkElement control)
        {
            if (control is not LayerValueListController editor)
                return;
            var vm = editor.DataContext as LayerValueListControllerViewModel;
            vm?.Dispose();
            editor.DataContext = null;
        }
    }
}
