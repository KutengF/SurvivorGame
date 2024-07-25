using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string prefabName;
        public GameObject prefab;
        public int initialSize;
        public int maxSize;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, List<GameObject>> activeObjectsDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        activeObjectsDictionary = new Dictionary<string, List<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            List<GameObject> activeObjects = new List<GameObject>();

            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.prefabName, objectPool);
            activeObjectsDictionary.Add(pool.prefabName, activeObjects);
        }
    }

    public GameObject GetObject(string prefabName)
    {
        if (!poolDictionary.ContainsKey(prefabName))
        {
            Debug.LogWarning("Pool with prefab name " + prefabName + " doesn't exist.");
            return null;
        }

        Queue<GameObject> objectPool = poolDictionary[prefabName];
        List<GameObject> activeObjects = activeObjectsDictionary[prefabName];

        GameObject obj;
        if (objectPool.Count > 0)
        {
            obj = objectPool.Dequeue();
        }
        else if (activeObjects.Count < pools.Find(pool => pool.prefabName == prefabName).maxSize)
        {
            obj = Instantiate(pools.Find(pool => pool.prefabName == prefabName).prefab);
        }
        else
        {
            obj = activeObjects[0];
            activeObjects.RemoveAt(0);
            obj.SetActive(false);
        }

        obj.SetActive(true);
        activeObjects.Add(obj);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        string prefabName = obj.name.Replace("(Clone)", "").Trim();

        if (poolDictionary.ContainsKey(prefabName))
        {
            poolDictionary[prefabName].Enqueue(obj);
            activeObjectsDictionary[prefabName].Remove(obj);
        }
        else
        {
            Debug.LogWarning("Pool with prefab name " + prefabName + " doesn't exist.");
        }
    }
}
