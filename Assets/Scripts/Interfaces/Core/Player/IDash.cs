namespace Interfaces.Core.Player
{
    public interface IDash
    {
        public bool IsDashing { get; set; }
        public bool WasDashingBeforeJump { get; set; }
    }
}