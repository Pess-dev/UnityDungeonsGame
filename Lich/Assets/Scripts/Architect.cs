using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Architect : MonoBehaviour
{
    [SerializeField]
    private int currentLevel = 0;

    public float changeDelay = 2f;

    //private List<Object> usedLevelVariants = new List<Object>();

    public List<Level> levels = new List<Level>();
    [System.Serializable]
    public class Level
    {
        public Object StartChunk;
        public Object EndChunk;
        public int ChunkCount = 1;
        public List<Object> ChunkVariants;
    }

    private GameObject currentLevelGameObject = null;

    public Unit playerUnit;

    public UnityEvent end;
    public UnityEvent LevelChangeStart;

    private void Start()
    {
        //player.SetGameplay(true);
        //ToLevel(currentLevel);
    }

    public void NewGame()
    {
        currentLevel = -1;
        NextLevel();
    }

    public void ToLevel(int number)
    {
        currentLevel = number;
        if (currentLevel >= levels.Count)
            return;
        DestroyNonPlayerObjects();

        //currentLevelGameObject = ((GameObject)Instantiate(GetRandomChunk()));
        currentLevelGameObject = ((GameObject)Instantiate(levels[currentLevel].StartChunk)); 
        currentLevelGameObject.GetComponent<Chunk>().architect = this;
        currentLevelGameObject.GetComponent<Chunk>().remainingChunks = levels[currentLevel].ChunkCount;

        //usedLevelVariants.Add(variants[randomLevel]);

        if (playerUnit != null)
        {
            playerUnit.transform.position = currentLevelGameObject.GetComponent<Chunk>().playerSpawner.position;
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
        coroutine = SetLevelCoroutine(currentLevel + 1);
        StartCoroutine(coroutine);
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

        int randomLevel = Random.Range(0, variants.Count);

        return variants[randomLevel];
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
