using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.PsdFilePath;
using System.IO;
using System.Windows.Data;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using Binding = System.Windows.Data.Binding;

namespace SinTachiePlugin.Control.PartValueList.SourceSelectorControl
{
    public class PsdFileSelector : System.Windows.Controls.UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public PsdFileSelector()
        {
            FileSelector selector = new()
            {
                ShowThumbnail = true,
                FileType = YukkuriMovieMaker.Settings.FileType.画像,
                FileGroup = YukkuriMovieMaker.Settings.FileGroupType.TachieParts
            };
            selector.ListupFilter = (x) => Path.GetExtension(x) == ".psd"
                                           && (Path.GetDirectoryName(x) == Path.GetDirectoryName(selector.Value));
            selector.SetBinding(
                FileSelector.ValueProperty,
                new Binding(nameof(IPsdFilePathParameter.PsdFilePath))
                {
                    Mode = BindingMode.TwoWay,
                });
            selector.BeginEdit += (sender, e) => BeginEdit?.Invoke(sender, e);
            selector.EndEdit += (sender, e) => EndEdit?.Invoke(sender, e);
            Content = selector;
        }
    }
}
