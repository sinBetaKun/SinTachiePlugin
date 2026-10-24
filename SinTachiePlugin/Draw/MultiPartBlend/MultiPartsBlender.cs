using SinTachiePlugin.Draw.ParamGroupOfPartNode;
using SinTachiePlugin.Enums;
using Vortice.Direct2D1;
using Vortice.Direct2D1.Effects;
using YukkuriMovieMaker.Commons;
using Blend = YukkuriMovieMaker.Project.Blend;

namespace SinTachiePlugin.Draw.MultiPartBlend
{
    internal class MultiPartsBlender : IDisposable
    {
        private readonly List<MultiPartBlendTuple> _tupples = [];
        private readonly IGraphicsDevicesAndContext _devices;
        private readonly AffineTransform2D _trans;
        private readonly ID2D1Bitmap _empty;
        private ID2D1CommandList? _cmdl = null;
        public ID2D1Image Output;

        public MultiPartsBlender(IGraphicsDevicesAndContext devices)
        {

            _devices = devices;
            _trans = new(devices.DeviceContext);
            _empty = devices.DeviceContext.CreateEmptyBitmap();
            _trans.SetInput(0, _empty, true);
            Output = _trans.Output;
        }

        public void Reset()
        {
            _tupples.Clear();
            _cmdl?.Dispose();
            _trans.SetInput(0, _empty, true);
        }

        public void Update(IEnumerable<MultiPartBlendTuple> tupples, bool orderCheck)
        {
            PartInsertTuple[] pit_a0 = [.. tupples.Select(x => new PartInsertTuple(x))];
            List<PartInsertTuple> pit_l0 = [];
            List<PartInsertTuple> pit_l1 = [];
            List<PartInsertTuple> pit_l2 = [];

            foreach (PartInsertTuple pit_0 in pit_a0)
            {
                PGoPN_LayerInformation lig_0 = pit_0.Target.Node.Params.LayerInformationGroup;

                if (lig_0.InsertDefineMode == PartInsertDefineMode.SelectTagAndPosition)
                {
                    if (pit_a0.FirstOrDefault(x => x.Target.Node.Tag == lig_0.TagToInsert) is not null)
                    {
                        pit_l0.Add(pit_0);
                        continue;
                    }
                }
                else if (lig_0.InsertDefineMode == PartInsertDefineMode.ToParentAndPosition)
                {
                    if (pit_a0.FirstOrDefault(x => x.Target.Node.Tag == lig_0.Parent) is not null)
                    {
                        pit_l0.Add(pit_0);
                        continue;
                    }
                }

                pit_l1.Add(pit_0);
            }

            List<PartInsertTuple> pit_l3 = [];

            while (pit_l0.Count > 0)
            {
                PartInsertTuple pit_0 = pit_l0.First();
                pit_l3.Clear();
                pit_l3.Add(pit_0);

                while (true)
                {
                    PGoPN_LayerInformation lig_0 = pit_0.Target.Node.Params.LayerInformationGroup;
                    pit_0 = pit_a0.First(x => x.Target.Node.Tag == lig_0.TagToInsert);

                    int i_0 = pit_l3.IndexOf(pit_0);

                    if (i_0 >= 0)
                    {
                        pit_l3.ForEach(x => pit_l0.Remove(x));
                        pit_l1.AddRange(pit_l3[i_0..]);
                        pit_l2.AddRange(pit_l3[..i_0]);
                        break;
                    }

                    if (pit_l1.Contains(pit_0) || pit_l2.Contains(pit_0))
                    {
                        pit_l3.ForEach(x => pit_l0.Remove(x));
                        pit_l2.AddRange(pit_l3);
                        break;
                    }

                    pit_l3.Add(pit_0);
                }
            }

            foreach (PartInsertTuple pit_0 in pit_l1)
            {
                pit_l3.Clear();

                foreach (PartInsertTuple pit_1 in pit_l2)
                {
                    PGoPN_LayerInformation lig_0 = pit_1.Target.Node.Params.LayerInformationGroup;

                    if (lig_0.TagToInsert == pit_0.Target.Node.Tag)
                    {
                        pit_l0.Add(pit_1);
                        pit_l3.Add(pit_1);

                        if (lig_0.InsertPosition == PartInsertPosition.Back)
                        {
                            pit_0.Back.Add(pit_1);
                        }
                        else
                        {
                            pit_0.Front.Add(pit_1);
                        }
                    }
                }

                pit_l3.ForEach(x => pit_l2.Remove(x));
            }

            while (pit_l2.Count > 0)
            {
                pit_l3.Clear();

                foreach (PartInsertTuple pit_0 in pit_l0)
                {
                    foreach (PartInsertTuple pit_1 in pit_l2)
                    {
                        PGoPN_LayerInformation lig_0 = pit_1.Target.Node.Params.LayerInformationGroup;

                        if (lig_0.TagToInsert == pit_0.Target.Node.Tag)
                        {
                            pit_l3.Add(pit_1);

                            if (lig_0.InsertPosition == PartInsertPosition.Back)
                            {
                                pit_0.Back.Add(pit_1);
                            }
                            else
                            {
                                pit_0.Front.Add(pit_1);
                            }
                        }
                    }
                }

                pit_l0.Clear();
                pit_l0.AddRange(pit_l3);
                pit_l3.ForEach(x => pit_l2.Remove(x));
            }

            pit_l1 = OrderByZAndPriority([.. pit_l1.OrderBy(x => pit_a0.IndexOf(x))]);
            
            foreach (PartInsertTuple pit_0 in pit_l1)
            {
                var _0 = OrderByZAndPriority([.. pit_0.Back.OrderBy(x => pit_a0.IndexOf(x))]);
                pit_0.Back.Clear();
                pit_0.Back.AddRange(_0);
                var _1 = OrderByZAndPriority([.. pit_0.Front.OrderBy(x => -pit_a0.IndexOf(x))]);
                pit_0.Front.Clear();
                pit_0.Front.AddRange(_1);
            }

            List<MultiPartBlendTuple> mpbt_l0 = [];

            foreach (PartInsertTuple pit_0 in pit_l1)
                mpbt_l0.AddRange(pit_0.GetList());

            List<MultiPartBlendTuple> mpbt_l1 = [.. mpbt_l0.Where(x => !x.Node.Params.Hide)];

            bool b_0 = false;

            if (orderCheck && mpbt_l1.Count == _tupples.Count)
            {
                for (int i_0 = 0; i_0 < mpbt_l1.Count; i_0++)
                {
                    if (mpbt_l1[i_0].Node != _tupples[i_0].Node)
                    {
                        b_0 = true;
                        break;
                    }
                }
            }
            else
            {
                b_0 = true;
            }

            if (b_0)
            {
                _tupples.Clear();
                _tupples.AddRange(mpbt_l1);
                _cmdl?.Dispose();
                
                if (mpbt_l1.Count == 0)
                {
                    _trans.SetInput(0, _empty, true);
                }
                else
                {
                    ID2D1DeviceContext6 dc = _devices.DeviceContext;
                    _cmdl = dc.CreateCommandList();
                    dc.Target = _cmdl;
                    dc.BeginDraw();
                    dc.Clear(null);

                    foreach (MultiPartBlendTuple mpbt_0 in mpbt_l1)
                        DrawOrBlend(dc, mpbt_0);

                    dc.EndDraw();
                    _cmdl.Close();//CommandListはEndDraw()の後に必ずClose()を呼んで閉じる必要がある
                    _trans.SetInput(0, _cmdl, true);
                }
            }
        }

        static private void DrawOrBlend(ID2D1DeviceContext6 dc, MultiPartBlendTuple mpbt)
        {
            ID2D1Image output = mpbt.Output;
            Blend blend = mpbt.Node.Params.DrawingGroup.Blend;

            switch (blend)
            {
                case Blend.Normal:
                    dc.DrawImage(output, compositeMode: CompositeMode.SourceOver);
                    break;

                case Blend.Add:
                    dc.DrawImage(output, compositeMode: CompositeMode.Plus);
                    break;

                case Blend.DestinationOver:
                    dc.DrawImage(output, compositeMode: CompositeMode.DestinationOver);
                    break;

                case Blend.DestinationOut:
                    dc.DrawImage(output, compositeMode: CompositeMode.DestinationOut);
                    break;

                case Blend.SourceAtop:
                    dc.DrawImage(output, compositeMode: CompositeMode.SourceAtop);
                    break;

                case Blend.XOR:
                    dc.DrawImage(output, compositeMode: CompositeMode.Xor);
                    break;

                case Blend.MaskInvert:
                    dc.DrawImage(output, compositeMode: CompositeMode.MaskInverseErt);
                    break;

                case Blend.Multiply:
                    dc.BlendImage(output, BlendMode.Multiply, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Screen:
                    dc.BlendImage(output, BlendMode.Screen, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Darker:
                    dc.BlendImage(output, BlendMode.Darken, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Lighter:
                    dc.BlendImage(output, BlendMode.Lighten, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Dissolve:
                    dc.BlendImage(output, BlendMode.Dissolve, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.ColorBurn:
                    dc.BlendImage(output, BlendMode.ColorBurn, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.LinearBurn:
                    dc.BlendImage(output, BlendMode.LinearBurn, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.DarkerColor:
                    dc.BlendImage(output, BlendMode.DarkerColor, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.LighterColor:
                    dc.BlendImage(output, BlendMode.LighterColor, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.ColorDodge:
                    dc.BlendImage(output, BlendMode.ColorDodge, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.LinearDodge:
                    dc.BlendImage(output, BlendMode.LinearDodge, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Overlay:
                    dc.BlendImage(output, BlendMode.Overlay, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.SoftLight:
                    dc.BlendImage(output, BlendMode.SoftLight, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.HardLight:
                    dc.BlendImage(output, BlendMode.HardLight, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.VividLight:
                    dc.BlendImage(output, BlendMode.VividLight, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.LinearLight:
                    dc.BlendImage(output, BlendMode.LinearLight, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.PinLight:
                    dc.BlendImage(output, BlendMode.PinLight, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.HardMix:
                    dc.BlendImage(output, BlendMode.HardMix, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Difference:
                    dc.BlendImage(output, BlendMode.Difference, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Exclusion:
                    dc.BlendImage(output, BlendMode.Exclusion, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Hue:
                    dc.BlendImage(output, BlendMode.Hue, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Saturation:
                    dc.BlendImage(output, BlendMode.Saturation, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Color:
                    dc.BlendImage(output, BlendMode.Color, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Luminosity:
                    dc.BlendImage(output, BlendMode.Luminosity, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Subtract:
                    dc.BlendImage(output, BlendMode.Subtract, null, null, InterpolationMode.MultiSampleLinear);
                    break;

                case Blend.Division:
                    dc.BlendImage(output, BlendMode.Division, null, null, InterpolationMode.MultiSampleLinear);
                    break;
            }
        }

        private static List<PartInsertTuple> OrderByZAndPriority(List<PartInsertTuple> ts)
        {
            PartInsertTuple[] pit_a0 = [.. ts.Where(x => x.Target.Node.Params.DrawingGroup.ZSort == ZSortMode2.IgnorePriority)];
            int i_0;

            for (i_0 = 0; i_0 < pit_a0.Length; i_0++)
                if (pit_a0[i_0].Target.Node.Z >= 0.0f)
                    break;

            List<PartInsertTuple> pit_l0 = [.. pit_a0[..i_0]];

            PartInsertTuple[] pit_a3 = [.. ts.Where(x => !pit_a0.Contains(x))];
            double[] d_a0 = [.. pit_a3.Select(x => x.Target.Node.Params.DrawingGroup.Priority).Distinct().OrderBy(x => x)];

            foreach (double d_0 in d_a0)
                pit_l0.AddRange(pit_a3
                    .Where(x => x.Target.Node.Params.DrawingGroup.Priority == d_0)
                    .OrderBy(x => x.Target.Node.Params.DrawingGroup.ZSort == ZSortMode2.IgnoreZ ? 0 : x.Target.Node.Z));

            pit_l0.AddRange(pit_a0[i_0..]);

            return pit_l0;
        }

        public void Dispose()
        {
            _trans.SetInput(0, null, true);
            Output.Dispose();
            _trans.Dispose();
            _empty.Dispose();
            _cmdl?.Dispose();
        }
    }
}
