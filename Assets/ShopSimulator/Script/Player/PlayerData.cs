using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private PlayerEvents playerEvent;
    [SerializeField] private StoreEvents storeEvents;
    [SerializeField] private float playerCurrency;

    void Start()
    {
        storeEvents.OnChangeCurrency += AddCurrency;
        playerEvent.UpdateCurrencyUI(playerCurrency);
    }

    public void AddCurrency(float value)
    {
        playerCurrency += value;
        playerEvent.UpdateCurrencyUI(playerCurrency);
    }
}
