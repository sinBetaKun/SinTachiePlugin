using SinTachiePlugin.Parts;
using SinTachiePlugin.ShapePludin.PartsListControllerForShape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YukkuriMovieMaker.Commons;

namespace ShapeOfSinTachie.PartsListControllerForShape
{
    public class PartsListControllerForShapeAttribute : PropertyEditorAttribute2
    {
        public override FrameworkElement Create()
        {
            return new PartsListControllerForShape();
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            if (control is not PartsListControllerForShape editor)
                return;
            var vm = new PartsListControllerForShapeViewModel(itemProperties);
            editor.DataContext = vm;
        }

        public override void ClearBindings(FrameworkElement control)
        {
            if (control is not PartsListControllerForShape editor)
                return;
            var vm = editor.DataContext as PartsListControllerForShapeViewModel;
            vm?.Dispose();
            editor.DataContext = null;
        }
    }
}
