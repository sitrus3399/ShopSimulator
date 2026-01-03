using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] private float cash;
    [SerializeField] private List<Shelf> shelfs;

    [SerializeField] private CashierDesk cashierDeesk;
    [SerializeField] private Transform frontPoint;
    [SerializeField] private Transform entrancePoint;

    public float Cash {  get { return cash; } }
    public List<Shelf> Shelfs {  get { return shelfs; } }
    public CashierDesk CashierDesk { get { return cashierDeesk; } }
    public Transform FrontPoint { get { return frontPoint; } }
    public Transform EntrancePoint { get { return entrancePoint; } }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}