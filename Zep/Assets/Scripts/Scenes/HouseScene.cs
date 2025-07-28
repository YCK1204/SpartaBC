using Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        Type = SceneType.House;
    }
    public void LoadZep()
    {
        Manager.Scene.LoadScene("Zep");
    }
}
