using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class CoinText : MonoBehaviour {
        [SerializeField] private Text coinText;
        private int coinCount = 0;
        private void OnEnable()
        {
            Pickups.Coin.OnCoinCollected += IncrementCoinCout; 
            //SUBRCIBE COIN TO EVENT HERE
        }

        private void IncrementCoinCout()
        {
            coinCount++;
            coinText.text = coinCount.ToString();
        }
    }
}
