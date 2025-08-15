using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToSceneOne()
    {
        var currentScene = SceneManager.GetActiveScene().name;
        SceneManager.UnloadSceneAsync(currentScene);

        // 先加载 RPG 主场景
        SceneManager.LoadScene("Scene_RPG");

        // 再叠加加载 UI 场景
        SceneManager.LoadScene("Scene_UI", LoadSceneMode.Additive);
    }
}
