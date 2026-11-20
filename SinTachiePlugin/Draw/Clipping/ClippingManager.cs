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

        public ClippingManager(IGraphicsDevicesAndContext devices)
        {
            _devices = devices;
        }

        public void Reset()
        {
            Clippers.ForEach(x => x.Dispose());
            Clippers = [];
        }

        public void Update(IEnumerable<PartNode> overview)
        {
            PartNode[] pn_a0 = [.. overview];
            List<ClippingTuple> ct_l0 = []; // クリッピング先指定先検証済み一時置き場A
            List<ClippingTuple> ct_l1 = []; // クリッピング先指定未検証

            foreach (PartNode pn_0 in pn_a0)
            {
                PGoPN_LayerInformation lig_0 = pn_0.Params.LayerInformationGroup;

                if (lig_0.ClippingMode == ClippingMode.ClipToParent)
                {
                    if (pn_a0.FirstOrDefault(x => x.Tag == lig_0.Parent) is PartNode pn_1)
                    {
                        ct_l1.Add(new ClippingTuple(pn_0, pn_1));
                        continue;
                    }
                }
                else if (lig_0.ClippingMode == ClippingMode.ClipWithTag)
                {
                    if (pn_a0.FirstOrDefault(x => x.Tag == lig_0.TagToClipTo) is PartNode pn_1)
                    {
                        ct_l1.Add(new ClippingTuple(pn_0, pn_1));
                        continue;
                    }
                }

                ct_l0.Add(new ClippingTuple(pn_0, null));
            }

            List<ClippingTuple> ct_l2 = []; // クリッピング先指定先検証済み一時置き場B
            List<ClippingTuple> ct_l3 = []; // クリッピング先指定検証済み最終置き場

            while (true) 
            {
                if (ct_l1.Count == 0)
                {
                    ct_l3.AddRange(ct_l0);
                    break;
                }

                foreach (ClippingTuple ct_0 in ct_l1)
                    if (ct_l0.Exists(x => x.Destination == ct_0.Mask) || ct_l2.Exists(x => x.Destination == ct_0.Mask))
                        ct_l2.Add(ct_0);

                ct_l3.AddRange(ct_l0);
                ct_l0.Clear();

                if (ct_l2.Count > 0)
                {
                    ct_l0.AddRange(ct_l2);
                    ct_l2.ForEach(x => ct_l1.Remove(x));
                    ct_l2.Clear();
                }
                else
                {
                    ct_l3.AddRange(from x in ct_l1 select new ClippingTuple(x.Destination, null));
                    break;
                }
            }

            List<PartNodeClipper> pnc_l0 = []; // 再利用するノード + 新規作成したノード
            List<PartNodeClipper> pnc_l1 = []; // Dispose するノード

            foreach (PartNodeClipper pnc_0 in Clippers)
            {
                if (pn_a0.Contains(pnc_0.Destination))
                    pnc_l0.Add(pnc_0);
                else
                    pnc_l1.Add(pnc_0);
            }

            pnc_l0.AddRange(pn_a0.Except(pnc_l0.Select(x => x.Destination)).Select(x => new PartNodeClipper(_devices, x)));
            List<PartNodeClipper> pnc_l2 = []; // 最終的なクリッパーリスト

            foreach (ClippingTuple ct_0 in ct_l3.OrderBy(x => pn_a0.IndexOf(x.Destination)))
            {
                PartNodeClipper pnc_0 = pnc_l0.First(x => x.Destination == ct_0.Destination);
                pnc_l2.Add(pnc_0);

                if (ct_0.Mask is null)
                {
                    pnc_0.Update(null);
                }
                else
                {
                    pnc_0.Update(pnc_l0.First(x => x.Destination == ct_0.Mask));
                }
            }

            pnc_l1.ForEach(x => x.Dispose());
            Clippers = [.. pnc_l2];
        }

        public void Dispose()
        {
            Clippers.ForEach(x => x.Dispose());
        }
    }
}
