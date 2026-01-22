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
        // Volvemos al gameplay: aquí sí tiene sentido bloquear el cursor si tu juego lo requiere
        SetPaused(false);
        ApplyCursorForGameplay();
    }

    public void ExitToLevelSelect()
    {
        // 1) Reanuda el tiempo (importante para la siguiente escena)
        Time.timeScale = 1f;

        // 2) Oculta UI de pausa
        _isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // 3) Reactiva scripts del jugador (por si vuelves a jugar más tarde)
        if (playerControlScripts != null)
        {
            foreach (var b in playerControlScripts)
            {
                if (b != null) b.enabled = true;
            }
        }

        // 4) MUY IMPORTANTE: dejar el cursor en modo UI para el selector de niveles
        ApplyCursorForUI();

        // 5) Fade + cambio de escena
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

        // En pausa queremos cursor para UI
        if (_isPaused) ApplyCursorForUI();

        // Control del jugador
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
