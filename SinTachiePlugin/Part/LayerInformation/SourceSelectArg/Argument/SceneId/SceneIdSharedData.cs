namespace SinTachiePlugin.Part.LayerInformation.SourceSelectArg.Argument.SceneId
{
    internal class SceneIdSharedData
    {
        public Guid SceneId { get; set; }

        public SceneIdSharedData()
        {
        }

        public SceneIdSharedData(ISceneIdParameter parameter)
        {
            SceneId = parameter.SceneId;
        }

        public void CopyTo(ISceneIdParameter parameter)
        {
            parameter.SceneId = SceneId;
        }
    }
}
