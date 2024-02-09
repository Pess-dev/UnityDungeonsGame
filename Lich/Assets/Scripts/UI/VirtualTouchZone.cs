using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class VirtualTouchZone : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{   
    private float tapTimer;

    [Header("Rect Reference")]
    public RectTransform containerRect;

    [Header("Output")]
    public UnityEvent<Vector2> touchZoneOutputEvent;
    public UnityEvent tapOutputEvent;

    public Vector2 delta{get; private set;}

    private Vector2 previousPosition;

    void Update(){
        tapTimer = tapTimer>0?tapTimer-Time.deltaTime:0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Рассчитываем дельту между текущим положением и предыдущим положением в каждом кадре
        delta = eventData.position - previousPosition;

        // Сохраняем текущее положение для использования в следующем кадре
        previousPosition = eventData.position;
        touchZoneOutputEvent.Invoke(delta);
    }

    public void OnPointerDown(PointerEventData eventData)
    {   
        previousPosition = eventData.position;
        tapTimer = InputSystem.settings.defaultTapTime;
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {   
        previousPosition = eventData.position;
        if (tapTimer > 0)
            tapOutputEvent.Invoke();
    }
}
