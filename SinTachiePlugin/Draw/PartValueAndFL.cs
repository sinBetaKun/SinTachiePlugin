using SinTachiePlugin.Part;

namespace SinTachiePlugin.Draw
{
    internal class PartValueAndFL(PartValue partValue, FrameAndLength fl)
    {
        public PartValue PartValue { get; init; } = partValue;
        public FrameAndLength FL { get; init; } = fl;
    }
}
