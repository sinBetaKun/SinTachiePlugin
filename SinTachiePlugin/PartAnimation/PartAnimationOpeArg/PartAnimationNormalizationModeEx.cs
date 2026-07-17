using SinTachiePlugin.Enums;
using SinTachiePlugin.Properties;

namespace SinTachiePlugin.PartAnimation.PartAnimationOpeArg
{
    internal static class PartAnimationNormalizationModeEx
    {
        public static PartAnimationResultB Normalize(this PartAnimationNormalizationMode mode, PartAnimationResultA result, int indexCount)
        {
            if (result.IsVolume)
            {
                double n0 = result.Volume;

                switch (mode)
                {
                    case PartAnimationNormalizationMode.Limit:
                        int n1;

                        if (n0 >= 1)
                            n1 = -1;
                        else if (n0 <= 0)
                            n1 = 0;
                        else
                            n1 = (int)Math.Floor(n0 * indexCount);

                        return PartAnimationResultB.FromIndex(n1);

                    case PartAnimationNormalizationMode.Shuttle:
                        double n2 = (n0 % 2 + 2) % 2;
                        int n3;

                        if (n2 >= 1)
                        {
                            n3 = 2 * indexCount - (int)Math.Floor(n2 * indexCount);

                            if (n3 == indexCount)
                                n3 = -1;
                        }
                        else
                        {
                            n3 = (int)Math.Floor(n2 * indexCount);
                        }

                        return PartAnimationResultB.FromIndex(n3);

                    case PartAnimationNormalizationMode.Loop:
                        double n4 = n0 - Math.Floor(n0);
                        int n5;

                        if (n4 == 0)
                        {
                            n5 = -1;
                        }
                        else
                        {
                            n5 = (int)Math.Floor(n0 * indexCount);

                            if (n5 == indexCount)
                                n5 = -1;
                        }

                        return PartAnimationResultB.FromIndex(n5);

                    default:
                        throw new Exception(TextResource.ImageFileNode_Error_InvalidNormalizationMode + $":({nameof(mode)})");
                }
            }
            else
            {
                return PartAnimationResultB.FromText(result.Text);
            }
        }
    }
}
