using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.InsertArg.Argument.InsertPosition;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.InsertArg.Parameter
{
    internal class SelectPositionParameter : InsertArgBase, IInsertPositionParameter
    {

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_InsertArg_InsertPosition), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public PartInsertPosition InsertPosition { get => _insertPosition; set => Set(ref _insertPosition, value); }
        private PartInsertPosition _insertPosition = PartInsertPosition.Back;

        public SelectPositionParameter()
        {
        }

        public SelectPositionParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(InsertArgBase? origin)
        {
            if (origin is IInsertPositionParameter ip)
                InsertPosition = ip.InsertPosition;
        }

        public override InsertArgBase GetClone()
        {
            SelectPositionParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new InsertPositionSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<InsertPositionSharedData>() is InsertPositionSharedData insertPositionSharedData)
                insertPositionSharedData.CopyTo(this);
        }
    }
}
