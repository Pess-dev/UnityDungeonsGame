using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool randomYRotation = true;

    public List<PrefabProbability> objectsVariants = new List<PrefabProbability>();

    [System.Serializable]
    public class PrefabProbability
    {
        public Object prefab;
        public float probability;
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        normalizeProbability();
        Spawn();
        Destroy(gameObject);
    }

    private void Spawn()
    {
        float random = Random.value;
        float sum = 0f;
        
        foreach (PrefabProbability pp in objectsVariants)
        {
            if (random <= sum + pp.probability && random >= sum )
            {
                if (pp.prefab == null)
                {
                    return;
                }
                Quaternion rot = Quaternion.LookRotation(transform.forward-Vector3.up*transform.forward.y,Vector3.up);
                if (randomYRotation)
                    rot *= Quaternion.AngleAxis(Random.Range(0f,360f), Vector3.up);
                Instantiate(pp.prefab, transform.position, rot);
                return;
            }
            sum += pp.probability; 
        }
    }

    private void normalizeProbability()
    {
        float sum = 0f;

        foreach (PrefabProbability pp in objectsVariants)
        {
            sum += pp.probability;
        }

        if (sum == 0f)
            return;

        foreach (PrefabProbability pp in objectsVariants)
        {
            pp.probability /= sum;
        }
    }
}
