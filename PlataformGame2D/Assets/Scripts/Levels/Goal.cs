using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Tiempo de espera antes de cambiar de escena (por ejemplo, para SFX/VFX).")]
    [SerializeField] private float loadDelay = 0f;

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

        if (loadDelay > 0f)
            Invoke(nameof(LoadNextLevel), loadDelay);
        else
            LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (!currentSceneName.StartsWith("Level"))
        {
            Debug.LogWarning($"Scene '{currentSceneName}' no sigue el formato 'LevelX'. No se puede calcular el siguiente nivel.");
            return;
        }

        string numberPart = currentSceneName.Replace("Level", "");

        if (!int.TryParse(numberPart, out int currentLevelNumber))
        {
            Debug.LogWarning($"No se pudo leer el número de la escena: '{currentSceneName}'. Asegúrate que sea Level1, Level2, etc.");
            return;
        }

        int nextLevelNumber = currentLevelNumber + 1;
        string nextSceneName = "Level" + nextLevelNumber;

        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning($"No existe la escena '{nextSceneName}' en Build Settings. Puedes cargar un menú o final aquí.");
            // Ejemplo: cargar un menú final si no existe el siguiente nivel:
            // SceneManager.LoadScene("MainMenu");
        }
    }
}
