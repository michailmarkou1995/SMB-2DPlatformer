using Interfaces.UI;

namespace Interfaces.Core.Managers
{
    public interface ILevelManager : ILevelManagerEssentials
    {
        public void RetrieveGameState();
    }
    
    public interface ILevelManagerEssentials
    {
        
    }
    
}