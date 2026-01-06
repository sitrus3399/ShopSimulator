using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Payment : MonoBehaviour
{
    [SerializeField] private StoreEvents storeEvent;
    [SerializeField] private PaymentType paymentType;
    [SerializeField] private float price;

    public float Price { get { return price; } }
    public PaymentType PaymentType { get { return paymentType; } }

    private void Start()
    {
        storeEvent.OnTakePayment += TakePayment;
    }

    public void SetPrice(float newPrice)
    {
        price = newPrice;
    }

    void TakePayment(float value, PaymentType type)
    {
        gameObject.SetActive(false);
    }
}

[System.Serializable]
public enum PaymentType
{
    Cash,
    Card
}
