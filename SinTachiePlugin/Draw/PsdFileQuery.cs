using PsdParser;

namespace SinTachiePlugin.Draw
{
    internal class PsdFileQuery(string filePath, PsdFile psdFile)
    {
        public string FilePath { get; init; } = filePath;
        public PsdFile PsdFile { get; init; } = psdFile;
    }
}
