using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.Drawing.DrawingArg;
using SinTachiePlugin.Part.Drawing.DrawingArg.Parameter;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter;
using SinTachiePlugin.Properties;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;

namespace SinTachiePlugin.Part
{
    internal class ControledParametersOfPart : Animatable
    {
        [Display(GroupName = nameof(Texts.GroupName_LayerInfo), Name = nameof(Texts.PartParam_LayerInfo_Tag), ResourceType = typeof(Texts))]
        [TextEditor]
        public string Tag { get => tag; set => Set(ref tag, value); }
        private string tag = string.Empty;

        [Display(GroupName = nameof(Texts.GroupName_LayerInfo), Name = nameof(Texts.PartParam_LayerInfo_LayerType), ResourceType = typeof(Texts))]
        [EnumComboBox]
        public LayerType LayerType { get => layerType; set => Set(ref layerType, value); }
        private LayerType layerType = LayerType.Image;

        [Display(GroupName = nameof(Texts.GroupName_LayerInfo), Name = nameof(Texts.PartParam_LayerInfo_Hide), ResourceType = typeof(Texts))]
        [ToggleSlider]
        public bool Hide { get => hide; set => Set(ref hide, value); }
        private bool hide = false;

        [Display(GroupName = nameof(Texts.GroupName_LayerInfo), AutoGenerateField = true)]
        public SourceSelectArgBase SourceSelectArg { get => sourceSelectArg; set => Set(ref sourceSelectArg, value); }
        private SourceSelectArgBase sourceSelectArg = new ImageFileParameter();

        [Display(GroupName = nameof(Texts.GroupName_LayerInfo), Name = nameof(Texts.PartParam_LayerInfo_Comment), ResourceType = typeof(Texts))]
        [TextEditor]
        public string Comment { get => comment; set => Set(ref comment, value); }
        private string comment = string.Empty;

        [Display(GroupName = nameof(Texts.GroupName_Drawing), AutoGenerateField = true)]
        public DrawingArgBase DrawingArg { get => drawingArg; set => Set(ref drawingArg, value); }
        private DrawingArgBase drawingArg = new HavingSourceParameter();

        public ControledParametersOfPart()
        {
        }

        public override ValueTask EndEditAsync()
        {
            SourceSelectArg = LayerType.Convert(SourceSelectArg);
            DrawingArg = LayerType.Convert(DrawingArg);
            return base.EndEditAsync();
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [SourceSelectArg];
    }
}
