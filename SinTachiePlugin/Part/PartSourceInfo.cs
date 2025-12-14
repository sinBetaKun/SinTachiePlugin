using Newtonsoft.Json;
using SinTachiePlugin.Informations;
using SinTachiePlugin.Parts;
using SinTachiePlugin.Properties;
using System.IO;
using System.Text;

namespace SinTachiePlugin.Part
{
    internal class PartSourceInfo
    {
        public static string Extension => "stpsi";

        [JsonIgnore]
        private string _stpsiPath = string.Empty;

        [JsonIgnore]
        private string _sourcePath = string.Empty;

        public static JsonSerializerSettings GetJsonSetting =>
            new()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

        public List<ControlledParametersOfPartTemplate> Templates { get; set; }

        public PartSourceInfo()
        {
            Templates = [];
        }

        [Obsolete]
        public PartSourceInfo(PartInfo partInfo)
        {
            if (partInfo.DefaltValues is not PartBlock block)
            {
                Templates = [];
                return;
            }

            ControlledParametersOfPart param = new(block);
            ControlledParametersOfPartTemplate template = new(TextResource.PartSourceInfo_DefaultTemplateName, param);
            Templates = [template];
        }

        private static string MakeStpsiPath(string sourcePath)
        {
            if (Path.GetDirectoryName(sourcePath) is not string dirName)
                return string.Empty;

            if (Path.GetFileNameWithoutExtension(sourcePath) is not string fileName)
                return string.Empty;

            return Path.Join(dirName, fileName) + "." + Extension;
        }

        public static PartSourceInfo? ReadStpsi(string sourcePath)
        {
            string stpsiPath = MakeStpsiPath(sourcePath);

            if (string.IsNullOrEmpty(stpsiPath))
                return null;

            if (!Path.Exists(stpsiPath))
                return null;

            try
            {
                using FileStream stream = new(stpsiPath, FileMode.Open);
                using StreamReader sr = new(stream);

                if (JsonConvert.DeserializeObject<PartSourceInfo>(sr.ReadToEnd(), GetJsonSetting) is PartSourceInfo info)
                {
                    info._sourcePath = sourcePath;
                    info._stpsiPath = stpsiPath;
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

        public void Export()
        {
            try
            {
                string stspi = JsonConvert.SerializeObject(this, Formatting.Indented, GetJsonSetting);
                using StreamWriter sw = new(_stpsiPath, false, Encoding.UTF8);
                sw.Write(stspi);
            }
            catch (Exception e)
            {
                SinTachieDialog.ShowError(new(TextResource.ErrorMessage_FailToSerializeStpsi + "\n" + e.Message));
                return;
            }
        }
    }
}
