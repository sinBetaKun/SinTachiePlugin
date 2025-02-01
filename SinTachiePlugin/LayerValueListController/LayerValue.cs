using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.LayerValueListController.Extra;
using SinTachiePlugin.LayerValueListController.Extra.Parameter;
using SinTachiePlugin.Parts;
using SinTachiePlugin.Parts.LayerValueListController;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;

namespace SinTachiePlugin.LayerValueListController
{
    public class LayerValue : Animatable
    {
        [Display(GroupName = "差分制御", Name = "制御モード", Description = "セラールとアブリールの値の使い方を指定")]
        [EnumComboBox]
        public LayerAnimationMode AnimationMode { get => animationMode; set => Set(ref animationMode, value); }
        LayerAnimationMode animationMode = LayerAnimationMode.CerrarPlusAbrir;

        [Display(GroupName = "差分制御", Name = "範囲外の値", Description = "0%~100%の範囲外になったときの値の変換方法を指定")]
        [EnumComboBox]
        public OuterLayerValueMode OuterMode { get => outerMode; set => Set(ref outerMode, value); }
        OuterLayerValueMode outerMode = OuterLayerValueMode.Limit;

        [Display(GroupName = "差分制御", Name = "セラール")]
        [AnimationSlider("F1", "%", -150, 150)]
        public Animation Cerrar { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = "差分制御", Name = "アブリール")]
        [AnimationSlider("F1", "%", -150, 150)]
        public Animation Abrir { get; } = new Animation(100, -10000, 10000);

        [Display(GroupName = "差分制御", AutoGenerateField = true)]
        public LayerValueExtraBase Extra {
            get => extra;
            set => Set(ref extra, value);
        }
        LayerValueExtraBase extra = new NoExtra();

        [Display(GroupName = "差分制御", Name = "備考")]
        [TextEditor(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public string Comment { get => comment; set => Set(ref comment, value); }
        string comment = string.Empty;

        public LayerValue()
        {
        }

        public LayerValue(LayerValue block)
        {
            AnimationMode = block.AnimationMode;
            OuterMode = block.OuterMode;
            Cerrar.CopyFrom(block.Cerrar);
            Abrir.CopyFrom(block.Abrir);
            Extra = AnimationMode.Convert(Extra);
            Extra.CopyFrom(block.Extra);
            Comment = block.Comment;
        }

        public double GetValue(FrameAndLength fl, int fps, double voiceVolume)
        {
            double cerrar = Cerrar.GetValue(fl.Frame, fl.Length, fps);
            double abrir = Abrir.GetValue(fl.Frame, fl.Length, fps);
            double num;
            string clsName = GetType().Name;
            string? mthName = MethodBase.GetCurrentMethod()?.Name;
            double extraValue = Extra.GetValue(fl.Frame, fl.Length, fps);
            switch (AnimationMode)
            {
                case LayerAnimationMode.CerrarPlusAbrir:
                    num = cerrar + abrir;
                    break;
                case LayerAnimationMode.CerrarTimesAbrir:
                    num = cerrar * abrir / 100;
                    break;
                case LayerAnimationMode.Sin:
                    num = cerrar * Math.Sin(abrir / 100 * 2 * Math.PI);
                    break;
                case LayerAnimationMode.VoiceVolume:
                    num = cerrar + voiceVolume * (abrir - cerrar);
                    break;
                case LayerAnimationMode.PeriodicShuttle:
                    extraValue = (0.5 - extraValue) * (extraValue <= 0.5 ? 2 : -2);
                    num = cerrar + extraValue * (abrir - cerrar);
                    break;
                case LayerAnimationMode.PeriodicLoop:
                    num = cerrar + extraValue * (abrir - cerrar);
                    break;
                default:
                    string message = $"存在しないタイプの制御モードが指定されました。\n(AnimationMode = {AnimationMode})";
                    SinTachieDialog.ShowError(message, clsName, mthName);
                    throw new Exception($"[{PluginInfo.Title}]{message}");
            }

            switch (OuterMode)
            {
                case OuterLayerValueMode.Limit:
                    return num < 0 ? 0 : num > 100 ? 1 : num / 100;
                case OuterLayerValueMode.Shuttle:
                    while (num < 0) num += 200;
                    return (num <= 100 ? num : 200 - num) / 100;
                case OuterLayerValueMode.Loop:
                    while (num < 0) num += 100;
                    return num % 100 / 100;
                default:
                    string message = $"存在しないタイプの範囲外変換モードが指定されました。\n(OuterMode = {OuterMode})";
                    SinTachieDialog.ShowError(message, clsName, mthName);
                    throw new Exception($"[{PluginInfo.Title}]{message}");
            }
        }

        public override void BeginEdit()
        {
            base.BeginEdit();
        }

        public override ValueTask EndEditAsync()
        {
            Extra = AnimationMode.Convert(Extra);

            return base.EndEditAsync();
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Cerrar, Abrir, Extra];
    }
}
