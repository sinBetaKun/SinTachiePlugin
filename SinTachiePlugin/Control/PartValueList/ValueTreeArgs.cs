using SinTachiePlugin.Part;

namespace SinTachiePlugin.Control.PartValueList
{
    internal record ValueTreeArgs(List<PartValue> List, PartValue? Selected, List<PartValue> AdditionalSelecteds)
    {
    }
}
