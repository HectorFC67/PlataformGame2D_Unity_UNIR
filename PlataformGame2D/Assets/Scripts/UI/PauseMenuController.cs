using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [Header("Input System")]
    [SerializeField] private InputActionReference pauseAction;

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Disable Player Control While Paused")]
    [SerializeField] private Behaviour[] playerControlScripts;

    [Header("Exit")]
    [SerializeField] private string levelSelectSceneName = "LevelSelector";
    [SerializeField] private float exitFadeDuration = 0.5f;

    private bool _isPaused;

    private void Awake()
    {
        SetPaused(false, force: true);
    }

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed += OnPausePerformed;
            pauseAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPausePerformed;
            pauseAction.action.Disable();
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        SetPaused(!_isPaused);
    }

    public void Resume()
    {
        SetPaused(false);
        ApplyCursorForGameplay();
    }

    public void ExitToLevelSelect()
    {
        Time.timeScale = 1f;

        _isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (playerControlScripts != null)
        {
            foreach (var b in playerControlScripts)
            {
                if (b != null) b.enabled = true;
            }
        }

        ApplyCursorForUI();

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(levelSelectSceneName, exitFadeDuration);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelSelectSceneName);
        }
    }

    private void SetPaused(bool paused, bool force = false)
    {
        if (!force && paused == _isPaused) return;
        _isPaused = paused;

        if (pausePanel != null)
            pausePanel.SetActive(_isPaused);

        Time.timeScale = _isPaused ? 0f : 1f;

        if (_isPaused) ApplyCursorForUI();

        if (playerControlScripts != null)
        {
            foreach (var b in playerControlScripts)
            {
                if (b != null) b.enabled = !_isPaused;
            }
        }
    }

    private void ApplyCursorForUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void ApplyCursorForGameplay()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
