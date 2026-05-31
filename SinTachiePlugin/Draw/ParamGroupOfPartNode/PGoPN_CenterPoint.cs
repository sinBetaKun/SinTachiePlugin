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
            CustomPointName = string.Empty;
            X = Y = 0;
            KeepPlace = false;

            foreach (PartValueAndFL pvfl in pvfls.Reverse<PartValueAndFL>())
            {
                ControlledParametersOfPart cpp = pvfl.PartValue.ControlledParameters;
                FrameAndLength fl = pvfl.FL;
                int fps = desc.FPS;

                if (cpp.CenterPointArg is HavingSourceCenterPointParameter hscpp)
                {
                    switch (hscpp.CenterMode)
                    {
                        case CenterPointMode.DontSet:
                            return;

                        case CenterPointMode.OnlyCoordinate:
                            OnlyCoordinateParameter ocp = (OnlyCoordinateParameter)hscpp.SubArg;
                            X = fl.GetValue(ocp.X, fps);
                            Y = fl.GetValue(ocp.Y, fps);
                            KeepPlace = ocp.KeepPlace;
                            return;

                        case CenterPointMode.CustomPointName:
                            CustomCenterPointParameter ccpp = (CustomCenterPointParameter)hscpp.SubArg;
                            CustomPointName = ccpp.CustomPointName;
                            X = fl.GetValue(ccpp.X, fps);
                            Y = fl.GetValue(ccpp.Y, fps);
                            KeepPlace = ccpp.KeepPlace;
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
        }

        public PartNodeComparateResult Comparate(PGoPN_CenterPoint pg)
        {
            bool valueChanged = 
                CenterMode != pg.CenterMode ||
                CustomPointName != pg.CustomPointName ||
                X != pg.X ||
                Y != pg.Y;

            return new(Value: valueChanged);
        }
    }
}
