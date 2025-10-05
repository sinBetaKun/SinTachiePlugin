using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinTachiePlugin.Enums;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment.Parameter;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter;
using SinTachiePlugin.Properties;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;

namespace SinTachiePlugin.PartAnimation
{
    internal class PartAnimationValue : Animatable
    {
        [Display(GroupName = nameof(Texts.PartAnimationOpeArg_GroupName), Name = nameof(Texts.PartAnimationOpeArg_AnimationTag), ResourceType = typeof(Texts))]
        [TextEditor]
        public string AnimationTag { get => animationTag; set => Set(ref animationTag, value); }
        private string animationTag = string.Empty;


        [Display(GroupName = nameof(Texts.PartAnimationOpeArg_GroupName), Name = nameof(Texts.PartAnimationOpeArg_AnimationTag), ResourceType = typeof(Texts))]
        [TextBoxSlider("F0", "", 0, 3)]
        [DefaultValue(0d)]
        [Range(0, 15)]
        public int Index { get => index; set => Set(ref index, value); }
        private int index = 0;

        [Display(GroupName = nameof(Texts.PartAnimationOpeArg_GroupName), Name = nameof(Texts.PartAnimationOpeMode), ResourceType = typeof(Texts))]
        [EnumComboBox]
        public PartAnimationOpeMode AnmOpeMode { get => anmOpeMode; set => Set(ref anmOpeMode, value); }
        private PartAnimationOpeMode anmOpeMode = PartAnimationOpeMode.Simple;

        [Display(GroupName = nameof(Texts.PartAnimationOpeArg_GroupName), Name = nameof(Texts.PartAnimationNormalizationMode), ResourceType = typeof(Texts))]
        [EnumComboBox]
        public PartAnimationNormalizationMode NormalizationMode { get => normalizationMode; set => Set(ref normalizationMode, value); }
        private PartAnimationNormalizationMode normalizationMode = PartAnimationNormalizationMode.Limit;

        [Display(GroupName = nameof(Texts.PartAnimationOpeArg_GroupName), AutoGenerateField = true, ResourceType = typeof(Texts))]
        public PartAnimationOpeArgBase AnmOpeArgments { get => anmOpeArgments; set => Set(ref anmOpeArgments, value); }
        private PartAnimationOpeArgBase anmOpeArgments = new SimpleParameter();

        [Display(GroupName = nameof(Texts.PartAnimationOpeArg_GroupName), Name = nameof(Texts.PartAnimationOpeArg_Comment), ResourceType = typeof(Texts))]
        [TextEditor(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public string Comment { get => comment; set => Set(ref comment, value); }
        string comment = string.Empty;

        [Display(GroupName = nameof(Texts.PartAnimationLink_GroupName), Name = nameof(Texts.PartAnimationLink_LinkOpeMode), ResourceType = typeof(Texts))]
        [EnumComboBox]
        public PartAnimationLinkOpeMode LinkOpeMode { get => linkMode; set => Set(ref linkMode, value); }
        private PartAnimationLinkOpeMode linkMode = PartAnimationLinkOpeMode.DontLink;

        [Display(GroupName = nameof(Texts.PartAnimationLink_GroupName), AutoGenerateField = true, ResourceType = typeof(Texts))]
        public PartAnimationLinkArgBase LinkArgments { get => linkArgments; set => Set(ref linkArgments, value); }
        private PartAnimationLinkArgBase linkArgments = new DontLinkParameter();

        public PartAnimationValue()
        {
        }

        public PartAnimationValue(PartAnimationValue origin)
        {
            AnimationTag = origin.AnimationTag;
            Index = origin.Index;
            AnmOpeMode = origin.AnmOpeMode;
            NormalizationMode = origin.NormalizationMode;
            AnmOpeArgments = AnmOpeMode.Convert(origin.AnmOpeArgments);
            Comment = origin.Comment;
            LinkOpeMode = origin.LinkOpeMode;
            LinkArgments = LinkOpeMode.Convert(origin.LinkArgments);
        }

        public override void BeginEdit()
        {
            base.BeginEdit();
        }

        public override ValueTask EndEditAsync()
        {
            AnmOpeArgments = AnmOpeMode.Convert(AnmOpeArgments);
            LinkArgments = LinkOpeMode.Convert(LinkArgments);

            return base.EndEditAsync();
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [AnmOpeArgments];
    }
}
