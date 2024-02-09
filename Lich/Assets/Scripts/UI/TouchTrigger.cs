using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchTrigger : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform handleRect;  
    private CanvasGroup handleCanvasGroup;
    private RectTransform containerRect;

    // Start is called before the first frame update
    void Start()
    {   
        containerRect = GetComponent<RectTransform>();
        if (handleRect.GetComponent<CanvasGroup>() == null)
        handleCanvasGroup = handleRect.gameObject.AddComponent<CanvasGroup>();
        else
        handleCanvasGroup = handleRect.gameObject.GetComponent<CanvasGroup>();
        SetActiveHandle(false);
    }

    public void OnPointerDown(PointerEventData eventData){
        print("down");
        SetActiveHandle(true);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRect, eventData.position, eventData.pressEventCamera, out Vector2 position);
        
       // position = ApplySizeDelta(position);
        
        handleRect.anchoredPosition = position;
    }

    public void OnPointerUp(PointerEventData eventData){
        print("up");
        SetActiveHandle(false);
    }

    public void OnDisable(){
        print("up");
        SetActiveHandle(false);
    }

    // public override void SetActive(bool value){

    // }

    private void SetActiveHandle(bool active){
        handleCanvasGroup.alpha = active ? 1f : 0f;
        handleCanvasGroup.blocksRaycasts = active? true : false;
    }

     Vector2 ApplySizeDelta(Vector2 position)
    {
        float x = (position.x/containerRect.sizeDelta.x);
        float y = (position.y/containerRect.sizeDelta.y);
        return new Vector2(x, y);
    }
}
