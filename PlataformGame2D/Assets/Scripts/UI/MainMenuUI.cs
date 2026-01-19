using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Scene Loading")]
    [SerializeField] private string levelSelectorSceneName = "LevelSelector";
    [SerializeField] private float fadeDuration = 0.6f;

    private bool isLoading;

    private void Start()
    {
        ShowMain();
    }

    public void OnPlayPressed()
    {
        LoadSceneWithFade(levelSelectorSceneName);
    }

    public void OnControlsPressed()
    {
        SetActivePanel(mainMenuPanel: false, controls: true, options: false);
    }

    public void OnOptionsPressed()
    {
        SetActivePanel(mainMenuPanel: false, controls: false, options: true);
    }

    public void OnExitPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnReturnFromControls()
    {
        ShowMain();
    }

    public void OnReturnFromOptions()
    {
        ShowMain();
    }

    private void ShowMain()
    {
        SetActivePanel(mainMenuPanel: true, controls: false, options: false);
    }

    private void SetActivePanel(bool mainMenuPanel, bool controls, bool options)
    {
        if (this.mainMenuPanel != null) this.mainMenuPanel.SetActive(mainMenuPanel);
        if (controlsPanel != null) controlsPanel.SetActive(controls);
        if (optionsPanel != null) optionsPanel.SetActive(options);
    }

    private void LoadSceneWithFade(string sceneName)
    {
        if (isLoading) return;
        isLoading = true;

        if (SceneFader.Instance == null)
        {
            Debug.LogError($"SceneFader.Instance es null. Añade SceneFader a la escena inicial (Menu) para que persista. No se cargó '{sceneName}'.");
            isLoading = false;
            return;
        }

        SceneFader.Instance.FadeToScene(sceneName, fadeDuration);
    }
}
