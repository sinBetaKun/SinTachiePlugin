using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Abrir;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Cerrar;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Inithal;
using SinTachiePlugin.Parts;
using SinTachiePlugin.Properties;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter
{
    internal class VoiceVolumePlusParameter : PartAnimationOpeArgBase, IAbrirParameter, ICerrarParameter, IInitialParameter
    {
        [Display(GroupName = nameof(Text.PartAnimationOpeArg_GroupName), Name = nameof(Text.PartAnimationOpeArg_Open), ResourceType = typeof(Text))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Abrir { get; } = new Animation(100, -10000, 10000);

        [Display(GroupName = nameof(Text.PartAnimationOpeArg_GroupName), Name = nameof(Text.PartAnimationOpeArg_Close), ResourceType = typeof(Text))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Cerrar { get; } = new Animation(0, -10000, 10000);

        [Display(GroupName = nameof(Text.PartAnimationOpeArg_GroupName), Name = nameof(Text.PartAnimationOpeArg_Silent), ResourceType = typeof(Text))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Initial { get; } = new Animation(0, -10000, 10000);

        /// <summary>
        /// 制御モードが周期的往復/ループのとき、差分を指定する値を返す。
        /// </summary>
        /// <param name="fl">アイテムのフレームと長さ</param>
        /// <param name="fps">fps</param>
        /// <returns>出力</returns>
        public override double GetValue(FrameAndLength fl, int fps, double voiceVolume)
        {
            if (voiceVolume < 0)
                return fl.GetValue(Initial, fps) / 100;

            double open = fl.GetValue(Abrir, fps) / 100;
            double close = fl.GetValue(Cerrar, fps) / 100;

            return close + (open - close) * voiceVolume;
        }

        public VoiceVolumePlusParameter()
        {
        }

        public VoiceVolumePlusParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(PartAnimationOpeArgBase? origin)
        {
            if (origin is IAbrirParameter abrirParam)
                Abrir.CopyFrom(abrirParam.Abrir);
            if (origin is ICerrarParameter cerrarParam)
                Cerrar.CopyFrom(cerrarParam.Cerrar);
            if (origin is IInitialParameter initialParam)
                Initial.CopyFrom(initialParam.Initial);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Abrir, Cerrar, Initial];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new AbrirSharedData(this));
            store.Save(new CerrarSharedData(this));
            store.Save(new InitialSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<AbrirSharedData>() is AbrirSharedData abrirParameter)
                abrirParameter.CopyTo(this);
            if (store.Load<CerrarSharedData>() is CerrarSharedData cerrarParameter)
                cerrarParameter.CopyTo(this);
        }
    }
}
