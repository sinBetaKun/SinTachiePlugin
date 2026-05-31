using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.ImageFilePath;
using System.IO;
using System.Windows.Data;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using Binding = System.Windows.Data.Binding;

namespace SinTachiePlugin.Control.PartValueList.SourceSelectorControl
{
    public class ImageFileSelector : System.Windows.Controls.UserControl, IPropertyEditorControl
    {
        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        public ImageFileSelector()
        {
            FileSelector selector = new()
            {
                ShowThumbnail = true,
                FileType = YukkuriMovieMaker.Settings.FileType.画像,
                FileGroup = YukkuriMovieMaker.Settings.FileGroupType.TachieParts
            };
            selector.ListupFilter = (x) => (!(from c in Path.GetFileName(x)
                                              where c == '.'
                                              select c).Skip(1).Any())
                                              && (Path.GetDirectoryName(x) == Path.GetDirectoryName(selector.Value));
            selector.SetBinding(
                FileSelector.ValueProperty,
                new Binding(nameof(IImageFilePathParameter.ImageFilePath))
                {
                    Mode = BindingMode.TwoWay,
                });
            selector.BeginEdit += (sender, e) => BeginEdit?.Invoke(sender, e);
            selector.EndEdit += (sender, e) => EndEdit?.Invoke(sender, e);
            Content = selector;
        }
    }
}
