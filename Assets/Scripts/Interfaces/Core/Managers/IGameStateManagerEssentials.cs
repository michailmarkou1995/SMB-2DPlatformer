namespace Interfaces.Core.Managers
{
    public interface IGameStateManagerEssentials
    {
        public bool SpawnFromPoint { get; set; }
        public int SpawnPointIdx { get; set; }
        public int SpawnPipeIdx { get; set; }
        public int PlayerSize { get; set; } // 0..2
        public int Lives { get; set; }
        public int Coins { get; set; }
        public int CoinBonus { get; set; }
        public int PowerupBonus { get; set; }
        public int StarmanBonus { get; set; }
        public int OneupBonus { get; set; }
        public int BreakBlockBonus { get; set; }
        public int Scores { get; set; }
        public float TimeLeft { get; set; }
        public int TimeLeftInt { get; set; }
        public bool HurryUp { get; set; } // within last 100 secs?
        public string SceneToLoad { get; set; }
        public bool TimeUp { get; set; }
        public bool GamePaused { get; set; }
        public bool TimerPaused { get; set; }
        public bool MusicPaused { get; set; }
    }
}