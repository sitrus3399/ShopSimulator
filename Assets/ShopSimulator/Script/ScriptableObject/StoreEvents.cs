using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StoreEvents", menuName = "Events/Store", order = 0)]
public class StoreEvents : ScriptableObject
{
    public Action OnQueue;
    public Action OnBuy;
    public Action OnFinishCustomer;
    public Action<float> OnChangeCurrency;
    public Action OnExitEDC;
    public Action OnExitCashMachine;
    public Action<CashChange> OnAddChange;
    public Action<CashChange> OnRemoveChange;
    public Action<float> OnPayment;
    public Action<float, PaymentType> OnTakePayment;
    public Action<float, Item> OnItemRegister;

    public void Payment(float price)
    {
        OnPayment?.Invoke(price);
    }

    public void FinishCustomer()
    {
        OnFinishCustomer?.Invoke();
    }

    public void TakePayment(float value, PaymentType type)
    {
        OnTakePayment?.Invoke(value, type);
    }

    public void ExitEDC()
    {
        OnExitEDC?.Invoke();
    }

    public void ExitCashMachine()
    {
        OnExitCashMachine?.Invoke();
    }

    public void AddChange(CashChange newChange)
    {
        OnAddChange?.Invoke(newChange);
    }

    public void RemoveChange(CashChange newChange)
    {
        OnRemoveChange?.Invoke(newChange);
    }

    public void ItemRegister(float value, Item newItem)
    {
        OnItemRegister?.Invoke(value, newItem);
    }

    public void ChangeCurrency(float value)
    {
        OnChangeCurrency?.Invoke(value);
    }
}