using System.ComponentModel.DataAnnotations;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Abrir;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Cerrar;
using SinTachiePlugin.Parts;
using SinTachiePlugin.Properties;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter
{
    internal class ProductParameter : PartAnimationOpeArgBase, IAbrirParameter, ICerrarParameter
    {
        [Display(Name = nameof(Text.PartAnimationOpeArg_A), ResourceType = typeof(Text))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Abrir { get; } = new Animation(100, -10000, 10000);

        [Display(Name = nameof(Text.PartAnimationOpeArg_B), ResourceType = typeof(Text))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Cerrar { get; } = new Animation(0, -10000, 10000);

        /// <summary>
        /// 制御モードが周期的往復/ループのとき、差分を指定する値を返す。
        /// </summary>
        /// <param name="fl">アイテムのフレームと長さ</param>
        /// <param name="fps">fps</param>
        /// <returns>出力</returns>
        public override double GetValue(FrameAndLength fl, int fps, double voiceVolume)
        {
            double a = fl.GetValue(Abrir, fps) / 100;
            double b = fl.GetValue(Cerrar, fps) / 100;

            return a + b;
        }

        public ProductParameter()
        {
        }

        public ProductParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(PartAnimationOpeArgBase? origin)
        {
            if (origin is IAbrirParameter abrirParam)
                Abrir.CopyFrom(abrirParam.Abrir);
            if (origin is ICerrarParameter cerrarParam)
                Cerrar.CopyFrom(cerrarParam.Cerrar);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [Abrir, Cerrar];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new AbrirSharedData(this));
            store.Save(new CerrarSharedData(this));
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
