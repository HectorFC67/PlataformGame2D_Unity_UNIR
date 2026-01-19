using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance { get; private set; }

    [SerializeField] private int startingLivesPerLevel = 3;

    public int Lives { get; private set; }

    private string lastLevelSceneName = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.StartsWith("Level"))
            return;

        if (lastLevelSceneName == null || scene.name != lastLevelSceneName)
        {
            Lives = startingLivesPerLevel;
            lastLevelSceneName = scene.name;
        }
    }

    public void LoseLife(int amount = 1)
    {
        Lives = Mathf.Max(0, Lives - amount);
    }
}
