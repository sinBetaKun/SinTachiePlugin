using System.Windows;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Control.CustomNamePointList
{
    internal class CustomNamePointListAttribute : PropertyEditorAttribute2
    {
        public override FrameworkElement Create()
        {
            return new CustomNamePointList();
        }

        public override void SetBindings(FrameworkElement control, ItemProperty[] itemProperties)
        {
            if (control is not CustomNamePointList editor)
                return;

            editor.DataContext = new CustomNamePointListViewModel(itemProperties);
        }

        public override void ClearBindings(FrameworkElement control)
        {
            if (control is not CustomNamePointList editor)
                return;

            if (editor.DataContext is CustomNamePointListViewModel vm)
                vm.Dispose();

            editor.DataContext = null;
        }
    }
}
