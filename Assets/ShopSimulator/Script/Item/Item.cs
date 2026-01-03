using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private ItemState itemState;
    [SerializeField] private float basicPrice;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider col;
    public string ItemName { get { return itemName; } }

    private void Start()
    {
        ChangeState(ItemState.Drop);
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

                break;
            case ItemState.Bag:
                break;
            default:
                break;
        }
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