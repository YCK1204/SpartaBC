using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    string _prefabPath = "Prefabs";
    public T Load<T>(string path) where T : Object
    {
        T resource = Resources.Load<T>(path);
        if (resource == null)
        {
            Debug.LogError($"Resource not found at path: {path}");
        }
        return resource;
    }
    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"{_prefabPath}/{path}");
        if (prefab == null)
        {
            return null;
        }
        GameObject instance = Object.Instantiate(prefab, parent);
        instance.name = prefab.name;
        return instance;
    }
}
