using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableButton2D : MonoBehaviour, IPointerClickHandler
{
    public string nameButton;

    public Action OnButtonClicked;

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(nameButton);
        OnButtonClicked?.Invoke();
    }
}