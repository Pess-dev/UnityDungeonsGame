using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerMoveHandler, IPointerExitHandler
{
    [System.Serializable]
    public class Event : UnityEvent<Vector2> { }
    
    [Header("Rect References")]
    public RectTransform containerRect;
    public RectTransform handleRect;

    [Header("Settings")]
    public float joystickRange = 50f;
    public float magnitudeMultiplier = 1f;
    public bool invertXOutputValue;
    public bool invertYOutputValue;

    [Header("Output")]
    public Event joystickOutputEvent;

    void Start()
    {
        SetupHandle();
    }

    private void SetupHandle()
    {
        if(handleRect)
        {
            UpdateHandleRectPosition(Vector2.zero);
        }
    }
    public void RejectJoystick(){
        OutputPointerEventValue(Vector2.zero);

        if(handleRect)
        {
             UpdateHandleRectPosition(Vector2.zero);
        }
    }
    public void UpdateJoystick(PointerEventData eventData){
        RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRect, eventData.position, eventData.pressEventCamera, out Vector2 position);
        
        position = ApplySizeDelta(position);
        
        Vector2 clampedPosition = ClampValuesToMagnitude(position);

        Vector2 outputPosition = ApplyInversionFilter(position);

        OutputPointerEventValue(outputPosition * magnitudeMultiplier);

        if(handleRect)
        {
            UpdateHandleRectPosition(clampedPosition * joystickRange);
        }
    }

    public void OnPointerDown(PointerEventData eventData){
        UpdateJoystick(eventData);
    }
    public void OnDrag(PointerEventData eventData){
        UpdateJoystick(eventData);
    }

    public void OnPointerMove(PointerEventData eventData){
        UpdateJoystick(eventData);
    }

    public void OnDrop(PointerEventData eventData){
        RejectJoystick();
    }

    public void OnPointerExit(PointerEventData eventData){
        RejectJoystick();
    }

    public void OnPointerUp(PointerEventData eventData){   
        RejectJoystick();
    }




    private void OutputPointerEventValue(Vector2 pointerPosition)
    {
        joystickOutputEvent.Invoke(pointerPosition);
    }

    private void UpdateHandleRectPosition(Vector2 newPosition)
    {
        handleRect.anchoredPosition = newPosition;
    }

    Vector2 ApplySizeDelta(Vector2 position)
    {
        float x = (position.x/containerRect.sizeDelta.x) * 2.5f;
        float y = (position.y/containerRect.sizeDelta.y) * 2.5f;
        return new Vector2(x, y);
    }

    Vector2 ClampValuesToMagnitude(Vector2 position)
    {
        return Vector2.ClampMagnitude(position, 1);
    }

    Vector2 ApplyInversionFilter(Vector2 position)
    {
        if(invertXOutputValue)
        {
            position.x = InvertValue(position.x);
        }

        if(invertYOutputValue)
        {
            position.y = InvertValue(position.y);
        }

        return position;
    }

    float InvertValue(float value)
    {
        return -value;
    }
    
}