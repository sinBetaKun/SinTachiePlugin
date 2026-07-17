using SinTachiePlugin.Enums;
using SinTachiePlugin.Part;
using SinTachiePlugin.Part.Drawing.DrawingArg.Parameter;
using SinTachiePlugin.Part.Drawing.DrawingArg.SubArgument.Parameter;
using YukkuriMovieMaker.Player.Video;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Draw.ParamGroupOfPartNode
{
    internal class PGoPN_Drawing
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }
        public double Opacity { get; private set; }
        public double Zoom_X { get; private set; }
        public double Zoom_Y { get; private set; }
        public double Rotate { get; private set; }
        public bool Invert { get; private set; }
        public Blend Blend { get; private set; }
        public ZSortMode2 ZSort { get; private set; } = ZSortMode2.BasedOnPriority;
        public double Priority { get; private set; }

        public void Reset()
        {
            X = Y = Z = Rotate = Priority = 0;
            Opacity = Zoom_X = Zoom_Y = 1;
            Invert = false;
            Blend = Blend.Normal;
            ZSort = ZSortMode2.BasedOnPriority;
        }

        public void Update(List<PartValueAndFL> pvfls, TachieSourceDescription desc)
        {
            X = Y = Z = Rotate = Priority = 0;
            Opacity = Zoom_X = Zoom_Y = 1;
            Invert = false;
            Blend = Blend.Normal;
            ZSort = ZSortMode2.BasedOnPriority;
            double zoom = 1;

            foreach (PartValueAndFL pvfl in pvfls)
            {
                ControlledParametersOfPart cpp = pvfl.PartValue.ControlledParameters;
                FrameAndLength fl = pvfl.FL;
                int fps = desc.FPS;

                if (cpp.DrawingArg is HavingSourceDrawingParameter hsdp)
                {
                    if (hsdp.SubArg is MasterModeParameter mmp)
                    {
                        switch (hsdp.ModeMaster)
                        {
                            case DrawingValueModeMaster.Override:
                                X = fl.GetValue(mmp.X, fps);
                                Y = fl.GetValue(mmp.Y, fps);
                                Z = fl.GetValue(mmp.Z, fps);
                                Opacity = fl.GetValue(mmp.Opacity, fps) / 100;
                                zoom = fl.GetValue(mmp.Zoom, fps) / 100;
                                Rotate = fl.GetValue(mmp.Rotation, fps);
                                Invert = fl.GetValue(mmp.Invert, fps) < 0.5;
                                Blend = mmp.Blend;
                                ZSort = mmp.ZSort;
                                Zoom_X = fl.GetValue(mmp.Zoom_X, fps) / 100;
                                Zoom_Y = fl.GetValue(mmp.Zoom_Y, fps) / 100;
                                Priority = fl.GetValue(mmp.Priority, fps);
                                break;

                            case DrawingValueModeMaster.Compose:
                                X += fl.GetValue(mmp.X, fps);
                                Y += fl.GetValue(mmp.Y, fps);
                                Z += fl.GetValue(mmp.Z, fps);
                                Opacity *= fl.GetValue(mmp.Opacity, fps) / 100;
                                zoom *= fl.GetValue(mmp.Zoom, fps) / 100;
                                Rotate += fl.GetValue(mmp.Rotation, fps);
                                Invert ^= fl.GetValue(mmp.Invert, fps) < 0.5;
                                Zoom_X *= fl.GetValue(mmp.Zoom_X, fps) / 100;
                                Zoom_Y *= fl.GetValue(mmp.Zoom_Y, fps) / 100;
                                Priority += fl.GetValue(mmp.Priority, fps);
                                break;
                        }
                    }
                    else if (hsdp.SubArg is CustomModeParameter cmp)
                    {
                        if (cmp.XYZMode == DrawingValueMode.Override)
                        {
                            X = fl.GetValue(cmp.X, fps);
                            Y = fl.GetValue(cmp.Y, fps);
                            Z = fl.GetValue(cmp.Z, fps);
                        }
                        else
                        {
                            X += fl.GetValue(cmp.X, fps);
                            Y += fl.GetValue(cmp.Y, fps);
                            Z += fl.GetValue(cmp.Z, fps);
                        }

                        if (cmp.OpacityMode == DrawingValueMode.Override)
                        {
                            Opacity = fl.GetValue(cmp.Opacity, fps) / 100;
                        }
                        else
                        {
                            Opacity *= fl.GetValue(cmp.Opacity, fps) / 100;
                        }

                        if (cmp.ZoomMode == DrawingValueMode.Override)
                        {
                            zoom = fl.GetValue(cmp.Zoom, fps) / 100;
                            Zoom_X = fl.GetValue(cmp.Zoom_X, fps) / 100;
                            Zoom_Y = fl.GetValue(cmp.Zoom_Y, fps) / 100;
                        }
                        else
                        {
                            zoom *= fl.GetValue(cmp.Zoom, fps) / 100;
                            Zoom_X *= fl.GetValue(cmp.Zoom_X, fps) / 100;
                            Zoom_Y *= fl.GetValue(cmp.Zoom_Y, fps) / 100;
                        }

                        if (cmp.RotationMode == DrawingValueMode.Override)
                        {
                            Rotate = fl.GetValue(cmp.Rotation, fps);
                        }
                        else
                        {
                            Rotate += fl.GetValue(cmp.Rotation, fps);
                        }

                        if (cmp.InverseMode == DrawingValueMode.Override)
                        {
                            Invert = fl.GetValue(cmp.Invert, fps) < 0.5;
                        }
                        else
                        {
                            Invert ^= fl.GetValue(cmp.Invert, fps) < 0.5;
                        }

                        if (cmp.BlendMode == DrawingValueMode.Override)
                        {
                            Blend = cmp.Blend;
                        }

                        if (cmp.ZSortMode == DrawingValueMode.Override)
                        {
                            ZSort = cmp.ZSort;
                        }

                        if (cmp.PriorityMode == DrawingValueMode.Override)
                        {
                            Priority = fl.GetValue(cmp.Priority, fps);
                        }
                        else
                        {
                            Priority += fl.GetValue(cmp.Priority, fps);
                        }
                    }
                }
            }
        }

        public void CopyTo(PGoPN_Drawing pg)
        {
            pg.X = X;
            pg.Y = Y;
            pg.Z = Z;
            pg.Opacity = Opacity;
            pg.Zoom_X = Zoom_X;
            pg.Zoom_Y = Zoom_Y;
            pg.Rotate = Rotate;
            pg.Invert = Invert;
            pg.Blend = Blend;
            pg.ZSort = ZSort;
            pg.Priority = Priority;
        }

        public PartNodeComparateResult Comparate(PGoPN_Drawing pg)
        {
            bool valueChanged =
                pg.X != X ||
                pg.Y != Y ||
                pg.Z != Z ||
                pg.Opacity != Opacity ||
                pg.Zoom_X != Zoom_X ||
                pg.Zoom_Y != Zoom_Y ||
                pg.Rotate != Rotate ||
                pg.Invert != Invert ||
                pg.Blend != Blend ||
                pg.ZSort != ZSort;

            bool clChanged = pg.Priority != Priority;

            return new(CommandList: clChanged, Value: valueChanged);
        }
    }
}
