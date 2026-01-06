using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : Singleton<CustomerManager>
{
    [SerializeField] private Customer customerPrefab;
    [SerializeField] private List<Customer> customerList;
    private List<Customer> customerListObjectPool = new List<Customer>();
    [SerializeField] private int maxCustomer;

    [Header("Respawn")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 0f;
    [SerializeField] private float spawnMinInterval = 1f;
    [SerializeField] private float spawnMaxInterval = 20f;

    public Transform SpawnPoint { get { return spawnPoint; } }

    void Start()
    {
        maxCustomer = ShopManager.Instance.CashierDesk.QueueLocation.Count;

        SpawnCustomer();
    }

    private void Update()
    {
        if (customerList.Count >= maxCustomer) return;

        spawnInterval -= Time.deltaTime;

        if (spawnInterval <= 0)
        {
            SpawnCustomer();
        }
    }

    void SpawnCustomer()
    {
        Customer selectCustomer = SelectCustomer();
        selectCustomer.gameObject.SetActive(true);
        selectCustomer.transform.position = spawnPoint.position;
        selectCustomer.ResetData();

        AddCustomer(selectCustomer);

        spawnInterval = Random.Range(spawnMinInterval, spawnMaxInterval);
    }

    public void AddCustomer(Customer tmpCustomer)
    {
        customerList.Add(tmpCustomer);
    }

    public void RemoveCustomer(Customer tmpCustomer) 
    { 
        tmpCustomer.gameObject.SetActive(false);
        customerList.Remove(tmpCustomer); 
    }

    private Customer SelectCustomer()
    {
        foreach (Customer tmpCustomer in customerListObjectPool)
        {
            if (!tmpCustomer.gameObject.activeInHierarchy)
            {
                return tmpCustomer;
            }
        }

        return CreateCustomer();
    }

    private Customer CreateCustomer()
    {
        Customer newCustomer = Instantiate(customerPrefab);
        newCustomer.gameObject.SetActive(false);
        customerListObjectPool.Add(newCustomer);
        return newCustomer;
    }
}