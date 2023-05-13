using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Architect : MonoBehaviour
{
    [SerializeField]
    private int currentLevel = 0;

    [SerializeField]
    private float spawnPlayerDelay = 1f;

    private float timer = 0f;

    //private List<Object> usedLevelVariants = new List<Object>();

    public List<ListWrapper> Levels = new List<ListWrapper>();
    [System.Serializable]
    public class ListWrapper
    {
        public Object StartChunk;
        public Object EndChunk;
        public int ChunkCount = 1;
        public List<Object> ChunkVariants;
    }

    private GameObject currentLevelGameObject = null;

    public PlayerControl player;
    
    private void Start()
    {
        player.SetGameplay(true);
        ToLevel(currentLevel);
    }


    public void ToLevel(int number)
    {
        currentLevel = number;
        if (currentLevel >= Levels.Count)
            return;
        DestroyNonPlayerObjects();

        //currentLevelGameObject = ((GameObject)Instantiate(GetRandomChunk()));
        currentLevelGameObject = ((GameObject)Instantiate(Levels[currentLevel].StartChunk)); 
        currentLevelGameObject.GetComponent<Chunk>().architect = this;
        currentLevelGameObject.GetComponent<Chunk>().remainingChunks = Levels[currentLevel].ChunkCount;

        //usedLevelVariants.Add(variants[randomLevel]);

        if (player.unit != null)
        {
            player.unit.transform.position = currentLevelGameObject.GetComponent<Chunk>().playerSpawner.position;
            player.unit.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    private IEnumerator coroutine;

    public void NextLevel()
    {
        if (currentLevel + 1 >= Levels.Count)
        {
            Debug.Log("Final for now");
            return;
        }
        timer = spawnPlayerDelay;
        coroutine = SetLevelCoroutine(currentLevel + 1);
        StartCoroutine(coroutine);
    }

    private IEnumerator SetLevelCoroutine(int number)
    {
        player.SetGameplay(false);
        yield return new WaitForSeconds(spawnPlayerDelay / 2);
        ToLevel(currentLevel + 1);
        yield return new WaitForSeconds(spawnPlayerDelay / 2);
        player.SetGameplay(true);
    }


    public Object GetRandomChunk()
    {
        List<Object> variants = new List<Object>(Levels[currentLevel].ChunkVariants);

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
                if (obj.GetComponent<Unit>() == player.unit)
                    continue;
            if (obj.GetComponent<Item>() != null)
                if (obj.GetComponent<Item>().GetUser() == player.unit)
                    continue;
            if (obj.tag == "Player")
                continue;

            Destroy(obj);
        }
    }
}
