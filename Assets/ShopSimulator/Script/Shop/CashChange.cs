using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashChange : MonoBehaviour
{
    [SerializeField] private StoreEvents storeEvents;
    [SerializeField] private bool onBox;

    [SerializeField] private float valueChange;
    [SerializeField] private Rigidbody rb;

    public float ValueChange { get { return valueChange; } }

    public void Take()
    {
        if (onBox)
        {
            storeEvents.AddChange(this);
        }
        else
        {
            storeEvents.RemoveChange(this);
        }
    }

    public void ChangeOnBox(bool value)
    {
        onBox = value;

        rb.useGravity = !onBox;
        rb.isKinematic = onBox;
    }
}