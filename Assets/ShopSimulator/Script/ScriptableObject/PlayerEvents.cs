using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerEvents", menuName = "Events/Player", order = 0)]
public class PlayerEvents : ScriptableObject
{
    public Action OnInteract;
    public Action<EDCMachine> OnUseEDC;
    public Action<Vector2> OnRotate;
    public Action<Vector3> OnMove;

    public void Interact()
    {
        OnInteract?.Invoke();
    }

    public void UseEDC(EDCMachine edcMachine)
    {
        OnUseEDC?.Invoke(edcMachine);
    }

    public void Rotate(Vector2 value)
    {
        OnRotate?.Invoke(value);

    }

    public void Move(Vector3 value)
    {
        OnMove?.Invoke(value);
    }
}