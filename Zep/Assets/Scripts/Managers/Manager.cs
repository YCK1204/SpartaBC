using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : BaseMonobehaviour
{
    static Manager _instance;
    public static Manager Instance { get { return _instance; } }

    ResourceManager _resource = new ResourceManager();
    public static ResourceManager Resource { get { return Instance._resource; } }
    SceneManagerEx _scene = new SceneManagerEx();
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    BaseGameManager _game;
    public static BaseGameManager Game { get { return Instance._game; } set { Instance._game = value; } }
    void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        Screen.SetResolution(640, 840, false);
    }
}
