using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StoreEvents", menuName = "Events/Store", order = 0)]
public class StoreEvents : ScriptableObject
{
    public Action OnQueue;
    public Action OnBuy;
}
