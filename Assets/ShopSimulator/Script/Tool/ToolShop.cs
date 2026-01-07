using System;
using UnityEditor;
using UnityEngine;

public class ToolShop : MonoBehaviour
{
    protected Rigidbody rb;
    protected BoxCollider boxCollider;
    [SerializeField] protected LayerMask targetLayer;

    public LayerMask TargetLayer {  get { return targetLayer; } }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public virtual void Use(GameObject target)
    {

    }

    public virtual void Drop()
    {
        rb.useGravity = true;
    }

    public virtual void Grab(Transform handPoint)
    {
        transform.parent = handPoint;
        transform.localPosition = Vector3.zero;
        transform.rotation = handPoint.rotation;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
    }

    public virtual void InsertToCase(Transform dropPoint)
    {
        transform.parent = dropPoint;
        transform.localPosition = Vector3.zero;
        transform.rotation = dropPoint.rotation;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
    }
}