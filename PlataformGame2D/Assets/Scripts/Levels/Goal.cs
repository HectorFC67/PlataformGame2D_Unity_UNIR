using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Tiempo de espera antes de iniciar la transición (SFX/VFX).")]
    [SerializeField] private float loadDelay = 0f;

    [Tooltip("Duración del fade al cambiar de escena.")]
    [SerializeField] private float fadeDuration = 0.6f;

    private bool hasTriggered = false;

    private void Reset()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag(playerTag)) return;

        hasTriggered = true;

        string nextSceneName = GetNextLevelSceneName();
        if (string.IsNullOrEmpty(nextSceneName)) return;

        if (loadDelay > 0f)
            Invoke(nameof(TriggerLoadNext), loadDelay);
        else
            TriggerLoadNext();
    }

    private void TriggerLoadNext()
    {
        string nextSceneName = GetNextLevelSceneName();
        if (string.IsNullOrEmpty(nextSceneName)) return;

        if (SceneFader.Instance != null)
        {
            SceneFader.Instance.FadeToScene(nextSceneName, fadeDuration);
        }
        else
        {
            Debug.LogWarning("No SceneFader found. Loading scene without fade.");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private string GetNextLevelSceneName()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (!currentSceneName.StartsWith("Level"))
        {
            Debug.LogWarning($"Scene '{currentSceneName}' no sigue el formato 'LevelX'.");
            return null;
        }

        string numberPart = currentSceneName.Replace("Level", "");

        if (!int.TryParse(numberPart, out int currentLevelNumber))
        {
            Debug.LogWarning($"No se pudo leer el número de '{currentSceneName}'. Ej: Level1, Level2...");
            return null;
        }

        string nextSceneName = "Level" + (currentLevelNumber + 1);

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogWarning($"No existe '{nextSceneName}' en Build Settings. Puedes mandar al menú final aquí.");
            return null;
        }

        return nextSceneName;
    }
}
