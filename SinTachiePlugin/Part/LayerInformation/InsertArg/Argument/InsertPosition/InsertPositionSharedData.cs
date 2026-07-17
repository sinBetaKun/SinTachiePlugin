using SinTachiePlugin.Enums;

namespace SinTachiePlugin.Part.LayerInformation.InsertArg.Argument.InsertPosition
{
    internal class InsertPositionSharedData
    {
        public PartInsertPosition InsertPosition { get; set; } = PartInsertPosition.Back;

        public InsertPositionSharedData()
        {
        }

        public InsertPositionSharedData(IInsertPositionParameter parameter)
        {
            InsertPosition = parameter.InsertPosition;
        }

        public void CopyTo(IInsertPositionParameter parameter)
        {
            parameter.InsertPosition = InsertPosition;
        }
    }
}
