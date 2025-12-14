using System.Windows;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Control.PartAnimationValueList
{
    internal class PartAnimationValueListAttribute : PropertyEditorAttribute2
    {
        public override FrameworkElement Create()
        {
            return new PartAnimationValueListView();
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            if (control is not PartAnimationValueListView editor)
                return;

            editor.DataContext = new PartAnimationValueListViewModel(itemProperties);
        }

        public override void ClearBindings(FrameworkElement control)
        {
            if (control is not PartAnimationValueListView editor)
                return;

            if (editor.DataContext is PartAnimationValueListViewModel vm)
                vm.Dispose();

            editor.DataContext = null;
        }
    }
}
