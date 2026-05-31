using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.PsdFilePath;
using System.IO;
using System.Windows.Data;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using Binding = System.Windows.Data.Binding;

namespace SinTachiePlugin.Control.PartValueList.SourceSelectorControl
{
    public class VideoFileSelector : System.Windows.Controls.UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public VideoFileSelector()
        {
            FileSelector selector = new()
            {
                ShowThumbnail = true,
                FileType = YukkuriMovieMaker.Settings.FileType.動画,
                FileGroup = YukkuriMovieMaker.Settings.FileGroupType.TachieParts
            };
            selector.ListupFilter = (x) => Path.GetDirectoryName(x) == Path.GetDirectoryName(selector.Value);
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
