using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coin : MonoBehaviour, ICollectible
{
    public static event Action OnCoinCollected;
    public void Collect()
    {
        Destroy(gameObject);
        OnCoinCollected?.Invoke();
    }

}
