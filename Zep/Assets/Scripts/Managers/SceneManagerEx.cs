using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerEx
{
    public BaseScene CurrentScene { get; set; }
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        // Logic to load a scene by name
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
    public void LoadSceneAsync(string sceneName)
    {
        var oper = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        
    }
}
