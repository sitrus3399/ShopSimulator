using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashierDesk : MonoBehaviour
{
    [SerializeField] private StoreEvents storeEvent;

    [SerializeField] private List<Transform> queueLocation;
    [SerializeField] private List<Customer> queueCustomer;
    public Action<Transform> SetQueuePoint;

    [SerializeField] private Transform dropPoint;

    public List<Transform> QueueLocation { get { return queueLocation; } }
    public Transform DropPoint { get { return dropPoint; } }

    private void Start()
    {
        storeEvent.OnQueue += SortQueue;
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
            // Beritahu customer untuk mengecek status antriannya
            queueCustomer[i].CheckIfFirstInLine();
        }
    }

    [SerializeField] private float processingTimePerItem = 1.0f;
    private bool isProcessing = false;

    // Cek apakah customer ini yang paling depan
    public bool IsFirstInQueue(Customer customer)
    {
        return queueCustomer.Count > 0 && queueCustomer[0] == customer;
    }

    // Fungsi untuk mensimulasikan kasir memproses barang
    public IEnumerator ProcessCustomerItems(Customer customer, List<Item> items)
    {
        isProcessing = true;
        Debug.Log("Kasir mulai memproses barang...");

        // Simulasi waktu proses per barang
        yield return new WaitForSeconds(items.Count * processingTimePerItem);

        Debug.Log("Proses selesai, customer boleh pergi.");
        isProcessing = false;

        // Perintahkan customer untuk keluar
        customer.ChangeState(CustomerState.ExitStore);
    }
}