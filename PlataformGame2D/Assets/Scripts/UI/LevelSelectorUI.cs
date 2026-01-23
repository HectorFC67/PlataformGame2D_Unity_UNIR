using System.Collections;
using UnityEngine;

public class LevelSelectorUI : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string menuSceneName = "Menu";

    [Tooltip("Duración del fade.")]
    [SerializeField] private float fadeDuration = 0.6f;

    [Header("Work In Progress Panel")]
    [SerializeField] private GameObject workInProgressPanel;
    [SerializeField] private float wipVisibleSeconds = 2.5f;

    private bool isLoading;
    private Coroutine wipRoutine;

    public void OnReturnPressed()
    {
        LoadSceneWithFade(menuSceneName);
    }

    public void OnLevelPressed(string levelSceneName)
    {
        LoadSceneWithFade(levelSceneName);
    }

    public void OnWorkInProgressPressed()
    {
        if (workInProgressPanel == null) return;

        if (wipRoutine != null) StopCoroutine(wipRoutine);
        wipRoutine = StartCoroutine(ShowWipTemporarily());
    }

    private IEnumerator ShowWipTemporarily()
    {
        workInProgressPanel.SetActive(true);
        yield return new WaitForSeconds(wipVisibleSeconds);
        workInProgressPanel.SetActive(false);
        wipRoutine = null;
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
