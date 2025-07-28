using Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        Type = SceneType.Start;

        Manager.Scene.LoadScene("Zep");
    }
}
