using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutScene : MonoBehaviour
{
    public UnityEvent end;
    public void EndInvoke()
    {
        end.Invoke();
    }
}
