using SinTachiePlugin.Part.LayerInformation.SourceSelectArg;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.ImageFilePath;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.PsdFileInfo;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.SceneId;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.VideoFilePath;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using Binding = System.Windows.Data.Binding;

namespace SinTachiePlugin.Control.PartValueList
{
    public class SourceSelectorForPartValue : UserControl, IPropertyEditorControl
    {
        enum SelectorMode
        {
            File,
            Scene,
            None,
        }

        public event EventHandler? BeginEdit;
        public event EventHandler? EndEdit;

        private SelectorMode _mode = SelectorMode.None;
        public string FilePath { get; set; } = string.Empty;
        private readonly FileSelector _fSelector = new();
        private readonly SceneComboBox _sSelector = new();
        private readonly Grid _grid = new();

        public SourceSelectorForPartValue()
        {
            Content = _grid;
            DataContextChanged += UpdateSelector;
        }

        public void UpdateSelector(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is not SourceSelectArgBase ssa)
            {
                SetGrid();
                return;
            }

            if (ssa is ImageFileParameter ifp)
            {
                SetImageFileSelector(ifp.ImageFilePath);
            }
            else if (ssa is PsdFileParameter pfp)
            {
                SetPsdFileSelector(pfp.PsdFileInfo.FilePath);
            }
            else if (ssa is VideoFileParameter vfp)
            {
                SetVideoFileSelector(vfp.VideoFilePath);
            }
            else if (ssa is SceneParameter sp)
            {
                SetSceneSelector(sp.SceneId);
            }
            else
            {
                SetGrid();
            }
        }

        public void InvokeBeginEdit(object? sender, EventArgs e) => BeginEdit?.Invoke(sender, e);
        public void InvokeEndEdit(object? sender, EventArgs e) => EndEdit?.Invoke(sender, e);

        private void ResetSelector()
        {
            switch (_mode)
            {
                case SelectorMode.File:
                    BindingOperations.ClearBinding(
                        _fSelector,
                        FileSelector.ValueProperty);
                    _fSelector.BeginEdit -= InvokeBeginEdit;
                    _fSelector.EndEdit -= InvokeEndEdit;
                    break;

                case SelectorMode.Scene:
                    BindingOperations.ClearBinding(
                        _sSelector,
                        SceneComboBox.ValueProperty);
                    _sSelector.BeginEdit -= InvokeBeginEdit;
                    _sSelector.EndEdit -= InvokeEndEdit;
                    break;
            }

            Content = _grid;
            _mode = SelectorMode.None;
        }

        private void SetImageFileSelector(string path)
        {
            ResetSelector();

            _fSelector.ShowThumbnail = true;
            _fSelector.FileType = YukkuriMovieMaker.Settings.FileType.画像;
            _fSelector.FileGroup = YukkuriMovieMaker.Settings.FileGroupType.TachieParts;
            _fSelector.ListupFilter = (x) => (!(from c in Path.GetFileName(x)
                                                where c == '.'
                                                select c).Skip(1).Any())
                                                      && (Path.GetDirectoryName(x) == Path.GetDirectoryName(_fSelector.Value));
            _fSelector.SetBinding(
                FileSelector.ValueProperty,
                new Binding(nameof(IImageFilePathParameter.ImageFilePath))
                {
                    Mode = BindingMode.TwoWay,
                });
            _fSelector.Value = path;
            _fSelector.BeginEdit += InvokeBeginEdit;
            _fSelector.EndEdit += InvokeEndEdit;
            Content = _fSelector;
            _mode = SelectorMode.File;
        }

        private void SetPsdFileSelector(string? path)
        {
            ResetSelector();

            _fSelector.ShowThumbnail = false;
            _fSelector.FileType = YukkuriMovieMaker.Settings.FileType.画像;
            _fSelector.FileGroup = YukkuriMovieMaker.Settings.FileGroupType.TachieParts;
            _fSelector.ListupFilter = (x) => Path.GetExtension(x) == ".psd"
                                           && (Path.GetDirectoryName(x) == Path.GetDirectoryName(_fSelector.Value));
            _fSelector.SetBinding(
                FileSelector.ValueProperty,
                new Binding(
                    $"{nameof(IPsdFileInfoParameter.PsdFileInfo)}.{nameof(IPsdFileInfoParameter.PsdFileInfo.FilePath)}")
                {
                    Mode = BindingMode.TwoWay,
                });
            _fSelector.Value = path;
            _fSelector.BeginEdit += InvokeBeginEdit;
            _fSelector.EndEdit += InvokeEndEdit;
            Content = _fSelector;
            _mode = SelectorMode.File;
        }

        private void SetVideoFileSelector(string path)
        {
            ResetSelector();
            _fSelector.ShowThumbnail = true;
            _fSelector.FileType = YukkuriMovieMaker.Settings.FileType.動画;
            _fSelector.FileGroup = YukkuriMovieMaker.Settings.FileGroupType.TachieParts;
            _fSelector.ListupFilter = (x) => Path.GetDirectoryName(x) == Path.GetDirectoryName(_fSelector.Value);
            _fSelector.SetBinding(
                FileSelector.ValueProperty,
                new Binding(nameof(IVideoFilePathParameter.VideoFilePath))
                {
                    Mode = BindingMode.TwoWay,
                });
            _fSelector.Value = path;
            _fSelector.BeginEdit += InvokeBeginEdit;
            _fSelector.EndEdit += InvokeEndEdit;
            Content = _fSelector;
            _mode = SelectorMode.File;
        }

        private void SetSceneSelector(Guid scene)
        {
            ResetSelector();
            _sSelector.SetBinding(
                SceneComboBox.ValueProperty,
                new Binding(nameof(ISceneIdParameter.SceneId))
                {
                    Mode = BindingMode.TwoWay,
                });
            _sSelector.Value = scene;
            _sSelector.BeginEdit += InvokeBeginEdit;
            _sSelector.EndEdit += InvokeEndEdit;
            Content = _sSelector;
            _mode = SelectorMode.Scene;
        }

        private void SetGrid()
        {
            ResetSelector();
        }
    }
}
