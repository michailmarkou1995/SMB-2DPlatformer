namespace Interfaces.Core.Player
{
    public interface IDeath
    {
        public bool IsDying { get; set; }
        public float DeadUpTimer { get; set; }
    }
}