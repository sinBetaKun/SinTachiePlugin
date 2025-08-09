using System.ComponentModel.DataAnnotations;

namespace SinTachiePlugin.Enums
{
    [Obsolete("代わりに PartAnimationMode を使います。")]
    public enum LayerAnimationMode
    {
        CerrarPlusAbrir,
        CerrarTimesAbrir,
        Sin,
        VoiceVolume,
        PeriodicShuttle,
        PeriodicLoop,
    }
}
