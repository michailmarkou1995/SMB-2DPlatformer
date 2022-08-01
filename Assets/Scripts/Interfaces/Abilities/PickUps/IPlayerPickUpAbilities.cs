using UnityEngine;

namespace Interfaces.Abilities.Pickups
{
    public interface IPlayerPickUpAbilities
    {
        // public void AddLife(Vector3 spawnPos = default);
        // public void AddCoin(Vector3 spawnPos = default);
        // public void AddScore(int? bonus = null, Vector3 spawnPos = default);
        public void AddLife();
        public void AddLife(Vector3 spawnPos);
        public void AddCoin();
        public void AddCoin(Vector3 spawnPos);
        public void AddScore(int bonus);
        public void AddScore(int bonus, Vector3 spawnPos);
    }
}