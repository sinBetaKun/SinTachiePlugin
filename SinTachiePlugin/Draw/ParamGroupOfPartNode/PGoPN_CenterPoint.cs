using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.Parameter;
using SinTachiePlugin.Part.CenterPoint.CenterPointArg.SubArgument.Parameter;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_CenterPoint
    {
        public CenterPointMode CenterMode { get; set; } = CenterPointMode.DontSet;
        public string CustomPointName { get; private set; } = string.Empty;
        public double X { get; private set; }
        public double Y { get; private set; }
        public bool KeepPlace { get; private set; } = false;

        public void Reset()
        {
            CenterMode = CenterPointMode.DontSet;
            CustomPointName = string.Empty;
            X = Y = 0;
            KeepPlace = false;
        }

        public void Update(List<PartValueAndFL> pvfls, TachieSourceDescription desc)
        {
            CenterMode = CenterPointMode.DontSet;
            CustomPointName = string.Empty;
            X = Y = 0;
            KeepPlace = false;
            int fps = desc.FPS;

            foreach (PartValueAndFL pvfl in pvfls.Reverse<PartValueAndFL>())
            {
                ControlledParametersOfPart cpp = pvfl.PartValue.ControlledParameters;

                if (cpp.CenterPointArg is HavingSourceCenterPointParameter hscpp)
                {
                    if (hscpp.CenterMode == CenterPointMode.DontSet)
                    {
                        return;
                    }

                    FrameAndLength fl = pvfl.FL;

                    if (hscpp.SubArg is OnlyCoordinateParameter ocp_0)
                    {
                        CenterMode = CenterPointMode.OnlyCoordinate;
                        X = fl.GetValue(ocp_0.X, fps);
                        Y = fl.GetValue(ocp_0.Y, fps);
                        KeepPlace = ocp_0.KeepPlace;
                        return;
                    }
                    
                    if (hscpp.SubArg is CustomCenterPointParameter ccpp_0)
                    {
                        CenterMode = CenterPointMode.CustomPointName;
                        CustomPointName = ccpp_0.CustomPointName;
                        X = fl.GetValue(ccpp_0.X, fps);
                        Y = fl.GetValue(ccpp_0.Y, fps);
                        KeepPlace = ccpp_0.KeepPlace;
                        return;
                    }
                }
            }
        }

        public void CopyTo(PGoPN_CenterPoint pg)
        {
            pg.CenterMode = CenterMode;
            pg.CustomPointName = CustomPointName;
            pg.X = X;
            pg.Y = Y;
            pg.KeepPlace = KeepPlace;
        }

        public PartNodeComparateResult Comparate(PGoPN_CenterPoint pg)
        {
            bool b_4v = 
                CenterMode != pg.CenterMode ||
                CustomPointName != pg.CustomPointName ||
                X != pg.X ||
                Y != pg.Y ||
                KeepPlace != pg.KeepPlace;

            return new(Value: b_4v);
        }
    }
}
