using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string prefabName;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

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

    public GameObject GetObject(string prefabName)
    {
        if (!poolDictionary.ContainsKey(prefabName))
        {
            Debug.LogWarning("Pool with prefab name " + prefabName + " doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[prefabName].Dequeue();
        poolDictionary[prefabName].Enqueue(objectToSpawn);
        return objectToSpawn;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }
}
