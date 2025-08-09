using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg.Argment.Cerrar
{
    internal class CerrarSharedData
    {
        public Animation Cerrar { get; } = new Animation(0, -10000, 10000);

        public CerrarSharedData()
        {
        }

        public CerrarSharedData(ICerrarParameter parameter)
        {
            Cerrar.CopyFrom(parameter.Cerrar);
        }

        public void CopyTo(ICerrarParameter parameter)
        {
            parameter.Cerrar.CopyFrom(Cerrar);
        }
    }
}
