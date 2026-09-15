using PsdParser;

namespace SinTachiePlugin.Draw
{
    internal class PsdFileQuery(string filePath, PsdFile psdFile) : IDisposable
    {
        public string FilePath { get; init; } = filePath;
        public PsdFile PsdFile { get; init; } = psdFile;

        public void Dispose()
        {
            PsdFile.Dispose();
        }
    }
}
