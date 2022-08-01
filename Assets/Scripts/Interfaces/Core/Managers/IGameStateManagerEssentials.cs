namespace Interfaces.Core.Managers
{
    public interface IGameStateManagerEssentials
    {
        public bool SpawnFromPoint { get; set; }
        public int SpawnPointIdx { get; set; }
        public int SpawnPipeIdx { get; set; }

        public int PlayerSize { get; set; }
        public int Lives { get; set; }
        public int Coins { get; set; }
        public int Scores { get; set; }
        public float TimeLeft { get; set; }
        public bool HurryUp { get; set; }

        public string SceneToLoad { get; set; }
        public bool TimeUp { get; set; }
    }
}