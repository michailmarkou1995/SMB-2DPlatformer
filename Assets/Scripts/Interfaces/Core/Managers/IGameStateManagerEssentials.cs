namespace Interfaces.Core.Managers
{
    public interface IGameStateManagerEssentials : IGameStateDataReset
    {
        public bool SpawnFromPoint { get; set; }
        public int SpawnPointIdx { get; set; }
        public int SpawnPipeIdx { get; set; }
        public string SceneToLoad { get; set; }
        public bool TimeUp { get; set; }
    }
}