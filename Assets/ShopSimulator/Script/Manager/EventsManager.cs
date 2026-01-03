using UnityEngine;

public class EventsManager : Singleton<EventsManager>
{
    [Header("Events")]
    [SerializeField] private GameEvents gameEvents;
    [SerializeField] private StoreEvents storeEvents;
}