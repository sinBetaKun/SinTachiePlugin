using SinTachiePlugin.Draw.ParamGroupOfPartNode;
using SinTachiePlugin.Enums;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.Draw.Clipping
{
    internal class ClippingManager : IDisposable
    {
        public ImmutableList<PartNodeClipper> Clippers { get; private set; } = [];

        private readonly IGraphicsDevicesAndContext _devices;

        public ClippingManager(IGraphicsDevicesAndContext d)
        {
            _devices = d;
        }

        public void Reset()
        {
            Clippers.ForEach(x => x.Dispose());
            Clippers = [];
        }

        public void Update(IEnumerable<PartNode> pns)
        {
            PartNode[] overview = [.. pns];
            List<ClippingTupple> ct_l0 = [];
            List<PartNode> pn_l0 = [];

            foreach (PartNode pn_0 in overview)
            {
                PGoPN_LayerInformation lig_0 = pn_0.Params.LayerInformationGroup;

                if (lig_0.ClippingMode == ClippingMode.ClipToParent)
                {
                    if (overview.FirstOrDefault(x => x.Tag == pn_0.Params.LayerInformationGroup.Parent) is not null)
                    {
                        pn_l0.Add(pn_0);
                    }
                }
                else if (lig_0.ClippingMode == ClippingMode.ClipWithTag)
                {
                    if (overview.FirstOrDefault(x => x.Tag == lig_0.TagToInsert) is not null)
                    {
                        pn_l0.Add(pn_0);
                    }
                }

                ct_l0.Add(new(pn_0, null));
            }

            List<PartNode> pn_l1 = [];

            while (pn_l0.Count > 0)
            {
                PartNode pn_0 = pn_l0.First();
                pn_l1.Clear();
                pn_l1.Add(pn_0);

                while (true)
                {
                    PGoPN_LayerInformation lig_0 = pn_0.Params.LayerInformationGroup;

                    if (lig_0.ClippingMode == ClippingMode.ClipToParent)
                    {
                        pn_0 = overview.First(x => x.Tag == pn_0.Params.LayerInformationGroup.Parent);
                    }
                    else
                    {
                        pn_0 = overview.First(x => x.Tag == lig_0.TagToInsert);
                    }

                    int i_0 = pn_l1.IndexOf(pn_0);

                    if (i_0 >= 0)
                    {
                        pn_l1.ForEach(x => pn_l0.Remove(x));
                        ct_l0.AddRange(pn_l1[i_0..].Select(x => new ClippingTupple(x, null)));
                        pn_l1.RemoveRange(i_0, pn_l1.Count - i_0);
                        PartNode pn_1 = pn_0;

                        while (pn_l1.Count > 0)
                        {
                            pn_0 = pn_l1.Last();
                            ct_l0.Add(new(pn_0, pn_1));
                            pn_1 = pn_0;
                            pn_l1.Remove(pn_0);
                        }

                        break;
                    }

                    if (ct_l0.FirstOrDefault(x => x.Destination == pn_0) is ClippingTupple ct_2)
                    {
                        PartNode pn_1 = ct_2.Destination;

                        while (pn_l1.Count > 0)
                        {
                            pn_0 = pn_l1.Last();
                            ct_l0.Add(new(pn_0, pn_1));
                            pn_1 = pn_0;
                            pn_l1.Remove(pn_0);
                        }

                        break;
                    }

                    pn_l1.Add(pn_0);
                }
            }

            List<PartNodeClipper> pnc_l0 = [.. Clippers];
            List<PartNodeClipper> pnc_l1 = [];

            foreach (ClippingTupple ct_0 in ct_l0)
            {
                PartNodeClipper pnc_0;

                if (pnc_l0.FirstOrDefault(x => x.Destination == ct_0.Destination) is PartNodeClipper pnc_1)
                {
                    pnc_l0.Remove(pnc_1);
                    pnc_0 = pnc_1;
                }
                else
                {
                    pnc_0 = new(_devices, ct_0.Destination);
                }

                pnc_l1.Add(pnc_0);

                if (ct_0.Mask is null)
                {
                    pnc_0.Update(null);
                }
                else
                {
                    pnc_0.Update(pnc_l1.First(x => x.Destination == ct_0.Mask));
                }
            }

            pnc_l0.ForEach(x => x.Dispose());
        }

        public void Dispose()
        {
            Clippers.ForEach(x => x.Dispose());
        }
    }
}
