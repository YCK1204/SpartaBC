using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static T GetComponentInChildren<T>(this GameObject parent, bool recursive = false, string name = null) where T : Component
    {
        return parent.transform.GetChild<T>(recursive, name);
    }
    public static T GetChild<T>(this Transform parent, bool recursive = false, string name = null) where T : Component
    {
        if (recursive)
        {
            var childs = parent.GetComponentsInChildren<T>();
            foreach (var child in childs)
            {
                if (child.transform.name == name || name == null)
                {
                    return child;
                }
            }
        }
        else
        {
            int count = parent.childCount;

            for (int i = 0; i < count; i++)
            {
                Transform child = parent.GetChild(i);
                var component = child.GetComponent<T>();
                if (component != null && (name == null || child.name == name))
                    return component;
            }
        }
        Debug.LogWarning($"Child with name '{name}' not found in {parent.name}");
        return null;
    }
    public static void SetActiveToggle(this GameObject go)
    {
        go.SetActive(!go.activeSelf);
    }
}
