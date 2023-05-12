using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLogic : MonoBehaviour
{
    public Transform playerSpawner;
    public Interactable endPortal;
    public Architect architect;

    private void Start()
    {
        endPortal.interacted.AddListener(NextLevel);
    }

    private void NextLevel()
    {
        architect.NextLevel();
    }
}
