using Newtonsoft.Json;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Properties;
using System.IO;

namespace SinTachiePlugin.Parts
{
    [Obsolete]
    internal class PartInfo
    {
        public PartBlock? DefaltValues { get; set; }
        public string StpiPath = string.Empty;

        static public string Extension => "stpi";

        public static JsonSerializerSettings GetJsonSetting =>
            new()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

        private static string MakeStpiPath(string sourcePath)
        {
            if (Path.GetDirectoryName(sourcePath) is not string dirName)
                return string.Empty;

            if (Path.GetFileNameWithoutExtension(sourcePath) is not string fileName)
                return string.Empty;

            return Path.Join(dirName, fileName) + "." + Extension;
        }

        public static PartInfo? ReadStpi(string sourcePath)
        {
            string stpiPath = MakeStpiPath(sourcePath);

            if (string.IsNullOrEmpty(stpiPath))
                return null;

            if (!Path.Exists(stpiPath))
                return null;

            try
            {
                using FileStream stream = new(stpiPath, FileMode.Open);
                using StreamReader sr = new(stream);

                if (JsonConvert.DeserializeObject<PartInfo>(sr.ReadToEnd(), GetJsonSetting) is PartInfo info)
                {
                    info.StpiPath = stpiPath;
                    return info;
                }
                else
                {
                    SinTachieDialog.ShowWarning(TextResource.ErrorMessage_FailToGetInfoFromStpsi);
                    return null;
                }
            }
            catch (Exception ex)
            {
                SinTachieDialog.ShowWarning(TextResource.ErrorMessage_FailToReadStpsi + "\n" + ex.Message);
                return null;
            }
        }
    }
}
