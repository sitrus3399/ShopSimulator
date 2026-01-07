using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Cashier : MonoBehaviour
{
    [SerializeField] private StoreEvents storeEvent;

    [SerializeField] private float currentBill;
    [SerializeField] private List<Item> itemOnBill = new List<Item>();

    [SerializeField] private List<Transform> queueLocation;
    [SerializeField] private List<Customer> queueCustomer;
    [SerializeField] private Customer firstCustomer;
    public Action<Transform> SetQueuePoint;

    [SerializeField] private Transform dropPoint;

    [Header("Payment")]
    [SerializeField] private Transform dropPointEDC;
    [SerializeField] private EDCMachine edcMachine;
    [SerializeField] private CashMachine cashMachine;

    [Header("UI")]
    [SerializeField] private TMP_Text billText;
    [SerializeField] private TMP_Text paymentText;

    public List<Transform> QueueLocation { get { return queueLocation; } }
    public Transform DropPoint { get { return dropPoint; } }

    private void Start()
    {
        storeEvent.OnQueue += SortQueue;
        storeEvent.OnItemRegister += AddBill;
        storeEvent.OnTakePayment += TakePayment;
        storeEvent.OnFinishCustomer += NextCustomer;
        storeEvent.OnExitEDC += ExitEDC;
        storeEvent.OnExitCashMachine += ExitCashMachine;
    }

    private void OnDisable()
    {
        storeEvent.OnQueue -= SortQueue;
        storeEvent.OnItemRegister -= AddBill;
        storeEvent.OnTakePayment -= TakePayment;
        storeEvent.OnFinishCustomer -= NextCustomer;
        storeEvent.OnExitEDC -= ExitEDC;
        storeEvent.OnExitCashMachine -= ExitCashMachine;
    }

    public void AddCustomer(Customer newCustomer)
    {
        queueCustomer.Add(newCustomer);
    }

    public void RemoveCustomer(Customer newCustomer)
    {
        queueCustomer.Remove(newCustomer);
    }

    public void SortQueue()
    {
        for (int i = 0; i < queueCustomer.Count; i++)
        {
            queueCustomer[i].SetTarget(queueLocation[i]);

            queueCustomer[i].CheckIfFirstInLine();
        }
    }

    [SerializeField] private float processingTimePerItem = 1.0f;
    private bool isProcessing = false;

    public bool IsFirstInQueue(Customer customer)
    {
        return queueCustomer.Count > 0 && queueCustomer[0] == customer;
    }

    public void SetFirstCustomer(Customer newCustomer)
    {
        firstCustomer = newCustomer;
    }

    void AddBill(float newBill, Item newItem)
    {
        currentBill += newBill;
        itemOnBill.Add(newItem);

        billText.text = $"${currentBill}";

        if (CheckBillMatch())
        {
            storeEvent.Payment(currentBill);
        }
    }

    bool CheckBillMatch()
    {
        if (itemOnBill.Count != firstCustomer.ItemOnHand.Count) return false;

        if (itemOnBill.Count != firstCustomer.ItemOnHand.Count) return false;

        var sortedBillNames = itemOnBill.Select(x => x.name).OrderBy(n => n);
        var sortedHandNames = firstCustomer.ItemOnHand.Select(x => x.name).OrderBy(n => n);

        //List<Item> sortedBill = itemOnBill.OrderBy(x => x.name).ToList();
        //List<Item> sortedHand = firstCustomer.ItemOnHand.OrderBy(x => x.name).ToList();

        return sortedBillNames.SequenceEqual(sortedHandNames);
    }

    void TakePayment(float value, PaymentType type)
    {
        switch (type)
        {
            case PaymentType.Cash:
                Debug.Log($"Payment {value}");
                paymentText.text = $"${value}";
                cashMachine.SetTarget(value - currentBill, currentBill);

                if (value == currentBill)
                {
                    storeEvent.ChangeCurrency(currentBill);
                    Invoke("HoldNextCustomer", 3f);
                }
                else
                {
                    cashMachine.ShowCashBox();
                }
                break;
            case PaymentType.Card:
                paymentText.text = $"$0.00";
                edcMachine.UseEDC(currentBill);
                break;
            default:
                break;
        }
    }

    void HoldNextCustomer()
    {
        storeEvent.FinishCustomer();
    }

    void NextCustomer()
    {
        if (firstCustomer != null)
        {
            firstCustomer.ChangeState(CustomerState.ExitStore);
            RemoveCustomer(firstCustomer);

            firstCustomer = null;
        }
            
        currentBill = 0;
        billText.text = "$0.00";
        paymentText.text = "$0.00";
        itemOnBill.Clear();

        if (queueCustomer.Count > 0)
        {
            firstCustomer = queueCustomer[0];
        }
        else
        {
            firstCustomer = null;
        }
        SortQueue();
    }

    void ExitEDC()
    {
        edcMachine.gameObject.transform.parent = dropPointEDC;
        edcMachine.gameObject.transform.localPosition = Vector3.zero;
        edcMachine.gameObject.transform.rotation = dropPointEDC.rotation;
    }

    void ExitCashMachine()
    {
        cashMachine.HideCashBox();
    }
}