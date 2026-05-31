using SinTachiePlugin.Draw;
using SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argument.Abrir;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter
{
    internal class SimpleParameter : PartAnimationOpeArgBase, IAbrirParameter
    {
        [Display(Name = nameof(TextResource.PartAnimationOpeArg_Output), ResourceType = typeof(TextResource))]
        [AnimationSlider("F1", "%", -100, 100)]
        public Animation Abrir { get; } = new Animation(100, -10000, 10000);

        /// <summary>
        /// 制御モードが周期的往復/ループのとき、差分を指定する値を返す。
        /// </summary>
        /// <returns>出力</returns>
        public override PartAnimationResult GetResult(TachieSourceDescription desc)
        {
            FrameAndLength fl = new(desc);
            int fps = desc.FPS;
            return PartAnimationResult.FromVolume(fl.GetValue(Abrir, fps) / 100);
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

        public override PartAnimationOpeArgBase GetClone()
        {
            SimpleParameter clone = new();
            clone.CopyFrom(this);
            return clone;
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
