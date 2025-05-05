using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button clicked at position: " + Input.mousePosition);
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            var raycastResults = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);
            foreach (var result in raycastResults)
            {
                Debug.Log("Raycast hit: " + result.gameObject.name);
            }
        }
    }
}