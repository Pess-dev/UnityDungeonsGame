using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    //begin and end points to spawn new chunks
    public Transform begin;
    public Transform end;
    
    public Transform playerSpawner;
    public Architect architect;

    public Interactable endInteractable;

    public int remainingChunks = 0;

    private bool nextSpawned = false;

    private void Start()
    {
        endInteractable.interacted.AddListener(NextLevel);
    }

    private void NextChunk()
    {
        if (nextSpawned)
            return;
        if (remainingChunks <= 0)
        {
            return;
        }

        Chunk newChunk = ((GameObject)Instantiate(architect.GetRandomChunk())).GetComponent<Chunk>();
        newChunk.remainingChunks = remainingChunks - 1;
        newChunk.architect = architect;
        MoveBy(newChunk.transform,newChunk.begin,end);
        nextSpawned = true;

    }

    private void MoveBy(Transform obj, Transform by, Transform to)
    {
        obj.rotation = to.rotation * by.localRotation;
        Vector3 positionOffset = to.position - by.position;
        obj.position = obj.position + positionOffset;
    }

    public void NextLevel()
    {
        NextChunk();
        if (remainingChunks > 0)
            return;
        
        architect.NextLevel();
    }
}
