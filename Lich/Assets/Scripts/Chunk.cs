using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Transform begin;
    public Transform end;
    
    public Transform playerSpawner;
    public Architect architect;

    public Interactable endInteractable;

    public int remainingChunks = 0;

    public GameObject pastChunk = null;

    private bool nextSpawned = false;

    private void Start()
    {
        endInteractable.interacted.AddListener(NextLevel);
    }

    private void NextChunk()
    {
        if (nextSpawned)
            return;
        if (remainingChunks < 0)
        {
            return;
        }
        Object chunkObj;
        if (remainingChunks == 0)
            chunkObj = architect.GetCurrentLevel().EndChunk;
        else 
            chunkObj = architect.GetRandomChunk();

        Chunk newChunk = ((GameObject)Instantiate(chunkObj)).GetComponent<Chunk>();
        newChunk.remainingChunks = remainingChunks - 1;
        newChunk.architect = architect;
        MoveBy(newChunk.transform,newChunk.begin,end);
        nextSpawned = true;

        newChunk.pastChunk = gameObject;

        //???
        if (pastChunk != null)
            Destroy(pastChunk);
    }

    static private void MoveBy(Transform obj, Transform by, Transform to)
    {
        obj.rotation = to.rotation * by.localRotation;
        Vector3 positionOffset = to.position - by.position;
        obj.position = obj.position + positionOffset;
    }

    public void NextLevel()
    {
        NextChunk();
        if (remainingChunks >= 0)
            return;
        
        architect.NextLevel();
    }
}
