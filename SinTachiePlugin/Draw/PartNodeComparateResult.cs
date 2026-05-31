namespace SinTachiePlugin.Draw
{
    internal record PartNodeComparateResult(bool CommandList = false, bool PartNetwork = false, bool Value = false)
    {
        public static PartNodeComparateResult operator +(PartNodeComparateResult a, PartNodeComparateResult b)
            => new(
                CommandList: a.CommandList | b.CommandList,
                PartNetwork: a.PartNetwork | b.PartNetwork,
                Value: a.Value | b.Value);
    }
}
