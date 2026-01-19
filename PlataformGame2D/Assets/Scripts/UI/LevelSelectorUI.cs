using UnityEngine;

public class LevelSelectorUI : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string menuSceneName = "Menu";

    [Tooltip("Duración del fade.")]
    [SerializeField] private float fadeDuration = 0.6f;

    private bool isLoading;

    public void OnReturnPressed()
    {
        LoadSceneWithFade(menuSceneName);
    }

    public void OnLevelPressed(string levelSceneName)
    {
        LoadSceneWithFade(levelSceneName);
    }

    private void LoadSceneWithFade(string sceneName)
    {
        if (isLoading) return;
        isLoading = true;

        if (SceneFader.Instance == null)
        {
            isLoading = false;
            return;
        }

        SceneFader.Instance.FadeToScene(sceneName, fadeDuration);
    }
}