using Newtonsoft.Json;
using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.LayerValueListController;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Plugin.Effects;
using SinTachiePlugin.Parts.Controller;
using SinTachiePlugin.LayerValueListController.Controller;

namespace SinTachiePlugin.Parts
{
    /// <summary>
    /// ユーザがコントローラーで編集できるパラメータをまとめたクラス。
    /// 何か新しくパラメータを追加したり、それらの管理システムを変更したいときは、このクラスを編集すること。
    /// </summary>
    public class ControlledParamsOfPart : Animatable
    {
        public bool Appear { get => appear; set => Set(ref appear, value); }
        bool appear = true;

        [Display(GroupName = nameof(Resources.GroupeName_BlockInfo), Name = nameof(Resources.ParamName_TagName), ResourceType = typeof(Resources))]
        [TextEditor(AcceptsReturn = true)]
        public string TagName { get => tagName; set => Set(ref tagName, value); }
        string tagName = string.Empty;

        [Display(GroupName = nameof(Resources.GroupeName_BlockInfo), Name = nameof(Resources.ParamName_Parent), ResourceType = typeof(Resources))]
        [TextEditor(AcceptsReturn = true)]
        public string Parent { get => parent; set => Set(ref parent, value); }
        string parent = string.Empty;

        /// <summary>
        /// 描画時に現在の値を取得する際は GetBusNum を呼び出す。
        /// ここから直々に GetValue すると、Listbox の一要素の UI に変化が起きなくなってしまう。
        /// </summary>
        [Display(GroupName = nameof(Resources.GroupeName_BlockInfo), Name = nameof(Resources.ParamName_BusNum), ResourceType = typeof(Resources))]
        [AnimationSlider("F0", "", -50, 50)]
        public Animation BusNum { get; } = new Animation(0, -1000, 1000);
/*
        /// <summary>
        /// Listbox の一要素の UI を変化させて、ユーザに見やすくしたいんじゃ。
        /// </summary>
        [JsonIgnore]
        public int BusNumView { get => busNumView; set => Set(ref busNumView, value); }
        int busNumView = 0;

        /// <summary>
        /// BusNum の現在の値を取得する際はこちらを呼び出す
        /// </summary>
        public int GetBusNum(long frame, long length, int fps)
        {
            BusNumView = (int)BusNum.GetValue(frame, length, fps);
            return BusNumView;
        }*/

        [Display(GroupName = nameof(Resources.GroupeName_BlockInfo), Name = nameof(Resources.ParamName_ImagePath), ResourceType = typeof(Resources))]
        [FileSelectorForPartOfSinTachie]
        public string ImagePath
        {
            get => imagePath;
            set
            {
                Set(ref imagePath, value);
            }
        }
        string imagePath = string.Empty;

        [Display(GroupName = nameof(Resources.GroupeName_BlockInfo), Name = nameof(Resources.ParamName_Comment), ResourceType = typeof(Resources))]
        [TextEditor(AcceptsReturn = true)]
        public string Comment { get => comment; set => Set(ref comment, value); }
        string comment = string.Empty;

        [Display(GroupName = nameof(Resources.GroupeName_BlockInfo), Name = nameof(Resources.ParamName_LayerValues), ResourceType = typeof(Resources))]
        [LayerValueListController(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<LayerValue> LayerValues { get => layerValue; set => Set(ref layerValue, value); }
        ImmutableList<LayerValue> layerValue = [];

        [Display(GroupName = nameof(Resources.GroupeName_Drawing), Name = nameof(Resources.ParamName_X), ResourceType = typeof(Resources))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation X { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = nameof(Resources.GroupeName_Drawing), Name = nameof(Resources.ParamName_Y), ResourceType = typeof(Resources))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Y { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = nameof(Resources.GroupeName_Drawing), Name = nameof(Resources.ParamName_Z), ResourceType = typeof(Resources))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Z { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = nameof(Resources.GroupeName_Drawing), Name = nameof(Resources.ParamName_Opacity), ResourceType = typeof(Resources))]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation Opacity { get; } = new Animation(100, 0, 100);

        [Display(GroupName = nameof(Resources.GroupeName_Drawing), Name = nameof(Resources.ParamName_Scale), ResourceType = typeof(Resources))]
        [AnimationSlider("F1", "%", 0, 200)]
        public Animation Scale { get; } = new Animation(100, 0, 5000);

        [Display(GroupName = nameof(Resources.GroupeName_Drawing), Name = nameof(Resources.ParamName_Rotate), ResourceType = typeof(Resources))]
        [AnimationSlider("F1", "°", -360, 360)]
        public Animation Rotate { get; } = new Animation(0, -36000, 36000, 360);

        [Display(GroupName = nameof(Resources.GroupeName_Drawing), Name = nameof(Resources.ParamName_Mirror), ResourceType = typeof(Resources))]
        [AnimationSlider("F0", "", 0, 1)]
        public Animation Mirror { get; } = new Animation(0, 0, 1);

        [Display(GroupName = nameof(Resources.GroupeName_Drawing), Name = nameof(Resources.ParamName_Composition), ResourceType = typeof(Resources))]
        [EnumComboBox]
        public BlendSTP BlendMode { get => blendMode; set { Set(ref blendMode, value); } }

        BlendSTP blendMode = BlendSTP.SourceOver;

        [Display(GroupName = nameof(Resources.GroupeName_Drawing), Name = nameof(Resources.ParamName_ZOrder), ResourceType = typeof(Resources))]
        [EnumComboBox]
        public ZSortMode ZSortMode { get => zSortMode; set { Set(ref zSortMode, value); } }

        ZSortMode zSortMode = ZSortMode.BusScreen;

        [Display(GroupName = nameof(Resources.GroupeName_ValueDependent), Name = nameof(Resources.ParamName_XYZ), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool XYZDependent { get => xyzDependent; set => Set(ref xyzDependent, value); }
        bool xyzDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_ValueDependent), Name = nameof(Resources.ParamName_Opacity), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool OpacityDependent { get => opacityDependent; set => Set(ref opacityDependent, value); }
        bool opacityDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_ValueDependent), Name = nameof(Resources.ParamName_Scale), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool ScaleDependent { get => scaleDependent; set => Set(ref scaleDependent, value); }
        bool scaleDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_ValueDependent), Name = nameof(Resources.ParamName_Rotate), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool RotateDependent { get => rotateDependent; set => Set(ref rotateDependent, value); }
        bool rotateDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_ValueDependent), Name = nameof(Resources.ParamName_Mirror), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool MirrorDependent { get => mirrorDependent; set => Set(ref mirrorDependent, value); }
        bool mirrorDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_CenterPoint), Name = nameof(Resources.ParamName_X), ResourceType = typeof(Resources))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Cnt_X { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = nameof(Resources.GroupeName_CenterPoint), Name = nameof(Resources.ParamName_Y), ResourceType = typeof(Resources))]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Cnt_Y { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = nameof(Resources.GroupeName_CenterPoint), Name = nameof(Resources.ParamName_KeepPlace), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool KeepPlace { get => keepPlace; set => Set(ref keepPlace, value); }
        bool keepPlace = false;

        [Display(GroupName = nameof(Resources.GroupeName_SubScale), Name = nameof(Resources.ParamName_Exp_X), ResourceType = typeof(Resources))]
        [AnimationSlider("F1", "%", 0, 200)]
        public Animation Exp_X { get; } = new Animation(100, 0, 5000);

        [Display(GroupName = nameof(Resources.GroupeName_SubScale), Name = nameof(Resources.ParamName_Exp_Y), ResourceType = typeof(Resources))]
        [AnimationSlider("F1", "%", 0, 200)]
        public Animation Exp_Y { get; } = new Animation(100, 0, 5000);

        [Display(GroupName = nameof(Resources.GroupeName_EffectDependent), Name = nameof(Resources.ParamName_XYZ), Description = nameof(Resources.ParamDesc_EffectXYZDependent), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool EffectXYZDependent { get => effectXYZDependent; set => Set(ref effectXYZDependent, value); }
        bool effectXYZDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_EffectDependent), Name = nameof(Resources.ParamName_Opacity), Description = nameof(Resources.ParamDesc_EffectOpacityDependent), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool EffectOpacityDependent { get => effectOpacityDependent; set => Set(ref effectOpacityDependent, value); }
        bool effectOpacityDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_EffectDependent), Name = nameof(Resources.ParamName_Scale), Description = nameof(Resources.ParamDesc_EffectZoomDependent), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool EffectZoomDependent { get => effectZoomDependent; set => Set(ref effectZoomDependent, value); }
        bool effectZoomDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_EffectDependent), Name = nameof(Resources.ParamName_Rotate), Description = nameof(Resources.ParamDesc_EffectRotateDependent), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool EffectRotateDependent { get => effectRotateDependent; set => Set(ref effectRotateDependent, value); }
        bool effectRotateDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_EffectDependent), Name = nameof(Resources.ParamName_Mirror), Description = nameof(Resources.ParamDesc_EffectMirrorDependent), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool EffectMirrorDependent { get => effectMirrorDependent; set => Set(ref effectMirrorDependent, value); }
        bool effectMirrorDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_EffectDependent), Name = nameof(Resources.ParamName_Camera), Description = nameof(Resources.ParamDesc_EffectCameraDependent), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool EffectCameraDependent { get => effectCameraDependent; set => Set(ref effectCameraDependent, value); }
        bool effectCameraDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_EffectDependent), Name = nameof(Resources.ParamName_EffectUnlazyDependent), Description = nameof(Resources.ParamDesc_EffectUnlazyDependent), ResourceType = typeof(Resources))]
        [ToggleSlider]
        public bool EffectUnlazyDependent { get => effectUnlazyDependent; set => Set(ref effectUnlazyDependent, value); }
        bool effectUnlazyDependent = true;

        [Display(GroupName = nameof(Resources.GroupeName_PartEffect))]
        [VideoEffectSelector(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<IVideoEffect> Effects { get => effects; set => Set(ref effects, value); }
        ImmutableList<IVideoEffect> effects = [];

        public void CopyFrom(ControlledParamsOfPart original)
        {
            Appear = original.Appear;
            BusNum.CopyFrom(original.BusNum);
            //BusNumView = original.BusNumView;
            TagName = original.TagName;
            Parent = original.Parent;
            ImagePath = original.ImagePath;
            Comment = original.Comment;
            LayerValues = original.LayerValues.Select(x => new LayerValue(x)).ToImmutableList();
            X.CopyFrom(original.X);
            Y.CopyFrom(original.Y);
            Z.CopyFrom(original.Z);
            Opacity.CopyFrom(original.Opacity);
            Scale.CopyFrom(original.Scale);
            Rotate.CopyFrom(original.Rotate);
            Mirror.CopyFrom(original.Mirror);
            BlendMode = original.BlendMode;
            ZSortMode = original.ZSortMode;
            Cnt_X.CopyFrom(original.Cnt_X);
            Cnt_Y.CopyFrom(original.Cnt_Y);
            KeepPlace = original.KeepPlace;
            Exp_X.CopyFrom(original.Exp_X);
            Exp_Y.CopyFrom(original.Exp_Y);
            XYZDependent = original.XYZDependent;
            ScaleDependent = original.ScaleDependent;
            OpacityDependent = original.OpacityDependent;
            RotateDependent = original.RotateDependent;
            MirrorDependent = original.MirrorDependent;
            EffectXYZDependent = original.EffectXYZDependent;
            EffectZoomDependent = original.EffectZoomDependent;
            EffectOpacityDependent = original.EffectOpacityDependent;
            EffectRotateDependent = original.EffectRotateDependent;
            EffectMirrorDependent = original.EffectMirrorDependent;
            EffectCameraDependent = original.EffectCameraDependent;
            EffectUnlazyDependent = original.EffectUnlazyDependent;
            try
            {
                string effectsStr = JsonConvert.SerializeObject(original.Effects, Newtonsoft.Json.Formatting.Indented, GetJsonSetting);
                if (JsonConvert.DeserializeObject<IVideoEffect[]>(effectsStr, GetJsonSetting) is IVideoEffect[] effects)
                {
                    Effects = [.. effects];
                }
                else
                {
                    throw new Exception(Resources.ErrorLog_DeserializeVideoEffects);
                }
            }
            catch (Exception ex)
            {
                Effects = [];
                SinTachieDialog.ShowWarning(ex.Message);
            }
        }

        public static JsonSerializerSettings GetJsonSetting =>
            new()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

        protected override IEnumerable<IAnimatable> GetAnimatables() => [BusNum, ..LayerValues, X, Y, Z, Opacity, Scale, Rotate, Mirror, Cnt_X, Cnt_Y, Exp_X, Exp_Y, .. Effects];
    }
}
