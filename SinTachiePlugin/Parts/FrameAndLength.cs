using Vortice.Mathematics;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Player.Video;

namespace SinTachiePlugin.Parts
{
    /// <summary>
    /// アイテム上の再生位置と、アイテムの長さをセットで扱うためのクラス。
    /// </summary>
    public class FrameAndLength
    {
        /// <summary>
        /// アイテム上の再生位置
        /// </summary>
        public int Frame;

        /// <summary>
        /// アイテムの長さ
        /// </summary>
        public int Length;

        /// <summary>
        /// ここで Length を 0 にしてしまうと、Animation の GetValue メソッドがバグる。
        /// </summary>
        public FrameAndLength()
        {
            Frame = 0;
            Length = 1;
        }

        /// <summary>
        /// TimelineItemSourceDescription またはその子クラスから、現在の再生位置と長さを取得する。
        /// </summary>
        /// <param name="description">立ち絵プラグインでは TachieSourceDescription<br/>図形プラグインでは TimelineItemSourceDescription</param>
        public FrameAndLength(TimelineItemSourceDescription description)
        {
            Frame = description.ItemPosition.Frame;
            Length = description.ItemDuration.Frame;
        }

        /// <summary>
        /// Frame と Length の両方を初期化する
        /// </summary>
        /// <param name="frame">アイテム上の再生位置</param>
        /// <param name="length">アイテムの長さ</param>
        public FrameAndLength(int frame, int length)
        {
            Frame = frame;
            Length = length;
        }

        /// <summary>
        /// 渡された FrameAndLength のフィールド変数の値で初期化する。
        /// </summary>
        /// <param name="origin">コピー元</param>
        public FrameAndLength(FrameAndLength origin)
        {
            Frame = origin.Frame;
            Length = origin.Length;
        }

        /// <summary>
        /// 渡された FrameAndLength と、フィールド変数の値をそろえる。
        /// </summary>
        /// <param name="origin">コピー元</param>
        public void CopyFrom(FrameAndLength origin)
        {
            Frame = origin.Frame;
            Length = origin.Length;
        }

        /// <summary>
        /// Animation の GetValue メソッドから値を取得する。
        /// </summary>
        /// <param name="animation">GetValue メソッドを使う Animation</param>
        /// <param name="fps">FPS</param>
        /// <returns>Animation の GetValue メソッドの戻り値</returns>
        public double GetValue(Animation animation, int fps) => animation.GetValue(Frame, Length, fps);

        public IEnumerable<double> GetValues(IEnumerable<Animation> animations, int fps) => animations.Select(a => GetValue(a, fps));

        /// <summary>
        /// 2つの Animation の GetValue メソッドからそれぞれ値を取得する。
        /// </summary>
        /// <param name="a1">1つ目の Animation</param>
        /// <param name="a2">2つ目の Animation</param>
        /// <param name="fps">FPS</param>
        /// <returns>Animation の GetValue メソッドの戻り値をまとめた Double2</returns>
        public Double2 GetDouble2(Animation a1, Animation a2, int fps) => new(GetValue(a1, fps), GetValue(a2, fps));

        /// <summary>
        /// 3つの Animation の GetValue メソッドからそれぞれ値を取得する。
        /// </summary>
        /// <param name="a1">1つ目の Animation</param>
        /// <param name="a2">2つ目の Animation</param>
        /// <param name="a3">3つ目の Animation</param>
        /// <param name="fps">FPS</param>
        /// <returns>Animation の GetValue メソッドの戻り値をまとめた Double3</returns>
        public Double3 GetDouble3(Animation a1, Animation a2, Animation a3, int fps) => new(GetValue(a1, fps), GetValue(a2, fps), GetValue(a3, fps));
    }
}
