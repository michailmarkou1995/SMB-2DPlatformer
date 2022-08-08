using Interfaces.Core.Managers;
using Interfaces.Core.Player;
using UnityEngine;

namespace Core.Player
{
    public class PlayerSize : MonoBehaviour, IPlayerSize
    {
        public void UpdateSize()
        {
            GetComponent<Animator>().SetInteger(PlayerAnimatorStatic.PlayerSizeAnimator,
                GameObject.FindGameObjectWithTag("LevelManager").GetComponent<ILevelManager>().GetGameStateData
                    .PlayerSize);
        }
    }
}