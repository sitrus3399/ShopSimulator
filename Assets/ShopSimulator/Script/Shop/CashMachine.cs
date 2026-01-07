using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CashMachine : MonoBehaviour
{
    [SerializeField] private float totalChange;
    [SerializeField] private float currentBill;

    [SerializeField] private StoreEvents storeEvents;
    [SerializeField] private GameObject cashMachineBox;

    [SerializeField] private Transform cashMachineBoxHidePoint;
    [SerializeField] private Transform cashMachineBoxShowPoint;

    [SerializeField] private List<CashChange> cashChangeList;

    [SerializeField] private List<CashChange> cashChangeActive;
    [SerializeField] private Transform cashChangeDropPoint;

    [SerializeField] private float targetChange;

    [SerializeField] private TMP_Text changeText;

    private void Start()
    {
        storeEvents.OnAddChange += AddChange;
        storeEvents.OnRemoveChange += RemoveChange;
        storeEvents.OnFinishCustomer += ResetChange;
    }

    private void OnDestroy()
    {
        storeEvents.OnAddChange -= AddChange;
        storeEvents.OnRemoveChange -= RemoveChange;
        storeEvents.OnFinishCustomer -= ResetChange;
    }

    public void ShowCashBox()
    {
        cashMachineBox.transform.localPosition = cashMachineBoxShowPoint.localPosition;
    }

    public void HideCashBox()
    {
        cashMachineBox.transform.localPosition = cashMachineBoxHidePoint.localPosition;
    }

    public void AddChange(CashChange tmpChange)
    {
        CashChange newChange = Instantiate(tmpChange, cashChangeDropPoint.position, cashChangeDropPoint.rotation);
        newChange.ChangeOnBox(false);
        cashChangeActive.Add(newChange);
        totalChange += newChange.ValueChange;
        UpdateUI();
        CheckChange();
    }

    public void RemoveChange(CashChange tmpChange)
    {
        cashChangeActive.Remove(tmpChange);

        totalChange -= tmpChange.ValueChange;
        UpdateUI();
        CheckChange();
        Destroy(tmpChange.gameObject);
    }

    public void ResetChange()
    {
        totalChange = 0;
        changeText.text = $"$0.00";

        foreach (CashChange cash in cashChangeActive)
        {
            Destroy(cash.gameObject);
        }

        cashChangeActive.Clear();
    }

    void UpdateUI()
    {
        changeText.text = $"${totalChange:F2}";
    }

    void CheckChange()
    {
        if (targetChange == totalChange)
        {
            Debug.Log("Transaksi Berhasil: $" + totalChange.ToString("F2"));

            storeEvents.ChangeCurrency(currentBill);
            storeEvents.FinishCustomer();

            storeEvents.ExitCashMachine();
        }
    }

    public void SetTarget(float newTarget, float totalBill)
    {
        currentBill = totalBill;
        targetChange = newTarget;
    }
}