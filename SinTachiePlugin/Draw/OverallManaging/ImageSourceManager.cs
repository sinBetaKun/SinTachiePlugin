using PsdParser;
using SinTachiePlugin.Draw.DataAndOutput;
using SinTachiePlugin.Draw.ParamGroupOfPartNode;
using SinTachiePlugin.Enums;
using System.IO;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw.OverallManaging
{
    internal class ImageSourceManager : IDisposable
    {
        private readonly IGraphicsDevicesAndContext _devices;
        private readonly List<ImageDataAndOutput> _imageDOs = [];
        private readonly List<PsdDataAndOutput> _psdDOs = [];
        private readonly List<PsdFileQuery> _psdFileQs = [];
        private readonly List<VideoDataAndOutput> _videoDOs = [];
        private readonly List<SceneDataAndOutput> _sceneDOs = [];
        private readonly ID2D1Bitmap _empty;


        public ImageSourceManager(IGraphicsDevicesAndContext devices)
        {
            _devices = devices;
            _empty = _devices.DeviceContext.CreateEmptyBitmap();
        }

        public void ClearSources()
        {
            _imageDOs.ForEach(x => x.Dispose());
            _imageDOs.Clear();
            _psdDOs.ForEach(x => x.Dispose());
            _psdDOs.Clear();
            _psdFileQs.ForEach(x => x.Dispose());
            _psdFileQs.Clear();
            _videoDOs.ForEach(x => x.Dispose());
            _videoDOs.Clear();
            _sceneDOs.ForEach(x => x.Dispose());
            _sceneDOs.Clear();
        }

        public void UpdateSourcesAssigning(IEnumerable<PartNode> pns, TimelineItemSourceDescription desc)
        {
            _imageDOs.ForEach(x => x.Used = false);
            _psdDOs.ForEach(x => x.Used = false);
            _videoDOs.ForEach(x => x.Occupied = false);
            _sceneDOs.ForEach(x => x.Occupied = false);

            VideoDataAndOutput[] vdo_a0 = [.. _videoDOs];
            SceneDataAndOutput[] sdo_a0 = [.. _sceneDOs];
            _videoDOs.Clear();
            _sceneDOs.Clear();

            foreach (string s_4pth_0 in vdo_a0.Select(x => x.FilePath).Distinct())
                _videoDOs.AddRange(vdo_a0.Where(x => x.FilePath == s_4pth_0).OrderBy(x => x.Time));

            foreach (Guid sId_0 in sdo_a0.Select(x => x.SceneId).Distinct())
                _sceneDOs.AddRange(sdo_a0.Where(x => x.SceneId == sId_0).OrderBy(x => x.Time));

            foreach (PartNode pn_0 in pns)
                Assign(pn_0, desc);

            ImageDataAndOutput[] ido_a0 = [.. _imageDOs.Where(x => !x.Used)];
            PsdDataAndOutput[] pdo_a0 = [.. _psdDOs.Where(x => !x.Used)];
            vdo_a0 = [.. _videoDOs.Where(x => !x.Occupied)];
            sdo_a0 = [.. _sceneDOs.Where(x => !x.Occupied)];

            foreach (ImageDataAndOutput ido_0 in ido_a0)
            {
                _imageDOs.Remove(ido_0);
                ido_0.Dispose();
            }

            foreach (PsdDataAndOutput pdo_0 in pdo_a0)
            {
                _psdDOs.Remove(pdo_0);
                pdo_0.Dispose();
            }

            foreach (VideoDataAndOutput vdo_0 in vdo_a0)
            {
                _videoDOs.Remove(vdo_0);
                vdo_0.Dispose();
            }

            foreach (SceneDataAndOutput sdo_0 in sdo_a0)
            {
                _sceneDOs.Remove(sdo_0);
                sdo_0.Dispose();
            }

            string[] s_a0 = [.. _psdDOs.Select(x => x.FilePath).Distinct()];
            PsdFileQuery[] pfq_a0 = [.. _psdFileQs.Where(x => !s_a0.Contains(x.FilePath))];

            foreach (PsdFileQuery pfq_0 in pfq_a0)
            {
                _psdFileQs.Remove(pfq_0);
                pfq_0.PsdFile.Dispose();
            }
        }

        private void Assign(PartNode pn, TimelineItemSourceDescription desc)
        {
            PGoPN_Source sg_0 = pn.Params.SourceGroup;

            switch (sg_0.ThisLayerType)
            {
                case LayerType.Image:
                    if (pn.GetSourcePath() is string path && Path.Exists(path))
                    {
                        if (_imageDOs.FirstOrDefault(x => x.FilePath == path) is ImageDataAndOutput ido_0
                        && ido_0.Output is not null)
                        {
                            pn.SetSource(ido_0.Output);
                            ido_0.Used = true;
                        }
                        else
                        {
                            ImageDataAndOutput ido_1 = new(_devices, path);

                            if (ido_1.Output is not null)
                            {
                                pn.SetSource(ido_1.Output);
                                ido_1.Used = true;
                                _imageDOs.Add(ido_1);
                            }
                            else
                            {
                                ido_1.Dispose();
                                pn.SetSource(_empty);
                            }
                        }
                    }
                    else
                    {
                        pn.SetSource(_empty);
                    }

                    break;

                case LayerType.Psd:
                    if (Path.Exists(sg_0.FilePath))
                    {
                        if (_psdDOs.FirstOrDefault(x => x.FilePath == sg_0.FilePath && sg_0.EnableLayers.SequenceEqual(x.EnableLayers)) is PsdDataAndOutput pso_0
                        && pso_0.Output is not null)
                        {
                            pn.SetSource(pso_0.Output);
                        }
                        else
                        {
                            PsdDataAndOutput pdo_1;

                            if (_psdFileQs.FirstOrDefault(x => x.FilePath == sg_0.FilePath)?.PsdFile is PsdFile pf_0)
                            {
                                pdo_1 = new(sg_0.FilePath, _devices, pf_0, sg_0.EnableLayers);
                            }
                            else
                            {
                                PsdFile pf_1 = new(sg_0.FilePath);
                                _psdFileQs.Add(new(sg_0.FilePath, pf_1));
                                pdo_1 = new(sg_0.FilePath, _devices, pf_1, sg_0.EnableLayers);
                            }

                            if (pdo_1.Output is not null)
                            {
                                pn.SetSource(pdo_1.Output);
                                pdo_1.Used = true;
                                _psdDOs.Add(pdo_1);
                            }
                            else
                            {
                                pdo_1.Dispose();
                                pn.SetSource(_empty);
                            }
                        }
                    }
                    else
                    {
                        pn.SetSource(_empty);
                    }

                    break;

                case LayerType.Video:
                    if (Path.Exists(sg_0.FilePath))
                    {
                        List<VideoDataAndOutput> vdo_l0 = [.. _videoDOs.Where(x => x.FilePath == sg_0.FilePath)];

                        if (vdo_l0.Count > 0)
                        {
                            TimeSpan ts_0 = vdo_l0.First().GetValidTimeSpan(sg_0.Time, sg_0.LoopPlayback);

                            if (vdo_l0.FirstOrDefault(x => x.Occupied && x.Time == ts_0) is VideoDataAndOutput vdo_0
                                && vdo_0.Output is not null)
                            {
                                pn.SetSource(vdo_0.Output);
                            }
                            else
                            {
                                List<VideoDataAndOutput> vdo_l1 = [.. vdo_l0.Where(x => !x.Occupied)];

                                if (vdo_l1.Count > 0)
                                {
                                    List<VideoDataAndOutput> vdo_l2 = [.. vdo_l0.Where(x => x.Time <= ts_0)];
                                    VideoDataAndOutput vdo_1;

                                    if (vdo_l1.Count > 0)
                                        vdo_1 = vdo_l1.OrderBy(x => x.Time).First();
                                    else
                                        vdo_1 = vdo_l0.OrderBy(x => x.Time).Last();

                                    vdo_1.Update(ts_0);

                                    if (vdo_1.Output is not null)
                                    {
                                        pn.SetSource(vdo_1.Output);
                                        vdo_1.Occupied = true;
                                    }
                                    else
                                    {
                                        pn.SetSource(_empty);
                                        _videoDOs.Remove(vdo_1);
                                        vdo_1.Dispose();
                                    }
                                }
                                else
                                {
                                    VideoDataAndOutput vdo_1 = new(_devices, sg_0.FilePath, ts_0, false);

                                    if (vdo_1.Output is not null)
                                    {
                                        pn.SetSource(vdo_1.Output);
                                        vdo_1.Occupied = true;
                                        _videoDOs.Add(vdo_1);
                                    }
                                    else
                                    {
                                        vdo_1.Dispose();
                                        pn.SetSource(_empty);
                                    }
                                }
                            }
                        }
                        else
                        {
                            VideoDataAndOutput vdo_0 = new(_devices, sg_0.FilePath, sg_0.Time, sg_0.LoopPlayback);

                            if (vdo_0.Output is not null)
                            {
                                pn.SetSource(vdo_0.Output);
                                vdo_0.Occupied = true;
                                _videoDOs.Add(vdo_0);
                            }
                            else
                            {
                                vdo_0.Dispose();
                                pn.SetSource(_empty);
                            }
                        }
                    }
                    else
                    {
                        pn.SetSource(_empty);
                    }
                        
                    break;
                
                case LayerType.Scene:
                    if (desc.Scenes.FirstOrDefault(x => x.ID == sg_0.SceneId) is not null)
                    {
                        List<SceneDataAndOutput> l0 = [.. _sceneDOs.Where(x => x.SceneId == sg_0.SceneId)];

                        if (l0.Count > 0)
                        {
                            TimeSpan t0 = l0.First().GetValidTimeSpan(sg_0.Time, sg_0.LoopPlayback);

                            if (l0.FirstOrDefault(x => x.Occupied && x.Time == t0) is SceneDataAndOutput _1
                                && _1.Output is not null)
                            {
                                pn.SetSource(_1.Output);
                            }
                            else
                            {
                                List<SceneDataAndOutput> l1 = [.. l0.Where(x => !x.Occupied)];

                                if (l1.Count > 0)
                                {
                                    List<SceneDataAndOutput> l2 = [.. l0.Where(x => x.Time <= t0)];
                                    SceneDataAndOutput _2;

                                    if (l1.Count > 0)
                                        _2 = l1.OrderBy(x => x.Time).First();
                                    else
                                        _2 = l0.OrderBy(x => x.Time).Last();

                                    _2.Update(desc, t0);

                                    if (_2.Output is not null)
                                    {
                                        pn.SetSource(_2.Output);
                                        _2.Occupied = true;
                                    }
                                    else
                                    {
                                        _sceneDOs.Remove(_2);
                                        _2.Dispose();
                                        pn.SetSource(_empty);
                                    }
                                }
                                else
                                {
                                    SceneDataAndOutput sdo_0 = new(_devices, desc, sg_0.SceneId, t0, false);

                                    if (sdo_0.Output is not null)
                                    {
                                        pn.SetSource(sdo_0.Output);
                                        sdo_0.Occupied = true;
                                        _sceneDOs.Add(sdo_0);
                                    }
                                    else
                                    {
                                        sdo_0.Dispose();
                                        pn.SetSource(_empty);
                                    }
                                }
                            }
                        }
                        else
                        {
                            SceneDataAndOutput sdo_0 = new(_devices, desc, sg_0.SceneId, sg_0.Time, sg_0.LoopPlayback);

                            if (sdo_0.Output is not null)
                            {
                                pn.SetSource(sdo_0.Output);
                                sdo_0.Occupied = true;
                                _sceneDOs.Add(sdo_0);
                            }
                            else
                            {
                                sdo_0.Dispose();
                                pn.SetSource(_empty);
                            }
                        }
                    }
                    else
                    {
                        pn.SetSource(_empty);
                    }

                    break;

                default:
                    pn.SetSource(_empty);
                    break;
            }
        }

        public void Dispose()
        {
            ClearSources();
            _empty.Dispose();
        }
    }
}
