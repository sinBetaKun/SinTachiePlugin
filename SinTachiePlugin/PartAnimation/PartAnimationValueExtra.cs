using SinTachiePlugin.Enums;
using SinTachiePlugin.LayerValueListController;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Parameter;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using SinTachiePlugin.Properties;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Controls;

namespace SinTachiePlugin.PartAnimation
{
    internal class PartAnimationValueExtra : PartAnimationValue
    {
        [Display(GroupName = nameof(TextResource.PartAnimationOpeArg_GroupName), Name = nameof(TextResource.PartAnimationOpeArg_AnimationTag), ResourceType = typeof(TextResource), Order = 0)]
        [TextEditor]
        public string AnimationTag { get => animationTag; set => Set(ref animationTag, value); }
        private string animationTag = string.Empty;

        [Display(GroupName = nameof(TextResource.PartAnimationOpeArg_GroupName), Name = nameof(TextResource.PartAnimationOpeArg_AnimationTag), ResourceType = typeof(TextResource), Order = 1)]
        [TextBoxSlider("F0", "", 0, 3)]
        [DefaultValue(0d)]
        [Range(0, 16)]
        public int Index { get => index; set => Set(ref index, value); }
        private int index = 0;

        [Display(GroupName = nameof(TextResource.PartAnimationLink_GroupName), Name = nameof(TextResource.PartAnimationLink_LinkOpeMode), ResourceType = typeof(TextResource), Order = 6)]
        [EnumComboBox]
        public PartAnimationLinkOpeMode LinkOpeMode { get => linkMode; set => Set(ref linkMode, value); }
        private PartAnimationLinkOpeMode linkMode = PartAnimationLinkOpeMode.DontLink;

        [Display(GroupName = nameof(TextResource.PartAnimationLink_GroupName), AutoGenerateField = true, ResourceType = typeof(TextResource), Order = 7)]
        public PartAnimationLinkArgBase LinkArgments { get => linkArgments; set => Set(ref linkArgments, value); }
        private PartAnimationLinkArgBase linkArgments = new DontLinkParameter();

        public PartAnimationValueExtra()
        {
        }

        public PartAnimationValueExtra(PartAnimationValue origin) : base(origin)
        {
        }

        public PartAnimationValueExtra(PartAnimationValueExtra origin) : base(origin)
        {
            AnimationTag = origin.AnimationTag;
            Index = origin.Index;
            LinkOpeMode = origin.LinkOpeMode;
            LinkArgments = origin.LinkArgments.GetClone();
        }

        public override ValueTask EndEditAsync()
        {
            LinkArgments = LinkOpeMode.Convert(LinkArgments);

            return base.EndEditAsync();
        }

        [Obsolete]
        public PartAnimationValueExtra(LayerValue layerValue) : base(layerValue)
        {
        }
    }
}
