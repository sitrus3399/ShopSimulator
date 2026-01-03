using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    [SerializeField] private string containerType;
    [SerializeField] private List<Item> items;
    [SerializeField] private List<Transform> itemLocation;
    [SerializeField] private List<string> preloadAssetString;
    private int maxItem;

    public int MaxItem {  get { return maxItem; } }
    public List<string> PreloadAssetString { get { return preloadAssetString; } }

    public void CountMaxItem()
    {
        maxItem = itemLocation.Count;
    }

    public void AddItem(Item tmpItem)
    {
        items.Add(tmpItem);
        tmpItem.gameObject.transform.position = itemLocation[items.Count - 1].position;
    }

    public void RemoveItem(Item tmpItem)
    {
        items.Remove(tmpItem);
    }
}
