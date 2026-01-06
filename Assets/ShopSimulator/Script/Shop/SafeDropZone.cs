using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeDropZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.gameObject.transform.position = ShopManager.Instance.CashierDesk.DropPoint.position;
        }
    }
}
