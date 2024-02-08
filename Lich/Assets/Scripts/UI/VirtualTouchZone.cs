using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VirtualTouchZone : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [System.Serializable]
    public class Event : UnityEvent<Vector2> { }

    [Header("Rect Reference")]
    public RectTransform containerRect;

    [Header("Output")]
    public Event touchZoneOutputEvent;

    public Vector2 delta;

    private Vector2 previousPosition;


    public void OnDrag(PointerEventData eventData)
    {
        // Рассчитываем дельту между текущим положением и предыдущим положением в каждом кадре
        delta = eventData.position - previousPosition;

        // Сохраняем текущее положение для использования в следующем кадре
        previousPosition = eventData.position;
        touchZoneOutputEvent.Invoke(delta);
        print(delta);
    }

    public void OnPointerDown(PointerEventData eventData)
    {   
        print("begin drag");
        previousPosition = eventData.position;
    }
}
