using PsdParser;
using SinTachiePlugin.Draw.DataAndOutput;
using SinTachiePlugin.Draw.ParamGroupOfPartNode;
using System.IO;
using Vortice.Direct2D1;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw
{
    internal class SourceManager : IDisposable
    {
        private readonly IGraphicsDevicesAndContext _devices;
        private readonly List<ImageDataAndOutput> _imageDOs = [];
        private readonly List<PsdDataAndOutput> _psdDOs = [];
        private readonly List<PsdFileQuery> _psdFileQs = [];
        private readonly List<VideoDataAndOutput> _videoDOs = [];
        private readonly List<SceneDataAndOutput> _sceneDOs = [];
        private readonly ID2D1Bitmap _empty;


        public SourceManager(IGraphicsDevicesAndContext devices)
        {
            _devices = devices;
            _empty = _devices.DeviceContext.CreateEmptyBitmap();
        }

        public void UpdateSourcesAssigning(IEnumerable<PartNode> pns, TimelineItemSourceDescription desc)
        {
            _imageDOs.ForEach(x => x.Used = false);
            _psdDOs.ForEach(x => x.Used = false);
            _videoDOs.ForEach(x => x.Occupied = false);
            _sceneDOs.ForEach(x => x.Occupied = false);

            VideoDataAndOutput[] a0 = [.. _videoDOs];
            SceneDataAndOutput[] a1 = [.. _sceneDOs];
            _videoDOs.Clear();
            _sceneDOs.Clear();

            foreach (string path in a0.Select(x => x.FilePath).Distinct())
                _videoDOs.AddRange(a0.Where(x => x.FilePath == path).OrderBy(x => x.Time));

            foreach (Guid scene in a1.Select(x => x.SceneId).Distinct())
                _sceneDOs.AddRange(a1.Where(x => x.SceneId == scene).OrderBy(x => x.Time));

            foreach (PartNode pn in pns)
                Assign(pn, desc);

            ImageDataAndOutput[] a2 = [.. _imageDOs.Where(x => !x.Used)];
            PsdDataAndOutput[] a3 = [.. _psdDOs.Where(x => !x.Used)];
            a0 = [.. _videoDOs.Where(x => !x.Occupied)];
            a1 = [.. _sceneDOs.Where(x => !x.Occupied)];

            foreach (ImageDataAndOutput _0 in a2)
            {
                _imageDOs.Remove(_0);
                _0.Dispose();
            }

            foreach (PsdDataAndOutput _0 in a3)
            {
                _psdDOs.Remove(_0);
                _0.Dispose();
            }

            foreach (VideoDataAndOutput _0 in a0)
            {
                _videoDOs.Remove(_0);
                _0.Dispose();
            }

            foreach (SceneDataAndOutput _0 in a1)
            {
                _sceneDOs.Remove(_0);
                _0.Dispose();
            }

            string[] a4 = [.. _psdDOs.Select(x => x.FilePath).Distinct()];
            PsdFileQuery[] a5 = [.. _psdFileQs.Where(x => !a4.Contains(x.FilePath))];

            foreach (PsdFileQuery _0 in a5)
            {
                _psdFileQs.Remove(_0);
                _0.PsdFile.Dispose();
            }
        }

        private void Assign(PartNode pn, TimelineItemSourceDescription desc)
        {
            PGoPN_Source _0 = pn.Params.SourceGroup;

            switch (_0.ThisLayerType)
            {
                case Enums.LayerType.Image:
                    if (pn.Ifnm is not null && Path.Exists(pn.Ifnm.Value))
                    {
                        if (_imageDOs.FirstOrDefault(x => x.FilePath == pn.Ifnm.Value) is ImageDataAndOutput _1
                        && _1.Output is not null)
                        {
                            pn.SetSource(_1.Output);
                            _1.Used = true;
                        }
                        else
                        {
                            ImageDataAndOutput _2 = new(_devices, pn.Ifnm.Value);

                            if (_2.Output is not null)
                            {
                                pn.SetSource(_2.Output);
                                _imageDOs.Add(_2);
                            }
                            else
                            {
                                _2.Dispose();
                                pn.SetSource(_empty);
                            }
                        }
                    }
                    else
                    {
                        pn.SetSource(_empty);
                    }

                    break;

                case Enums.LayerType.Psd:
                    if (Path.Exists(_0.FilePath))
                    {
                        if (_psdDOs.FirstOrDefault(x => x.FilePath == _0.FilePath && _0.EnableLayers.SequenceEqual(x.EnableLayers)) is PsdDataAndOutput _1
                        && _1.Output is not null)
                        {
                            pn.SetSource(_1.Output);
                        }
                        else
                        {
                            PsdDataAndOutput _3;

                            if (_psdFileQs.FirstOrDefault(x => x.FilePath == _0.FilePath)?.PsdFile is PsdFile _2)
                            {
                                _3 = new(_0.FilePath, _devices, _2, _0.EnableLayers);
                            }
                            else
                            {
                                PsdFile _4 = new(_0.FilePath);
                                _psdFileQs.Add(new(_0.FilePath, _4));
                                _3 = new(_0.FilePath, _devices, _4, _0.EnableLayers);
                            }

                            if (_3.Output is not null)
                            {
                                pn.SetSource(_3.Output);
                                _3.Used = true;
                                _psdDOs.Add(_3);
                            }
                            else
                            {
                                _3.Dispose();
                                pn.SetSource(_empty);
                            }
                        }
                    }
                    else
                    {
                        pn.SetSource(_empty);
                    }

                    break;

                case Enums.LayerType.Video:
                    if (Path.Exists(_0.FilePath))
                    {
                        List<VideoDataAndOutput> l0 = [.. _videoDOs.Where(x => x.FilePath == _0.FilePath)];

                        if (l0.Count > 0)
                        {
                            TimeSpan t0 = l0.First().GetValidTimeSpan(_0.Time, _0.LoopPlayback);

                            if (l0.FirstOrDefault(x => x.Occupied && x.Time == t0) is VideoDataAndOutput _1
                                && _1.Output is not null)
                            {
                                pn.SetSource(_1.Output);
                            }
                            else
                            {
                                List<VideoDataAndOutput> l1 = [.. l0.Where(x => !x.Occupied)];

                                if (l1.Count > 0)
                                {
                                    List<VideoDataAndOutput> l2 = [.. l0.Where(x => x.Time <= t0)];
                                    VideoDataAndOutput _2;

                                    if (l1.Count > 0)
                                        _2 = l1.OrderBy(x => x.Time).First();
                                    else
                                        _2 = l0.OrderBy(x => x.Time).Last();

                                    _2.Update(t0);

                                    if (_2.Output is not null)
                                    {
                                        pn.SetSource(_2.Output);
                                        _2.Occupied = true;
                                    }
                                    else
                                    {
                                        pn.SetSource(_empty);
                                        _videoDOs.Remove(_2);
                                        _2.Dispose();
                                    }
                                }
                                else
                                {
                                    VideoDataAndOutput _2 = new(_devices, _0.FilePath, t0, false);

                                    if (_2.Output is not null)
                                    {
                                        pn.SetSource(_2.Output);
                                        _2.Occupied = true;
                                        _videoDOs.Add(_2);
                                    }
                                    else
                                    {
                                        pn.SetSource(_empty);
                                        _2.Dispose();
                                    }
                                }
                            }
                        }
                        else
                        {
                            VideoDataAndOutput _1 = new(_devices, _0.FilePath, _0.Time, _0.LoopPlayback);

                            if (_1.Output is not null)
                            {
                                pn.SetSource(_1.Output);
                                _1.Occupied = true;
                                _videoDOs.Add(_1);
                            }
                            else
                            {
                                pn.SetSource(_empty);
                                _1.Dispose();
                            }
                        }
                    }
                    else
                    {
                        pn.SetSource(_empty);
                    }
                        
                    break;
                
                case Enums.LayerType.Scene:
                    if (desc.Scenes.FirstOrDefault(x => x.ID == _0.SceneId) is not null)
                    {
                        List<SceneDataAndOutput> l0 = [.. _sceneDOs.Where(x => x.SceneId == _0.SceneId)];

                        if (l0.Count > 0)
                        {
                            TimeSpan t0 = l0.First().GetValidTimeSpan(_0.Time, _0.LoopPlayback);

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
                                        pn.SetSource(_empty);
                                        _sceneDOs.Remove(_2);
                                        _2.Dispose();
                                    }
                                }
                                else
                                {
                                    SceneDataAndOutput _2 = new(_devices, desc, _0.SceneId, t0, false);

                                    if (_2.Output is not null)
                                    {
                                        pn.SetSource(_2.Output);
                                        _2.Occupied = true;
                                        _sceneDOs.Add(_2);
                                    }
                                    else
                                    {
                                        pn.SetSource(_empty);
                                        _2.Dispose();
                                    }
                                }
                            }
                        }
                        else
                        {
                            SceneDataAndOutput _1 = new(_devices, desc, _0.SceneId, _0.Time, _0.LoopPlayback);

                            if (_1.Output is not null)
                            {
                                pn.SetSource(_1.Output);
                                _1.Occupied = true;
                                _sceneDOs.Add(_1);
                            }
                            else
                            {
                                pn.SetSource(_empty);
                                _1.Dispose();
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
            foreach (ImageDataAndOutput image in _imageDOs)
                image.Dispose();

            foreach (PsdDataAndOutput psd in _psdDOs)
                psd.Dispose();

            foreach (PsdFile file in _psdFileQs.Select(x => x.PsdFile))
                file.Dispose();

            foreach (VideoDataAndOutput video in _videoDOs)
                video.Dispose();

            foreach (SceneDataAndOutput scene in _sceneDOs)
                scene.Dispose();
        }
    }
}
