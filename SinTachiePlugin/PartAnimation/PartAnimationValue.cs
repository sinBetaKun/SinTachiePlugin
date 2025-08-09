using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SinTachiePlugin.Enums;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter;
using SinTachiePlugin.Properties;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;

namespace SinTachiePlugin.PartAnimation
{
    internal class PartAnimationValue : Animatable
    {
        [Display(GroupName = nameof(Text.PartAnimationOpeArg_GroupName), Name = nameof(Text.PartAnimationOpeMode), ResourceType = typeof(Text))]
        [EnumComboBox]
        public PartAnimationOpeMode OpeMode { get => opeMode; set => Set(ref opeMode, value); }
        private PartAnimationOpeMode opeMode = PartAnimationOpeMode.Simple;

        [Display(GroupName = nameof(Text.PartAnimationOpeArg_GroupName), Name = nameof(Text.PartAnimationNormalizationMode), ResourceType = typeof(Text))]
        [EnumComboBox]
        public PartAnimationNormalizationMode NormalizationMode { get => normalizationMode; set => Set(ref normalizationMode, value); }
        private PartAnimationNormalizationMode normalizationMode = PartAnimationNormalizationMode.Limit;

        [Display(GroupName = nameof(Text.PartAnimationOpeArg_GroupName), AutoGenerateField = true, ResourceType = typeof(Text))]
        public PartAnimationOpeArgBase Argments { get => argments; set => Set(ref argments, value); }
        private PartAnimationOpeArgBase argments = new SimpleParameter();

        [Display(GroupName = nameof(Text.PartAnimationLink_GroupName), Name = nameof(Text.PartAnimationLink_LinkMode), ResourceType = typeof(Text))]
        [EnumComboBox]
        public PartAnimationLinkOpeMode LinkMode { get => linkMode; set => Set(ref linkMode, value); }
        private PartAnimationLinkOpeMode linkMode = PartAnimationLinkOpeMode.Override;
    }
}
