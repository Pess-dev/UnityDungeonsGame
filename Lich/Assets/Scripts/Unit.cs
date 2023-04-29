using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //transform to set camera to
    public Transform Eye;

    private bool controled = false;

    void Start()
    {

    }

    void Update()
    {

    }

    public void setControl()
    {
        controled = true;
    }
    public void releaseControl()
    {
        controled = false;
    }
}
