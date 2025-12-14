using System.Windows;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Control.PartValueList
{
    internal class PartValueListForShapeAttribute : PropertyEditorAttribute2
    {
        public override FrameworkElement Create()
        {
            return new PartValueListView();
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            if (control is not PartValueListView editor)
                return;

            PartValueListViewModel vm = new(itemProperties, true);
            editor.DataContext = vm;
        }

        public override void ClearBindings(FrameworkElement control)
        {
            if (control is not PartValueListView editor)
                return;

            var vm = editor.DataContext as PartValueListViewModel;
            vm?.Dispose();
            editor.DataContext = null;
        }
    }
}
