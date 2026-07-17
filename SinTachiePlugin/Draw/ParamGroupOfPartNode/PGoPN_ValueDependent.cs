using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.Parameter;
using SinTachiePlugin.Part.ValueDependent.ValueDependentArg.SubArgument.Parameter;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_ValueDependent
    {
        public bool XYZ { get; set; } = true;
        public bool Opacity { get; set; } = true;
        public bool Zoom { get; set; } = true;
        public bool Rotation { get; set; } = true;
        public bool Invert { get; set; } = true;
        public bool Camera { get; set; } = true;
        public bool UnlazyEffect { get; set; } = true;

        public void Reset()
        {
            XYZ = Opacity = Zoom = Rotation = Invert = Camera = UnlazyEffect = true;
        }

        public void Update(List<PartValueAndFL> pvfls)
        {
            XYZ = Opacity = Zoom = Rotation = Invert = Camera = UnlazyEffect = true;

            foreach (PartValueAndFL pvfl in pvfls)
            {
                PartValue pv = pvfl.PartValue;

                if (pv.ControlledParameters.ValueDependentArg is HavingSourceValueDependentParameter hsvdp)
                {
                    switch (hsvdp.ModeMaster)
                    {
                        case ValueDependentModeMaster.On:
                            XYZ = Opacity = Zoom = Rotation = Invert = Camera = UnlazyEffect = true;
                            break;

                        case ValueDependentModeMaster.Off:
                            XYZ = Opacity = Zoom = Rotation = Invert = Camera = UnlazyEffect = false;
                            break;

                        case ValueDependentModeMaster.Custom:
                            if (hsvdp.SubArg is CustomModeParameter cmp)
                            {
                                if (cmp.XYZ != ValueDependentMode.DontOverride)
                                    XYZ = cmp.XYZ == ValueDependentMode.On;
                                if (cmp.Opacity  != ValueDependentMode.DontOverride)
                                    Opacity = cmp.Opacity == ValueDependentMode.On;
                                if (cmp.Zoom != ValueDependentMode.DontOverride)
                                    Zoom = cmp.Zoom == ValueDependentMode.On;
                                if (cmp.Rotation != ValueDependentMode.DontOverride)
                                    Rotation = cmp.Rotation == ValueDependentMode.On;
                                if (cmp.Invert != ValueDependentMode.DontOverride)
                                    Invert = cmp.Invert == ValueDependentMode.On;
                                if (cmp.Camera != ValueDependentMode.DontOverride)
                                    Camera = cmp.Camera == ValueDependentMode.On;
                                if (cmp.UnlazyEffect != ValueDependentMode.DontOverride)
                                    UnlazyEffect = cmp.UnlazyEffect == ValueDependentMode.DontOverride;
                            }

                            break;
                    }
                }
            }
        }

        public void CopyTo(PGoPN_ValueDependent pg)
        {
            pg.XYZ = XYZ;
            pg.Opacity = Opacity;
            pg.Zoom = Zoom;
            pg.Rotation = Rotation;
            pg.Invert = Invert;
            pg.Camera = Camera;
            pg.UnlazyEffect = UnlazyEffect;
        }

        public PartNodeComparateResult Comparate(PGoPN_ValueDependent pg)
        {
            bool valueChanged =
                XYZ != pg.XYZ ||
                Opacity != pg.Opacity ||
                Zoom != pg.Zoom ||
                Rotation != pg.Rotation ||
                Invert != pg.Invert ||
                Camera != pg.Camera ||
                UnlazyEffect != pg.UnlazyEffect;

            return new(Value: valueChanged);
        }
    }
}
