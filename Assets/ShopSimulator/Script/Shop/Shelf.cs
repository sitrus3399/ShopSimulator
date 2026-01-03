using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [SerializeField] private List<Item> items; 
    [SerializeField] private List<Container> container;
    private int maxItem;

    public List<Item> Items {  get { return items; } }

    private void Start()
    {
        foreach (Container tmpContainer in container)
        {
            tmpContainer.CountMaxItem();

            maxItem += tmpContainer.MaxItem;
        }

        SpawnPreloadItem();
    }

    public void AddItem(Item newItem)
    {
        items.Add(newItem);
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);

        foreach (Container tmpContainer in container)
        {
            tmpContainer.RemoveItem(item);
        }
    }

    void SpawnPreloadItem()
    {
        foreach (Container tmpContainer in container)
        {
            foreach (string tmpItemName in tmpContainer.PreloadAssetString)
            {
                foreach (Item tmpItem in ItemManager.Instance.Items)
                {
                    if (tmpItem.ItemName == tmpItemName)
                    {
                        Item newItem = Instantiate(tmpItem, tmpContainer.transform);
                        items.Add(newItem);
                        tmpContainer.AddItem(newItem);
                    }
                }
            }
        }
    }
}