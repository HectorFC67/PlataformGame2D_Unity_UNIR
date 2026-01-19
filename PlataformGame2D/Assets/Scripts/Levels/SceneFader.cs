using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Image fadeImage;

    [Header("Fade Settings")]
    [SerializeField] private float defaultFadeDuration = 0.5f;

    private bool isTransitioning;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Instance = null;
    }

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (fadeImage == null)
        {
            return;
        }

        SetAlpha(1f);
        fadeImage.raycastTarget = true;

        StartCoroutine(FadeIn(defaultFadeDuration));
    }

    public void FadeToScene(string sceneName, float fadeDuration = -1f)
    {
        if (isTransitioning) return;

        if (fadeImage == null)
        {
            return;
        }

        float dur = fadeDuration > 0f ? fadeDuration : defaultFadeDuration;
        StartCoroutine(FadeAndSwitchScene(sceneName, dur));
    }

    private IEnumerator FadeAndSwitchScene(string sceneName, float duration)
    {
        isTransitioning = true;

        yield return FadeOut(duration);

        SceneManager.LoadScene(sceneName);

        yield return null;

        yield return FadeIn(duration);

        isTransitioning = false;
    }

    private IEnumerator FadeOut(float duration)
    {
        fadeImage.raycastTarget = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(Mathf.Clamp01(t / duration));
            yield return null;
        }

        SetAlpha(1f);
    }

    private IEnumerator FadeIn(float duration)
    {
        fadeImage.raycastTarget = true;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            SetAlpha(1f - Mathf.Clamp01(t / duration));
            yield return null;
        }

        SetAlpha(0f);
        fadeImage.raycastTarget = false;
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}