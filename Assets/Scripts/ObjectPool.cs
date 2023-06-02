using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]   
    public class Pool
    {
        public string tag;              //Object's name
        public GameObject prefab;       //Object's prefab
        public int size;                //Size of the pool
    }
    public List<Pool> pools;            //Pools is a list of pool
    private Dictionary<string, Queue<GameObject>> _poolDictionary;      //Using string to get the queue of target object

    //single instance
    public static ObjectPool Instance;
    private void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        //Initialize the _poolDictionary
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.name = obj.name.Replace("(Clone)", string.Empty);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            _poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public GameObject GetObject(string tag, Vector3 positon, Quaternion rotation)
    {
        //If try to get an object that doesn't exist in ObjectPool
        if (!_poolDictionary.ContainsKey(tag))
        {
            Debug.Log("Pool: " + tag + " does not exist");
            return null;
        }

        GameObject tempObj = _poolDictionary[tag].Dequeue();
        tempObj.transform.position = positon;
        tempObj.transform.rotation = rotation;
        
        if (tempObj.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
        {
            tempObj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        else
        {
            tempObj.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        
        tempObj.SetActive(true);
        
        return tempObj;
    }

    public void SetObject(string gameObjectName, GameObject gameObject)
    {
        gameObject.SetActive(false);
        _poolDictionary[gameObjectName].Enqueue(gameObject);
    }
}
