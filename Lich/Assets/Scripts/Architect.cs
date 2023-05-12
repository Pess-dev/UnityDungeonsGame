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

    public List<ListWrapper> Levels = new List<ListWrapper>();
    [System.Serializable]
    public class ListWrapper
    {
        public List<Object> LevelVariants;
    }

    private GameObject currentLevelGameObject = null;

    public PlayerControl player;
    
    private void Start()
    {
        ToLevel(currentLevel);
    }

    public void ToLevel(int number)
    {
        DestroyNonPlayerObjects();

        int randomLevel = Random.Range(0, Levels[number].LevelVariants.Count);
        currentLevelGameObject = ((GameObject)Instantiate(Levels[number].LevelVariants[randomLevel]));
        currentLevelGameObject.GetComponent<LevelLogic>().architect = this;
        if (player.unit != null)
        {
            player.unit.transform.position = currentLevelGameObject.GetComponent<LevelLogic>().playerSpawner.position;
            player.unit.GetComponent<Rigidbody>().velocity = Vector3.zero;
            
        }
    }

    public void NextLevel()
    {
        if (currentLevel+1 > Levels.Count)
        {
            //final
            //
            //
            Debug.Log("Final for now");
            //
            //

            return;
        }

        ToLevel(currentLevel+1);
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
