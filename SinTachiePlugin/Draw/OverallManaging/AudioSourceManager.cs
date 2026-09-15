using SinTachiePlugin.Draw.DataAndOutput;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Draw.OverallManaging
{
    internal class AudioSourceManager : IDisposable
    {
        private readonly List<AudioDataAndOutput> _dos = [];

        public void ClearSources()
        {
            _dos.ForEach(x => x.Dispose());
            _dos.Clear();
        }

        public void ResetAsseted()
        {
            _dos.ForEach(x => x.Reset());
        }

        public float Request(string path, int trackIndex, TimeSpan time, bool loop, int fluency)
        {
            AudioDataAndOutput[] do_a0 = [.. _dos
                .Where(x => x.FilePath == path && x.TackIndex == trackIndex)];
            AudioDataAndOutput do_0;
            TimeSpan ts_0;

            if (do_a0.Length == 0)
            {
                do_0 = new(path, trackIndex, fluency);

                if (time < do_0.Duration)
                    ts_0 = time;
                else if (loop)
                    ts_0 = TimeSpan.FromTicks(time.Ticks % do_0.Duration.Ticks);
                else
                    ts_0 = TimeSpan.FromTicks(do_0.Duration.Ticks - 1);

                _dos.Add(do_0);
            }
            else
            {
                do_0 = do_a0[0];

                if (time < do_0.Duration)
                    ts_0 = time;
                else if (loop)
                    ts_0 = TimeSpan.FromTicks(time.Ticks % do_0.Duration.Ticks);
                else
                    ts_0 = TimeSpan.FromTicks(do_0.Duration.Ticks - 1);

                if (do_a0.FirstOrDefault(x => x.Time == ts_0 && x.Occupied) is AudioDataAndOutput do_1)
                    return do_1.Value;

                List<AudioDataAndOutput> do_l0 = [];
                List<TimeSpan> ts_l0 = [];

                foreach (AudioDataAndOutput do_2 in do_a0.Where(x => !x.Occupied))
                {
                    TimeSpan ts_1;

                    if (do_2.Time <= ts_0)
                    {
                        ts_1 = time;
                    }
                    else if (loop)
                    {
                        ts_1 = TimeSpan.FromTicks(time.Ticks % ts_0.Ticks);
                    }
                    else
                    {
                        continue;
                    }

                    do_l0.Add(do_2);
                    ts_l0.Add(ts_1);
                }

                if (do_l0.Count > 0)
                {
                    TimeSpan ts_2 = ts_l0[0];

                    for (int i_0 = 1; i_0 < do_l0.Count; i_0++)
                    {
                        if (ts_2 < ts_l0[i_0])
                        {
                            ts_2 = ts_l0[i_0];
                            do_0 = do_l0[i_0];
                        }
                    }
                }
                else
                {
                    TimeSpan ts_2 = do_a0[0].Time;

                    for (int i_0 = 1; i_0 < do_l0.Count; i_0++)
                    {
                        if (ts_2 < do_a0[i_0].Time)
                        {
                            ts_2 = do_a0[i_0].Time;
                            do_0 = do_a0[i_0];
                        }
                    }
                }
            }

            do_0.Update(ts_0, fluency);
            do_0.Occupied = true;
            return do_0.Value;
        }

        public void DisposeVacancySource()
        {
            AudioDataAndOutput[] do_a0 = [.. _dos.Where(x => !x.Occupied)];

            foreach (AudioDataAndOutput do_0 in do_a0)
            {
                do_0.Dispose();
                _dos.Remove(do_0);
            }
        }

        public void Dispose()
        {
            _dos.ForEach(x => x.Dispose());
        }
    }
}
