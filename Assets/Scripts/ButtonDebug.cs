using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDebug : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Mouse hovered over button!");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Button clicked!");
    }
}