using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Item : MonoBehaviour
{
    [SerializeField] private StoreEvents storeEvent;

    [SerializeField] private string itemName;
    [SerializeField] private ItemState itemState;
    [SerializeField] private float basicPrice;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider col;

    [Header ("Bag")]
    [SerializeField] private Transform targetBag;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;
    [SerializeField] private bool onBag;
    [SerializeField] private bool onCashier;

    public string ItemName { get { return itemName; } }

    private void Start()
    {
        ChangeState(itemState);
        onCashier = false;
    }

    private void Update()
    {
        if (onBag)
        {
            float distance = Vector3.Distance(transform.position, targetBag.position);

            if (distance > stoppingDistance)
            {
                Vector3 direction = (targetBag.position - transform.position).normalized;

                rb.velocity = direction * speed;
            }
            else
            {
                rb.velocity = Vector3.zero;
                storeEvent.OnItemRegister(basicPrice, this);
                gameObject.SetActive(false);
            }
        }
    }

    public void ChangeState(ItemState newState)
    {
        itemState = newState;

        switch (itemState)
        {
            case ItemState.Drop:
                rb.isKinematic = false;
                rb.useGravity = true;
                col.isTrigger = false;
                break;
            case ItemState.Take:
                rb.isKinematic = true;
                rb.useGravity = false;
                col.isTrigger = true;
                break;
            case ItemState.Scan:
                onBag = true;
                targetBag = ShopManager.Instance.BagPoint;
                break;
            case ItemState.Bag:
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bucket"))
        {
            if (itemState == ItemState.Drop)
            {
                rb.velocity = Vector3.zero;
                transform.position = ShopManager.Instance.CashierDesk.DropPoint.position;
            }
            else if (itemState == ItemState.Scan)
            {
                storeEvent.OnItemRegister(basicPrice, this);
                gameObject.SetActive(false);
            }            
        }
    }

    public void SetOnCashier(bool value)
    {
        onCashier = value;
    }
}

[System.Serializable]
public enum ItemState
{
    Drop,
    Take,
    Scan,
    Bag
}