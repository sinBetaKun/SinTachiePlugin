using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Abrir;
using SinTachiePlugin.Parts;
using SinTachiePlugin.Properties;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter
{
    internal class SimpleParameter : PartAnimationOpeArgBase, IAbrirParameter
    {
        [Display(Name = nameof(Text.PartAnimationOpeArg_Output), ResourceType = typeof(Text))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Abrir { get; } = new Animation(100, -10000, 10000);

        /// <summary>
        /// 制御モードが周期的往復/ループのとき、差分を指定する値を返す。
        /// </summary>
        /// <param name="fl">アイテムのフレームと長さ</param>
        /// <param name="fps">fps</param>
        /// <returns>出力</returns>
        public override double GetValue(FrameAndLength fl, int fps, double voiceVolume)
        {
            return fl.GetValue(Abrir, fps) / 100;
        }

        public SimpleParameter()
        {
        }

        public SimpleParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(PartAnimationOpeArgBase? origin)
        {
            if (origin is IAbrirParameter abrirParam)
                Abrir.CopyFrom(abrirParam.Abrir);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Abrir];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new AbrirSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<AbrirSharedData>() is AbrirSharedData abrirParameter)
                abrirParameter.CopyTo(this);
        }
    }
}
