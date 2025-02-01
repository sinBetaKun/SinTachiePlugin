using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Parts
{
    public class FrameAndLength
    {
        public int Frame;
        public int Length;

        public FrameAndLength()
        {
        }
        
        public FrameAndLength(TimelineItemSourceDescription description)
        {
            Frame = description.ItemPosition.Frame;
            Length = description.ItemDuration.Frame;
        }

        public FrameAndLength(int frame, int length)
        {
            Frame = frame;
            Length = length;
        }

        public FrameAndLength(FrameAndLength origin)
        {
            Frame = origin.Frame;
            Length = origin.Length;
        }

        public FrameAndLength Update(int frame, int length)
        {
            Frame = frame;
            Length = length;
            return this;
        }

        public void CopyFrom(FrameAndLength origin)
        {
            Frame = origin.Frame;
            Length = origin.Length;
        }

        public double GetValue(Animation animation, int fps) => animation.GetValue(Frame, Length, fps);

        public IEnumerable<double> GetValues(IEnumerable<Animation> animations, int fps) => animations.Select(a => GetValue(a, fps));

        public Double2 GetDouble2(Animation a1, Animation a2, int fps) => new(GetValue(a1, fps), GetValue(a2, fps));

        public Double3 GetDouble3(Animation a1, Animation a2, Animation a3, int fps) => new(GetValue(a1, fps), GetValue(a2, fps), GetValue(a3, fps));
    }
}
