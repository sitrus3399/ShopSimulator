using System.Collections.Generic;
using UnityEngine;

public class ItemManager : Singleton<ItemManager>
{
    [SerializeField] private List<Item> items;

    public List<Item> Items {  get { return items; } }
}