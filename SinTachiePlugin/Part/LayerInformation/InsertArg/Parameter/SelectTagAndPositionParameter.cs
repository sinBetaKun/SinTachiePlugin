using SinTachiePlugin.Enums;
using SinTachiePlugin.Part.LayerInformation.InsertArg.Argument.InsertPosition;
using SinTachiePlugin.Part.LayerInformation.InsertArg.Argument.TagToInsert;
using SinTachiePlugin.Properties;
using System.ComponentModel.DataAnnotations;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.InsertArg.Parameter
{
    internal class SelectTagAndPositionParameter : InsertArgBase, ITagToInsertParameter, IInsertPositionParameter
    {
        [Display(Name = nameof(TextResource.PartParam_LayerInfo_InsertArg_TagToInsert), ResourceType = typeof(TextResource))]
        [TextEditor]
        public string TagToInsert { get => _tagToInsert; set => Set(ref _tagToInsert, value); }
        private string _tagToInsert = string.Empty;

        [Display(Name = nameof(TextResource.PartParam_LayerInfo_InsertArg_InsertPosition), ResourceType = typeof(TextResource))]
        [EnumComboBox]
        public PartInsertPosition InsertPosition { get => _insertPosition; set => Set(ref _insertPosition, value); }
        private PartInsertPosition _insertPosition = PartInsertPosition.Back;

        public SelectTagAndPositionParameter()
        {
        }

        public SelectTagAndPositionParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(InsertArgBase? origin)
        {
            if (origin is ITagToInsertParameter tp)
                TagToInsert = tp.TagToInsert;
            if (origin is IInsertPositionParameter ip)
                InsertPosition = ip.InsertPosition;
        }

        public override InsertArgBase GetClone()
        {
            SelectTagAndPositionParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new TagToInsertSharedData(this));
            store.Save(new InsertPositionSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<TagToInsertSharedData>() is TagToInsertSharedData tagToInsertSharedData)
                tagToInsertSharedData.CopyTo(this);
            if (store.Load<InsertPositionSharedData>() is InsertPositionSharedData insertPositionSharedData)
                insertPositionSharedData.CopyTo(this);
        }
    }
}
