using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinText : MonoBehaviour {
    [SerializeField] private Text coinText;
    private int coinCount = 0;
    private void OnEnable()
    {
        Coin.OnCoinCollected += IncrementCoinCout; 
        //SUBRCIBE COIN TO EVENT HERE
    }

    private void IncrementCoinCout()
    {
        coinCount++;
        coinText.text = coinCount.ToString();
    }
}
