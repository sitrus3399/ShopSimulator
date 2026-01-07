using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBox : ToolShop
{
    [SerializeField] private PlayerEvents playerEvent;
    [SerializeField] private List<Item> items;
    [SerializeField] private List<Transform> itemsPosition;

    public override void Use(GameObject target)
    {
        base.Use(target);

        Container container = target.GetComponent<Container>();

        if (container != null)
        {
            Debug.Log($"Use 2");
            if (items.Count > 0 && container.ItemsCount < container.MaxItem)
            {
                int lastIndex = items.Count - 1;
                Item itemToMove = items[lastIndex];

                container.AddItem(itemToMove);

                items.RemoveAt(lastIndex);
            }
            else if (items.Count <= 0)
            {
                Debug.Log("CardBox sudah habis!");
            }
            else
            {
                Debug.Log("Container sudah penuh!");
            }
        }
        else
        {
            Debug.Log($"Use 3");
            Drop();
        }
    }

    public override void Drop()
    {
        Debug.Log("Drop");
        base.Drop();
        boxCollider.isTrigger = false;
        rb.isKinematic = false;

        transform.parent = null;
        rb.useGravity = true;
    }

    public override void Grab(Transform handPoint)
    {
        Debug.Log("Grab");
        boxCollider.isTrigger = true;
        rb.isKinematic = true;
        base.Grab(handPoint);

        if (handPoint == null) return;
        
        PlayerController controller = handPoint.GetComponentInParent<PlayerController>();
        Transform frontPoint = controller.FrontPoint;
        transform.parent = frontPoint;
        transform.localPosition = Vector3.zero;
        transform.rotation = frontPoint.rotation;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
    }
}
