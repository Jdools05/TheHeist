using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Dynamic;

public class JoyButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector]
    public bool Pressed;
    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }
}
