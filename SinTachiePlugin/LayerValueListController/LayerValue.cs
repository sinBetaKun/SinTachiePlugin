﻿using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Parts.LayerValueListController;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;

namespace SinTachiePlugin.LayerValueListController
{
    public class LayerValue : Animatable
    {
        [Display(GroupName = "レイヤー制御", Name = "制御モード", Description = "セラールとアブリールの値の使い方を指定")]
        [EnumComboBox]
        public LayerAnimationMode AnimationMode { get => animationMode; set => Set(ref animationMode, value); }
        LayerAnimationMode animationMode = LayerAnimationMode.CerrarPlusAbrir;

        [Display(GroupName = "レイヤー制御", Name = "範囲外の値", Description = "0%~100%の範囲外になったときの値の変換方法を指定")]
        [EnumComboBox]
        public OuterLayerValueMode OuterMode { get => outerMode; set => Set(ref outerMode, value); }
        OuterLayerValueMode outerMode = OuterLayerValueMode.Limit;

        [Display(GroupName = "レイヤー制御", Name = "セラール")]
        [AnimationSlider("F1", "%", -150, 150)]
        public Animation Cerrar { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = "レイヤー制御", Name = "アブリール")]
        [AnimationSlider("F1", "%", -150, 150)]
        public Animation Abrir { get; } = new Animation(100, -10000, 10000);


        [Display(GroupName = "レイヤー制御", Name = "備考")]
        [TextEditor(PropertyEditorSize = PropertyEditorSize.FullWidth)]
        public string Comment { get => comment; set => Set(ref comment, value); }
        string comment = String.Empty;

        public LayerValue()
        {

        }

        public LayerValue(LayerValue block)
        {
            Cerrar.CopyFrom(block.Cerrar);
            Abrir.CopyFrom(block.Abrir);
        }

        public double GetValue(long len, long frame, int fps, double voiceVolume)
        {
            double cerrar = Cerrar.GetValue(frame, len, fps);
            double abrir = Abrir.GetValue(frame, len, fps);
            double num;

            switch (AnimationMode)
            {
                case LayerAnimationMode.CerrarPlusAbrir:
                    num = cerrar + abrir;
                    break;
                case LayerAnimationMode.CerrarTimesAbrir:
                    num = cerrar * abrir;
                    break;
                case LayerAnimationMode.Sin:
                    num = cerrar * Math.Sin(abrir / 100 * 2 * Math.PI);
                    break;
                case LayerAnimationMode.VoiceVolume:
                    num = cerrar + voiceVolume * (abrir - cerrar);
                    break;
                default:
                    string clsName = GetType().Name;
                    string? mthName = MethodBase.GetCurrentMethod()?.Name;
                    SinTachieDialog.ShowError("存在しないタイプのレイヤー制御モードが指定されました。", clsName, mthName);
                    throw new Exception($"[{PluginInfo.Title}]存在しないタイプのレイヤー制御モードが指定されました。\n(AnimationMode = {AnimationMode})");
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
                    string clsName = GetType().Name;
                    string? mthName = MethodBase.GetCurrentMethod()?.Name;
                    SinTachieDialog.ShowError("存在しないタイプのレイヤー制御モードが指定されました。", clsName, mthName);
                    throw new Exception($"[{PluginInfo.Title}]存在しないタイプの範囲外変換モードが指定されました。\n(OuterMode = {OuterMode})");
            }
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Cerrar, Abrir];
    }
}
