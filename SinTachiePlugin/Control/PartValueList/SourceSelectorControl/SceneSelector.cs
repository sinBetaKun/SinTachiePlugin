using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.SceneId;
using System.Windows.Data;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using Binding = System.Windows.Data.Binding;

namespace SinTachiePlugin.Control.PartValueList.SourceSelectorControl
{
    public class SceneSelector : System.Windows.Controls.UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public SceneSelector()
        {
            SceneComboBox selector = new();
            selector.SetBinding(
                SceneComboBox.ValueProperty,
                new Binding(nameof(ISceneIdParameter.SceneId))
                {
                    Mode = BindingMode.TwoWay,
                });
            selector.BeginEdit += (sender, e) => BeginEdit?.Invoke(sender, e);
            selector.EndEdit += (sender, e) => EndEdit?.Invoke(sender, e);
            Content = selector;
        }
    }
}
