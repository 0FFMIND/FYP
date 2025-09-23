using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Manager
{
    public class SceneMgr : SingletonMB<SceneMgr>
    {
        public void DisableScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
            {
                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    root.SetActive(false);
                }
            }
        }

        public void EnableScene(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
            {
                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    root.SetActive(true);
                }
            }
        }

        public void LoadScenesAdditive(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                Scene scene = SceneManager.GetSceneByName(sceneName);
                if (!scene.isLoaded)
                {
                    SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                }
            }
        }
    }
}
