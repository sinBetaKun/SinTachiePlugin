using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Abrir
{
    internal class AbrirSharedData
    {
        public Animation Abrir { get; } = new Animation(100, -10000, 10000);

        public AbrirSharedData()
        {
        }

        public AbrirSharedData(IAbrirParameter parameter)
        {
            Abrir.CopyFrom(parameter.Abrir);
        }

        public void CopyTo(IAbrirParameter parameter)
        {
            parameter.Abrir.CopyFrom(Abrir);
        }
    }
}
