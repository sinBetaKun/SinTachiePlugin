using Newtonsoft.Json;
using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.LayerValueListController;
using SinTachiePlugin.Properties;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Plugin.Effects;

namespace SinTachiePlugin.Parts
{
    /// <summary>
    /// ユーザがコントローラーで編集できるパラメータをまとめたクラス。
    /// 何か新しくパラメータを追加したり、それらの管理システムを変更したいときは、このクラスを編集すること。
    /// </summary>
    [Obsolete]
    public class ControlledParamsOfPart : Animatable
    {
        public bool Appear { get => appear; set => Set(ref appear, value); }
        bool appear = true;

        public string TagName { get => tagName; set => Set(ref tagName, value); }
        string tagName = string.Empty;

        public string Parent { get => parent; set => Set(ref parent, value); }
        string parent = string.Empty;

        public Animation BusNum { get; } = new Animation(0, -1000, 1000);

        public string ImagePath
        {
            get => imagePath;
            set
            {
                Set(ref imagePath, value);
            }
        }
        string imagePath = string.Empty;

        public string Comment { get => comment; set => Set(ref comment, value); }
        string comment = string.Empty;

        public ImmutableList<LayerValue> LayerValues { get => layerValue; set => Set(ref layerValue, value); }
        ImmutableList<LayerValue> layerValue = [];

        public Animation X { get; } = new Animation(0, -10000, 10000);

        public Animation Y { get; } = new Animation(0, -10000, 10000);

        public Animation Z { get; } = new Animation(0, -10000, 10000);

        public Animation Opacity { get; } = new Animation(100, 0, 100);

        public Animation Scale { get; } = new Animation(100, 0, 5000);

        public Animation Rotate { get; } = new Animation(0, -36000, 36000, 360);

        public Animation Mirror { get; } = new Animation(0, 0, 1);

        public BlendSTP BlendMode { get => blendMode; set { Set(ref blendMode, value); } }

        BlendSTP blendMode = BlendSTP.SourceOver;

        public ZSortMode ZSortMode { get => zSortMode; set { Set(ref zSortMode, value); } }

        ZSortMode zSortMode = ZSortMode.BusScreen;

        public bool XYZDependent { get => xyzDependent; set => Set(ref xyzDependent, value); }
        bool xyzDependent = true;

        public bool OpacityDependent { get => opacityDependent; set => Set(ref opacityDependent, value); }
        bool opacityDependent = true;
        
        public bool ScaleDependent { get => scaleDependent; set => Set(ref scaleDependent, value); }
        bool scaleDependent = true;

        public bool RotateDependent { get => rotateDependent; set => Set(ref rotateDependent, value); }
        bool rotateDependent = true;

        public bool MirrorDependent { get => mirrorDependent; set => Set(ref mirrorDependent, value); }
        bool mirrorDependent = true;

        public bool CameraDependent { get => cameraDependent; set => Set(ref cameraDependent, value); }
        bool cameraDependent = true;

        public bool UnlazyEffectDependent { get => unlazyEffectDependent; set => Set(ref unlazyEffectDependent, value); }
        bool unlazyEffectDependent = true;

        public Animation Cnt_X { get; } = new Animation(0, -10000, 10000);

        public Animation Cnt_Y { get; } = new Animation(0, -10000, 10000);

        public bool KeepPlace { get => keepPlace; set => Set(ref keepPlace, value); }
        bool keepPlace = false;

        public Animation Exp_X { get; } = new Animation(100, 0, 5000);

        public Animation Exp_Y { get; } = new Animation(100, 0, 5000);

        public ImmutableList<IVideoEffect> Effects { get => effects; set => Set(ref effects, value); }
        ImmutableList<IVideoEffect> effects = [];

        public void CopyFrom(ControlledParamsOfPart original)
        {
            Appear = original.Appear;
            BusNum.CopyFrom(original.BusNum);
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
            CameraDependent = original.CameraDependent;
            UnlazyEffectDependent = original.UnlazyEffectDependent;

            try
            {
                string effectsStr = JsonConvert.SerializeObject(original.Effects, Formatting.Indented, GetJsonSetting);
                if (JsonConvert.DeserializeObject<IVideoEffect[]>(effectsStr, GetJsonSetting) is IVideoEffect[] effects)
                {
                    Effects = [.. effects];
                }
                else
                {
                    throw new Exception(TextResource.PartEffectArg_Error_DeserializeVideoEffects);
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
