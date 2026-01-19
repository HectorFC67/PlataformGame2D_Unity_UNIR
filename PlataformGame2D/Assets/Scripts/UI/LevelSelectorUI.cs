using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorUI : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string menuSceneName = "Menu";

    [Tooltip("Duración del fade si existe SceneFader.")]
    [SerializeField] private float fadeDuration = 0.6f;

    private bool isLoading;

    public void OnReturnPressed()
    {
        LoadScene(menuSceneName);
    }

    public void OnLevelPressed(string levelSceneName)
    {
        LoadScene(levelSceneName);
    }

    private void LoadScene(string sceneName)
    {
        if (isLoading) return;
        isLoading = true;

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(sceneName, fadeDuration);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
