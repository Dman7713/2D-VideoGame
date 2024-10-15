using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public static string prevScene;
    public static string currentScene;
    public virtual void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
    }

    public void SwitchScene(string SceneName)
    {
        prevScene = currentScene;
        SceneManager.LoadScene(SceneName);
    }
}
