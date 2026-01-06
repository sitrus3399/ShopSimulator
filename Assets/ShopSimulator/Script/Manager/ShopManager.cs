using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private float cash;
    [SerializeField] private List<Shelf> shelfs;

    [SerializeField] private Cashier cashierDesk;
    [SerializeField] private Transform frontPoint;
    [SerializeField] private Transform entrancePoint;
    [SerializeField] private Transform bagPoint;

    public float Cash {  get { return cash; } }
    public List<Shelf> Shelfs {  get { return shelfs; } }
    public Cashier CashierDesk { get { return cashierDesk; } }
    public Transform FrontPoint { get { return frontPoint; } }
    public Transform EntrancePoint { get { return entrancePoint; } }
    public Transform BagPoint { get { return bagPoint; } }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}