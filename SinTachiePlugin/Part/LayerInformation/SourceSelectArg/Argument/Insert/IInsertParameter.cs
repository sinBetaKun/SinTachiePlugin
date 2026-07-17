using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.InsertArg;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Insert
{
    internal interface IInsertParameter
    {
        public PartInsertDefineMode InsertDefineMode { get; set; }
        public InsertArgBase InsertArg { get; set; }
    }
}
