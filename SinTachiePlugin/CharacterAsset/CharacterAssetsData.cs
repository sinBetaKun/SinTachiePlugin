using SinTachiePlugin.Enums;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Properties;
using System.Collections.Immutable;
using YukkuriMovieMaker.Commons;

namespace SinTachiePlugin.CharacterAsset
{
    public class CharacterAssetsData : Animatable
    {
        public CharacterAssetType AssetType { get => _assetType; private set => Set(ref _assetType, value); }
        private CharacterAssetType _assetType = CharacterAssetType.Directory;

        public string Path { get => _path; private set => Set(ref _path, value); }
        private string _path = string.Empty;

        public bool Packed { get => _packed; private set => Set(ref _packed, value); }
        private bool _packed = false;

        public ImmutableList<string> ObsoletePaths { get => _obsoletePaths; private set => Set(ref _obsoletePaths, value); }
        private ImmutableList<string> _obsoletePaths = [];

        public CharacterAssetsData()
        {
        }

        public CharacterAssetsData(CharacterAssetsData origin)
        {
            AssetType = origin.AssetType;
            Path = origin.Path;
            Packed = origin.Packed;
            ObsoletePaths = origin.ObsoletePaths;
        }

        public void ChangeType(CharacterAssetType assetType)
        {
            if (AssetType == assetType)
                return;

            DialogResult dr_0 = SinTachieDialog.GetOKorCancel(
                TextResource.CharacterAssetsData_AskChangeType);

            if (dr_0 == DialogResult.OK)
            {
                AssetType = assetType;
                Path = string.Empty;
                Packed = false;
                ObsoletePaths = [];
            }
        }

        public void ChangePath(string path)
        {
            if (Path == path || string.IsNullOrEmpty(path))
                return;

            if (!ObsoletePaths.Contains(path))
                return;

            DialogResult dr_0 = SinTachieDialog.GetYESorNO(
                TextResource.CharacterAssetsData_AskSubscribeObsoletePath);

            if (dr_0 == DialogResult.Yes)
            {
                ObsoletePaths = ObsoletePaths.Add(path);
            }

            Path = path;
            Packed = false;
        }

        public void ChangePathToPacking(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            DialogResult dr_0 = SinTachieDialog.GetOKorCancel(
                TextResource.CharacterAssetsData_AskChangePathToPacking
                + "\n" + path);

            if (dr_0 == DialogResult.OK)
            {
                ChangePath(path);
                Packed = true;
            }
        }

        public void RemoveObsoletePath(string path)
        {
            if (ObsoletePaths.Contains(path))
                ObsoletePaths = ObsoletePaths.Remove(path);
        }

        public void AddObsoletePath(string path)
        {
            if (!ObsoletePaths.Contains(path))
                ObsoletePaths = ObsoletePaths.Add(path);
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];
    }
}
