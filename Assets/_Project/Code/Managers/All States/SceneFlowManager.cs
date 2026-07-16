using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFlowManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.OnSceneLoadRequested += LoadScene;
    }

    private void OnDisable()
    {
        GameEvents.OnSceneLoadRequested -= LoadScene;
    }

    private void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
