using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using Unity.VisualScripting;

public class Architect : MonoBehaviour
{
    [SerializeField]
    private int firstLevel = 0;

    public int currentLevel = 0;

    public float changeDelay = 2f;

    public List<Level> levels = new List<Level>();

    [System.Serializable]
    public class Level
    {
        public Object StartChunk;
        public Object EndChunk;
        public int ChunkCount = 1;
        public List<Object> ChunkVariants;
    }
    
    private List<Object> usedChunks = new List<Object>();

    private Chunk currentLevelGameObject = null;

    public Unit playerUnit;

    public UnityEvent end;
    public UnityEvent LevelChangeStart;

    public void NewGame()
    {
        currentLevel = firstLevel - 1;
        NextLevel();
    }

    public void StartLevel(int level)
    {
        //StopCoroutine(coroutine);
        coroutine = SetLevelCoroutine(level);
        StartCoroutine(coroutine);
    }

    public void ToLevel(int number)
    {
        currentLevel = number;
        if (currentLevel >= levels.Count)
            return;
        DestroyNonPlayerObjects();

        usedChunks.Clear();

        currentLevelGameObject = Instantiate(levels[currentLevel].StartChunk).GetComponent<Chunk>(); 
        currentLevelGameObject.architect = this;
        currentLevelGameObject.remainingChunks = levels[currentLevel].ChunkCount;

        if (playerUnit != null)
        {
            //playerUnit.transform.SetPositionAndRotation(currentLevelGameObject.playerSpawner.position, currentLevelGameObject.playerSpawner.rotation);
            playerUnit.GetComponent<Rigidbody>().MovePosition(currentLevelGameObject.playerSpawner.position);
            playerUnit.GetComponent<Rigidbody>().MoveRotation(currentLevelGameObject.playerSpawner.rotation);
            playerUnit.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    private IEnumerator coroutine;

    public void NextLevel()
    {
        if (currentLevel + 1 >= levels.Count)
        {
            end.Invoke();
            return;
        }
        
        StartLevel(currentLevel + 1);
    }

    private IEnumerator SetLevelCoroutine(int number)
    {
        LevelChangeStart.Invoke();
        yield return new WaitForSeconds(changeDelay / 2);
        ToLevel(number);
        yield return new WaitForSeconds(changeDelay / 2);
    }


    public Object GetRandomChunk()
    {
        List<Object> variants = new List<Object>(levels[currentLevel].ChunkVariants);
        List<Object> newVariants = new List<Object>();
        foreach (Object obj in variants){
            if (!usedChunks.Contains(obj))
                newVariants.Add(obj);
        }


        int randomLevel = Random.Range(0, newVariants.Count);

        usedChunks.Add(newVariants[randomLevel]);
        return newVariants[randomLevel];
    }


    public void DestroyNonPlayerObjects()
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            if (obj == gameObject)
                continue;
            if (obj.GetComponent<Unit>() != null)
                if (obj.GetComponent<Unit>() == playerUnit)
                    continue;
            if (obj.GetComponent<Item>() != null)
                if (obj.GetComponent<Item>().GetUser() == playerUnit)
                    continue;
            if (obj.tag == "Player")
                continue;
            Destroy(obj);
        }
    }

    public Level GetCurrentLevel()
    {
        return levels[currentLevel];
    }
}
