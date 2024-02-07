using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class TouchArea : MonoBehaviour
{
    private Rect rect;
    
    public Vector2 delta {get; private set;}

    public UnityEvent<Vector2> deltaUpdated;

    void Start(){
        rect = GetComponent<RectTransform>().rect;
    }

    void Update(){
        UpdateTouchDelta();
    }

    private void UpdateTouchDelta(){
        delta = Vector2.zero;
        foreach (Touch touch in Input.touches){
            if (rect.Contains(touch.position)){
                delta = touch.deltaPosition;
                print("contact "+ touch);
            }
        }
        deltaUpdated.Invoke(delta);
    } 
    
}
