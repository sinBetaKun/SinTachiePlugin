using SinTachiePlugin.Enums;
using SinTachiePlugin.LayerValueListController;
using SinTachiePlugin.PartAnimation.PartAnimarionLinkArgment;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;

namespace SinTachiePlugin.PartAnimation
{
    internal class PartAnimationValue : Animatable
    {
        [Display(GroupName = nameof(TextResource.PartAnimationOpeArg_GroupName), Name = nameof(TextResource.PartAnimationOpeMode), ResourceType = typeof(TextResource), Order = 2)]
        [EnumComboBox]
        public PartAnimationOpeMode AnmOpeMode { get => anmOpeMode; set => Set(ref anmOpeMode, value); }
        private PartAnimationOpeMode anmOpeMode = PartAnimationOpeMode.Simple;

        [Display(GroupName = nameof(TextResource.PartAnimationOpeArg_GroupName), Name = nameof(TextResource.PartAnimationNormalizationMode), ResourceType = typeof(TextResource), Order = 3)]
        [EnumComboBox]
        public PartAnimationNormalizationMode NormalizationMode { get => normalizationMode; set => Set(ref normalizationMode, value); }
        private PartAnimationNormalizationMode normalizationMode = PartAnimationNormalizationMode.Limit;

        [Display(GroupName = nameof(TextResource.PartAnimationOpeArg_GroupName), AutoGenerateField = true, ResourceType = typeof(TextResource), Order = 4)]
        public PartAnimationOpeArgBase AnmOpeArgments { get => anmOpeArgments; set => Set(ref anmOpeArgments, value); }
        private PartAnimationOpeArgBase anmOpeArgments = new SimpleParameter();

        [Display(GroupName = nameof(TextResource.PartAnimationOpeArg_GroupName), Name = nameof(TextResource.PartAnimationOpeArg_Comment), ResourceType = typeof(TextResource), Order = 5)]
        [TextEditor(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public string Comment { get => comment; set => Set(ref comment, value); }
        string comment = string.Empty;

        public PartAnimationValue()
        {
        }

        public PartAnimationValue(PartAnimationValue origin)
        {
            AnmOpeMode = origin.AnmOpeMode;
            NormalizationMode = origin.NormalizationMode;
            AnmOpeArgments = origin.AnmOpeArgments.GetClone();
            Comment = origin.Comment;
        }

        [Obsolete]
        public PartAnimationValue(LayerValue layerValue)
        {
            switch (layerValue.AnimationMode)
            {
                case LayerAnimationMode.CerrarPlusAbrir:
                    AnmOpeMode = PartAnimationOpeMode.Sum;
                    SumParameter sumParam = new();
                    sumParam.Cerrar.CopyFrom(layerValue.Cerrar);
                    sumParam.Abrir.CopyFrom(layerValue.Abrir);
                    AnmOpeArgments = sumParam;
                    break;

                case LayerAnimationMode.CerrarTimesAbrir:
                    AnmOpeMode = PartAnimationOpeMode.Product;
                    ProductParameter productParam = new();
                    productParam.Cerrar.CopyFrom(layerValue.Cerrar);
                    productParam.Abrir.CopyFrom(layerValue.Abrir);
                    AnmOpeArgments = productParam;
                    break;

                case LayerAnimationMode.Sin:
                    AnmOpeMode = PartAnimationOpeMode.Sin;
                    SinParameter sinParam = new();
                    sinParam.Cerrar.CopyFrom(layerValue.Cerrar);
                    sinParam.Abrir.CopyFrom(layerValue.Abrir);
                    AnmOpeArgments = sinParam;
                    break;

                case LayerAnimationMode.VoiceVolume:
                    AnmOpeMode = PartAnimationOpeMode.VoiceVolumePlus;
                    VoiceVolumePlusParameter voiceParam = new();
                    voiceParam.Cerrar.CopyFrom(layerValue.Cerrar);
                    voiceParam.Abrir.CopyFrom(layerValue.Abrir);
                    AnmOpeArgments = voiceParam;

                    if (layerValue.Extra is LayerValueListController.Extra.Parameter.VoiceVolumeParameter extra_voice)
                        voiceParam.Initial.CopyFrom(extra_voice.NoVoiceValue);

                    break;

                case LayerAnimationMode.PeriodicShuttle:
                    AnmOpeMode = PartAnimationOpeMode.PeriodicShuttle;
                    PeriodicShuttleParameter shuttleParam = new();
                    shuttleParam.Cerrar.CopyFrom(layerValue.Cerrar);
                    shuttleParam.Abrir.CopyFrom(layerValue.Abrir);
                    AnmOpeArgments = shuttleParam;

                    if (layerValue.Extra is LayerValueListController.Extra.Parameter.PeriodicParameter extra_period1)
                    {
                        shuttleParam.Offset.CopyFrom(extra_period1.Start);
                        shuttleParam.Interval.CopyFrom(extra_period1.Interval);
                        shuttleParam.Transition.CopyFrom(extra_period1.Transition);
                    }

                    break;

                case LayerAnimationMode.PeriodicLoop:
                    AnmOpeMode = PartAnimationOpeMode.PeriodicLoop;
                    PeriodicLoopParameter loopParam = new();
                    loopParam.Cerrar.CopyFrom(layerValue.Cerrar);
                    loopParam.Abrir.CopyFrom(layerValue.Abrir);
                    AnmOpeArgments = loopParam;

                    if (layerValue.Extra is LayerValueListController.Extra.Parameter.PeriodicParameter extra_period2)
                    {
                        loopParam.Offset.CopyFrom(extra_period2.Start);
                        loopParam.Interval.CopyFrom(extra_period2.Interval);
                        loopParam.Transition.CopyFrom(extra_period2.Transition);
                    }

                    break;
            }

            switch (layerValue.OuterMode)
            {
                case OuterLayerValueMode.Limit:
                    NormalizationMode = PartAnimationNormalizationMode.Limit;
                    break;

                case OuterLayerValueMode.Shuttle:
                    NormalizationMode = PartAnimationNormalizationMode.Shuttle;
                    break;

                case OuterLayerValueMode.Loop:
                    NormalizationMode |= PartAnimationNormalizationMode.Loop;
                    break;
            }

            Comment = layerValue.Comment;
        }

        public override void BeginEdit()
        {
            base.BeginEdit();
        }

        public override ValueTask EndEditAsync()
        {
            AnmOpeArgments = AnmOpeMode.Convert(AnmOpeArgments);

            return base.EndEditAsync();
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [AnmOpeArgments];
    }
}
