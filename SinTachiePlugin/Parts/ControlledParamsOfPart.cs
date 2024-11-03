using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using SinTachiePlugin.Effects;
using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.LayerValueListController;
using SinTachiePlugin.Parts.LayerValueListController;
using SinTachiePlugin.Parts;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vortice;
using Windows.Services.Maps;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;

namespace SinTachiePlugin.Parts
{
    /// <summary>
    /// ユーザがコントローラーで編集できるパラメータをまとめたクラス。
    /// 何か新しくパラメータを追加したり、それらの管理システムを変更したいときは、このクラスを編集すること。
    /// </summary>
    public class ControlledParamsOfPart : SinTachieDialog
    {
        public bool Appear { get => appear; set { Set(ref appear, value); } }
        bool appear = true;

        [Display(GroupName = "ブロック情報", Name = "タグ")]
        [TextEditor(AcceptsReturn = true)]
        public string TagName { get => tagName; set => Set(ref tagName, value); }
        string tagName = string.Empty;

        [Display(GroupName = "ブロック情報", Name = "親")]
        [TextEditor(AcceptsReturn = true)]
        public string Parent { get => parent; set => Set(ref parent, value); }
        string parent = string.Empty;

        /// <summary>
        /// 描画時に現在の値を取得する際は GetBusNum を呼び出す。
        /// ここから直々に GetValue すると、Listbox の一要素の UI に変化が起きなくなってしまう。
        /// </summary>
        [Display(GroupName = "ブロック情報", Name = "バス")]
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

        [Display(GroupName = "ブロック情報", Name = "画像")]
        [FileSelectorForPartOfSinTachie]
        public string ImagePath
        {
            get => imagePath;
            set
            {
                Set(ref imagePath, value);
                SetImageSource(value);
            }
        }
        string imagePath = string.Empty;

        protected void SetOnlyImagePth(string input)
        {
            imagePath = input;
        }

        [JsonIgnore]
        public BitmapSource? ImageSource { get => imageSource; set => Set(ref imageSource, value); }
        BitmapSource? imageSource = null;

        async void SetImageSource(string file)
        {
            ImageSource = await ShellThumbnail.LoadCroppedThumbnailAsync(file);
        }

        [Display(GroupName = "ブロック情報", Name = "レイヤー")]
        [LayerValueListController(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public ImmutableList<LayerValue> LayerValues { get => layerValue; set => Set(ref layerValue, value); }
        ImmutableList<LayerValue> layerValue = [];

        [Display(GroupName = "描画", Name = "X")]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation X { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = "描画", Name = "Y")]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Y { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = "描画", Name = "不透明度")]
        [AnimationSlider("F1", "%", 0, 100)]
        public Animation Opacity { get; } = new Animation(100, 0, 100);

        [Display(GroupName = "描画", Name = "拡大率")]
        [AnimationSlider("F1", "%", 0, 200)]
        public Animation Scale { get; } = new Animation(100, 0, 5000);

        [Display(GroupName = "描画", Name = "回転角")]
        [AnimationSlider("F1", "°", -360, 360)]
        public Animation Rotate { get; } = new Animation(0, -36000, 36000, 360);

        [Display(GroupName = "描画", Name = "左右反転")]
        [AnimationSlider("F0", "", -1, 2)]
        public Animation Mirror { get; } = new Animation(0, 0, 1);

        [Display(GroupName = "描画", Name = "合成モード")]
        [EnumComboBox]
        public BlendSTP BlendMode { get => blendMode; set { Set(ref blendMode, value); } }

        BlendSTP blendMode = BlendSTP.SourceOver;

        [Display(GroupName = "描画", Name = "拡大率依存", Description = "拡大率依存")]
        [ToggleSlider]
        public bool ScaleDependent { get => scaleDependent; set => Set(ref scaleDependent, value); }
        bool scaleDependent = true;

        [Display(GroupName = "描画", Name = "不透明度依存", Description = "不透明度依存")]
        [ToggleSlider]
        public bool OpacityDependent { get => opacityDependent; set => Set(ref opacityDependent, value); }
        bool opacityDependent = true;

        [Display(GroupName = "描画", Name = "回転角依存", Description = "回転角依存")]
        [ToggleSlider]
        public bool RotateDependent { get => rotateDependent; set => Set(ref rotateDependent, value); }
        bool rotateDependent = true;

        [Display(GroupName = "中心位置", Name = "X")]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Cnt_X { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = "中心位置", Name = "Y")]
        [AnimationSlider("F1", "px", -500, 500)]
        public Animation Cnt_Y { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = "中心位置", Name = "位置を保持")]
        [ToggleSlider]
        public bool KeepPlace { get => keepPlace; set => Set(ref keepPlace, value); }
        bool keepPlace = false;

        [Display(GroupName = "サブ拡大率", Name = "横方向")]
        [AnimationSlider("F1", "%", 0, 200)]
        public Animation Exp_X { get; } = new Animation(100, 0, 5000);

        [Display(GroupName = "サブ拡大率", Name = "縦方向")]
        [AnimationSlider("F1", "%", 0, 200)]
        public Animation Exp_Y { get; } = new Animation(100, 0, 5000);

        //[Display(GroupName = "クリッピング", Name = "上")]
        //[AnimationSlider("F1", "px", 0, 500)]
        //public Animation Top { get; } = new Animation(0, 0, 10000);

        //[Display(GroupName = "クリッピング", Name = "下")]
        //[AnimationSlider("F1", "px", 0, 500)]
        //public Animation Bottom { get; } = new Animation(0, 0, 10000);

        //[Display(GroupName = "クリッピング", Name = "左")]
        //[AnimationSlider("F1", "px", 0, 500)]
        //public Animation Left { get; } = new Animation(0, 0, 10000);

        //[Display(GroupName = "クリッピング", Name = "右")]
        //[AnimationSlider("F1", "px", 0, 500)]
        //public Animation Right { get; } = new Animation(0, 0, 10000);

        //[Display(GroupName = "ぼかし", Name = "ぼかし度")]
        //[AnimationSlider("F1", "px", 0, 50)]
        //public Animation GBlurValue { get; } = new Animation(0, 0, 250);

        //[Display(GroupName = "方向ブラー", Name = "ぼかし度")]
        //[AnimationSlider("F1", "", 0, 50)]
        //public Animation DBlurValue { get; } = new Animation(0, 0, 250);

        //[Display(GroupName = "方向ブラー", Name = "角度")]
        //[AnimationSlider("F1", "°", -360, 360)]
        //public Animation DBlurAngle { get; } = new Animation(0, -36000, 36000, 360);

        public void CopyFrom(ControlledParamsOfPart original)
        {
            Appear = original.Appear;
            BusNum.CopyFrom(original.BusNum);
            //BusNumView = original.BusNumView;
            TagName = original.TagName;
            Parent = original.Parent;
            ImagePath = original.ImagePath;
            LayerValues = original.LayerValues.Select(x => new LayerValue(x)).ToImmutableList();
            X.CopyFrom(original.X);
            Y.CopyFrom(original.Y);
            Opacity.CopyFrom(original.Opacity);
            Scale.CopyFrom(original.Scale);
            Rotate.CopyFrom(original.Rotate);
            Mirror.CopyFrom(original.Mirror);
            BlendMode = original.BlendMode;
            Cnt_X.CopyFrom(original.Cnt_X);
            Cnt_Y.CopyFrom(original.Cnt_Y);
            KeepPlace = original.KeepPlace;
            Exp_X.CopyFrom(original.Exp_X);
            Exp_Y.CopyFrom(original.Exp_Y);
            //Top.CopyFrom(original.Top);
            //Bottom.CopyFrom(original.Bottom);
            //Left.CopyFrom(original.Left);
            //Right.CopyFrom(original.Right);
            //GBlurValue.CopyFrom(original.GBlurValue);
            //DBlurValue.CopyFrom(original.DBlurValue);
            //DBlurAngle.CopyFrom(original.DBlurAngle);
            ScaleDependent = original.ScaleDependent;
            OpacityDependent = original.OpacityDependent;
            RotateDependent = original.RotateDependent;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [BusNum, ..LayerValues, X, Y, Opacity, Scale, Rotate, Mirror, Cnt_X, Cnt_Y, Exp_X, Exp_Y/*, Top, Bottom, Left, Right, GBlurValue, DBlurValue, DBlurAngle*/];
    }
}
