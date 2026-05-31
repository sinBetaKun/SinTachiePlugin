using System.Text.Json.Serialization;
using SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.IsOpened;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Project;

namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Parameter
{
    internal class GroupParameter : SourceSelectArgBase, IIsOpenedParameter
    {
        [JsonIgnore]
        public bool IsOpened { get => _isOpened; set => Set(ref _isOpened, value); }
        private bool _isOpened;

        public GroupParameter()
        {
        }

        public GroupParameter(SharedDataStore? store) : base(store)
        {
        }

        public override void CopyFrom(SourceSelectArgBase? origin)
        {
            if (origin is IIsOpenedParameter isOpenedParameter)
                IsOpened = isOpenedParameter.IsOpened;
        }

        public override SourceSelectArgBase GetClone()
        {
            GroupParameter clone = new();
            clone.CopyFrom(this);
            return clone;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];

        protected override void SaveSharedData(SharedDataStore store)
        {
            store.Save(new IsOpenedSharedData(this));
        }

        protected override void LoadSharedData(SharedDataStore store)
        {
            if (store.Load<IsOpenedSharedData>() is IsOpenedSharedData isOpenedSharedData)
                isOpenedSharedData.CopyTo(this);
        }
    }
}
