using Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseScene : MonoBehaviour
{
    [HideInInspector]
    public SceneType Type;
    void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        Manager.Scene.CurrentScene = this;
    }
}
