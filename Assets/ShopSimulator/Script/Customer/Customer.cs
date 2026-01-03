using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    [SerializeField] private StoreEvents storeEvent;

    [SerializeField] private CustomerState state;
    [SerializeField] private Transform target;
    private List<string> buyList = new List<string>();
    private int currentItemIndex = 0;

    private NavMeshAgent agent;
    [SerializeField] private float triggerDistance = 0.5f;
    [SerializeField] private float shelfTriggerDistance = 1.5f;
    private float distance;

    private Shelf targetShelf;
    [SerializeField] private float searchWaitDuration = 2.0f; // Durasi tunggu (detik)
    private float waitTimer;
    private bool isWaiting = false;

    private List<Item> itemOnHand = new List<Item>();
    [SerializeField] private Transform itemOnHandLocation;

    private bool hasDroppedItems = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (target != null)
        {
            agent.destination = target.position;
        }

        ChangeState(CustomerState.ToStore);
    }

    public void ResetData()
    {
        isWaiting = false;
        hasDroppedItems = false;
        currentItemIndex = 0;

        foreach (Item tmpItem in itemOnHand)
        {
            Destroy(tmpItem);
        }

        itemOnHand.Clear();

        ChangeState(CustomerState.ToStore);
    }

    void Update()
    {
        switch (state)
        {
            case CustomerState.ToStore:
                if (target != null)
                {
                    agent.SetDestination(target.position);

                    distance = Vector3.Distance(transform.position, target.position);
                }

                if (distance < triggerDistance)
                {
                    ChangeState(CustomerState.EnterStore);
                }
                break;
            case CustomerState.EnterStore:
                if (target != null)
                {
                    agent.SetDestination(target.position);

                    distance = Vector3.Distance(transform.position, target.position);
                }

                if (distance < triggerDistance)
                {
                    ChangeState(CustomerState.SearchItem);
                }
                break;
            case CustomerState.SearchItem:
                if (target != null)
                {
                    agent.SetDestination(target.position);

                    distance = Vector3.Distance(transform.position, target.position);
                }

                if (isWaiting)
                {
                    waitTimer -= Time.deltaTime;

                    if (waitTimer <= 0)
                    {
                        isWaiting = false;
                        agent.isStopped = false;

                        if (currentItemIndex < buyList.Count)
                        {
                            FindNextShelf();
                        }
                        else
                        {
                            ChangeState(CustomerState.ToCashier);
                        }
                    }
                }
                else if (distance < shelfTriggerDistance)
                {
                    //Debug.Log($"Mengambil item: {buyList[currentItemIndex]} (Index: {currentItemIndex})");

                    for (int i = targetShelf.Items.Count - 1; i >= 0; i--)
                    {
                        Item item = targetShelf.Items[i];

                        if (item.ItemName == buyList[currentItemIndex])
                        {
                            targetShelf.RemoveItem(item);

                            itemOnHand.Add(item); // Dipindah ke tangan atau tas belanja
                            item.transform.parent = itemOnHandLocation;
                            item.transform.localPosition = new Vector3(0, 0, 0);

                            break;
                        }
                    }

                    currentItemIndex++;

                    isWaiting = true;
                    waitTimer = searchWaitDuration;
                    agent.isStopped = true;
                }
                break;
            case CustomerState.ToCashier:
                if (target != null)
                {
                    agent.SetDestination(target.position);

                    distance = Vector3.Distance(transform.position, target.position);
                }

                if (distance < triggerDistance)
                {
                    ChangeState(CustomerState.OnCashier);
                }
                break;
            case CustomerState.OnCashier:
                //Waiting item scan

                CheckIfFirstInLine();
                break;
            case CustomerState.ExitStore:
                if (target != null)
                {
                    agent.SetDestination(target.position);

                    distance = Vector3.Distance(transform.position, target.position);
                }

                if (distance < triggerDistance)
                {
                    ChangeState(CustomerState.ToDespawn);
                }
                break;
            case CustomerState.ToDespawn:
                if (target != null)
                {
                    agent.SetDestination(target.position);

                    distance = Vector3.Distance(transform.position, target.position);
                }

                if (distance < triggerDistance)
                {
                    CustomerManager.Instance.RemoveCustomer(this);
                }
                break;
            default:
                break;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ChangeState(CustomerState newState)
    {
        switch (newState)
        {
            case CustomerState.ToStore:
                SetTarget(ShopManager.Instance.FrontPoint);
                break;
            case CustomerState.EnterStore:
                SetTarget(ShopManager.Instance.EntrancePoint);
                break;
            case CustomerState.SearchItem:
                RandomBuyList();

                state = newState;
                isWaiting = false;
                if (agent != null) agent.isStopped = false;

                if (buyList.Count > 0)
                {
                    currentItemIndex = 0;
                    FindNextShelf();
                }
                else
                {
                    ChangeState(CustomerState.ExitStore);
                }
                //Search Item
                break;
            case CustomerState.ToCashier:
                ShopManager.Instance.CashierDesk.AddCustomer(this);
                CheckQueuePoint();
                break;
            case CustomerState.OnCashier:
                //Drop Item
                //agent.isStopped = true;
                break;
            case CustomerState.ExitStore:
                //agent.isStopped = false; // Mulai jalan lagi
                ShopManager.Instance.CashierDesk.RemoveCustomer(this);
                SetTarget(ShopManager.Instance.FrontPoint);
                CheckQueuePoint(); // Ini akan memicu SortQueue() untuk memajukan antrian berikutnya
                break;
            case CustomerState.ToDespawn:
                SetTarget(CustomerManager.Instance.SpawnPoint);
                break;
            default:
                break;
        }

        state = newState;
    }

    private void FindNextShelf()
    {
        string itemToFind = buyList[currentItemIndex];

        foreach (Shelf shelf in ShopManager.Instance.Shelfs)
        {
            bool hasItem = shelf.Items.Any(i => i.ItemName == itemToFind);

            if (hasItem)
            {
                targetShelf = shelf;
                SetTarget(shelf.transform);
                return;
            }
        }

        Debug.LogWarning($"Item {itemToFind} tidak ditemukan di rak manapun!");
        currentItemIndex++;
        if (currentItemIndex < buyList.Count) FindNextShelf();
        else ChangeState(CustomerState.ToCashier);
    }

    void RandomBuyList()
    {
        buyList.Clear();
        currentItemIndex = 0;

        List<string> availableItems = new List<string>();
        foreach (var shelf in ShopManager.Instance.Shelfs)
        {
            foreach (var item in shelf.Items)
            {
                if (!availableItems.Contains(item.ItemName))
                    availableItems.Add(item.ItemName);
            }
        }

        if (availableItems.Count == 0)
        {
            ChangeState(CustomerState.ExitStore);
            return;
        }

        int amountToBuy = Random.Range(1, 10);

        for (int i = 0; i < amountToBuy; i++)
        {
            string randomItem = availableItems[Random.Range(0, availableItems.Count)];
            buyList.Add(randomItem);
        }
    }

    public void CheckQueuePoint()
    {
        storeEvent.OnQueue?.Invoke();
    }

    public void CheckIfFirstInLine()
    {
        if (state != CustomerState.ToCashier && state != CustomerState.OnCashier) return;

        var cashier = ShopManager.Instance.CashierDesk;

        if (cashier.IsFirstInQueue(this) && !hasDroppedItems)
        {
            distance = Vector3.Distance(transform.position, target.position);
            if (distance < triggerDistance)
            {
                DropItemsToCashier();
            }
        }
    }

    private void DropItemsToCashier()
    {
        hasDroppedItems = true;

        foreach (Item item in itemOnHand)
        {
            item.transform.parent = ShopManager.Instance.CashierDesk.DropPoint;
            item.transform.localPosition = Vector3.zero;
            item.ChangeState(ItemState.Drop);
        }

        // Mulai proses di kasir
        //StartCoroutine(ShopManager.Instance.CashierDesk.ProcessCustomerItems(this, itemOnHand));

        itemOnHand.Clear();
    }
}

[System.Serializable]
public enum CustomerState
{
    ToStore,
    EnterStore,
    SearchItem,
    ToCashier,
    OnCashier,
    ExitStore,
    ToDespawn
}