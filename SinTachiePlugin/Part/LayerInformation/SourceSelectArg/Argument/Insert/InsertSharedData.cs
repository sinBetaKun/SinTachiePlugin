using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.InsertArg;
using SinTachiePlugin.Part.LayerInformation.InsertArg.Parameter;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.Insert
{
    internal class InsertSharedData
    {
        public PartInsertDefineMode InsertDefineMode { get; set; } = PartInsertDefineMode.DontInsert;
        public InsertArgBase InsertArg { get; set; } = new DontInsertParameter();

        public InsertSharedData()
        {   
        }

        public InsertSharedData(IInsertParameter parameter)
        {
            InsertDefineMode = parameter.InsertDefineMode;
            InsertArg = parameter.InsertArg.GetClone();
        }

        public void CopyTo(IInsertParameter parameter)
        {
            parameter.InsertDefineMode = InsertDefineMode;
            parameter.InsertArg = InsertArg.GetClone();
        }
    }
}
