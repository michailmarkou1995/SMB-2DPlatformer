using Interfaces.UI;

namespace Interfaces.Core.Managers
{
    public interface IGameStateDataReset : ITimeLeft
    {
        public bool HurryUp { get; set; } // within last 100 secs?
        public int PlayerSize { get; set; } // 0..2
        public int Lives { get; set; }
        public int Coins { get; set; }
        public int Scores { get; set; }
    }
}