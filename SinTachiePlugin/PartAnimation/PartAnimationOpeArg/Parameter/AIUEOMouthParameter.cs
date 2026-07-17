using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Plugin.Voice;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Parameter
{
    internal class AIUEOMouthParameter : PartAnimationOpeArgBase
    {
        public AIUEOMouthParameter()
        {
        }

        public AIUEOMouthParameter(SharedDataStore? store) : base(store)
        {
        }

        public override PartAnimationResultA GetResult(TachieSourceDescription desc)
        {
            return PartAnimationResultA.FromText(desc.MouthShape switch
            {
                MouthShape.A => "a",
                MouthShape.E => "e",
                MouthShape.I => "i",
                MouthShape.O => "o",
                MouthShape.U => "u",
                _ => "0",
            });
        }

        public override void CopyFrom(PartAnimationOpeArgBase? origin)
        {
        }

        public override PartAnimationOpeArgBase GetClone()
        {
            return new AIUEOMouthParameter();
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
        }
    }
}
