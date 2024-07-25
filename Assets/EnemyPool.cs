using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string prefabName;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.prefabName, objectPool);
        }
    }

    public GameObject GetEnemy(string prefabName)
    {
        if (!poolDictionary.ContainsKey(prefabName))
        {
            Debug.LogWarning("Pool with prefab name " + prefabName + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[prefabName].Dequeue();
        objectToSpawn.SetActive(true);
        poolDictionary[prefabName].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public void ReturnEnemy(GameObject obj)
    {
        obj.SetActive(false);

        // Ensure the object is correctly added back to the queue
        string prefabName = obj.name.Replace("(Clone)", "").Trim();
        if (poolDictionary.ContainsKey(prefabName))
        {
            poolDictionary[prefabName].Enqueue(obj);
        }
        else
        {
            Debug.LogWarning("Pool with prefab name " + prefabName + " doesn't exist.");
        }
    }
}
