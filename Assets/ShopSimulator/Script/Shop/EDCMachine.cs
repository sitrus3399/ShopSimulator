using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EDCMachine : MonoBehaviour
{
    [SerializeField] private PlayerEvents playerEvent;
    [SerializeField] private StoreEvents storeEvent;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private List<Button> numberButtons;
    [SerializeField] private Button commaButton;
    [SerializeField] private Button backspaceButton;
    [SerializeField] private Button confirmButton;

    [Header("Settings")]
    [SerializeField] private int maxDigits = 8;

    [Header("Output Value")]
    [SerializeField] private float totalPrice;
    [SerializeField] private float targetPrice;

    private string currentInput = "";

    void Start()
    {
        // Setup tombol angka 0-9
        for (int i = 0; i < numberButtons.Count; i++)
        {
            int index = i;
            numberButtons[i].onClick.AddListener(() => { AppendDigit(index.ToString()); });
        }

        if (commaButton != null) commaButton.onClick.AddListener(AppendComma);
        if (backspaceButton != null) backspaceButton.onClick.AddListener(Backspace);
        if (confirmButton != null) confirmButton.onClick.AddListener(Confirm);

        UpdateDisplay();
    }

    public void AppendDigit(string digit)
    {
        if (currentInput.Length < maxDigits)
        {
            currentInput += digit;
            SyncTotalPrice();
            UpdateDisplay();
        }
    }

    public void AppendComma()
    {
        if (!currentInput.Contains(".") && currentInput.Length < maxDigits)
        {
            if (string.IsNullOrEmpty(currentInput))
            {
                currentInput = "0.";
            }
            else
            {
                currentInput += ".";
            }
            UpdateDisplay();
        }
    }

    public void Backspace()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            SyncTotalPrice();
            UpdateDisplay();
        }
    }

    private void SyncTotalPrice()
    {
        if (string.IsNullOrEmpty(currentInput) || currentInput == ".")
        {
            totalPrice = 0f;
            return;
        }

        float.TryParse(currentInput, NumberStyles.Any, CultureInfo.InvariantCulture, out totalPrice);
    }

    public void Clear()
    {
        currentInput = "";
        totalPrice = 0f;
        UpdateDisplay();
    }

    public void Confirm()
    {
        if (totalPrice <= 0) return;

        if (targetPrice == totalPrice)
        {
            Debug.Log("Transaksi Berhasil: $" + totalPrice.ToString("F2"));

            storeEvent.ChangeCurrency(targetPrice);
            storeEvent.FinishCustomer();
            Clear();

            ExitEDC();
        }
    }

    void UpdateDisplay()
    {
        if (string.IsNullOrEmpty(currentInput))
        {
            displayText.text = "$0.00";
        }
        else
        {
            displayText.text = "$" + currentInput;
        }
    }

    public void UseEDC(float newPrice)
    {
        Debug.Log($"Use EDC");
        targetPrice = newPrice;
        playerEvent.UseEDC(this);

        Clear();
    }

    public void ExitEDC()
    {
        storeEvent.ExitEDC();
    }
}