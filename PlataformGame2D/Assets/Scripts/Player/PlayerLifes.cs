using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class PlayerLifes : MonoBehaviour
{
    [Header("Damage Filter")]
    [SerializeField] private bool useLayerMask = true;
    [SerializeField] private LayerMask enemyDamageLayers;

    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private string deadzoneTag = "Deadzone";

    [SerializeField] private int damagePerHit = 1;

    [Header("Animator")]
    [SerializeField] private Animator animator;

    [SerializeField] private string damageBool = "recibeDanio";
    [SerializeField] private string deadBool = "muerto";

    [SerializeField] private string damageStateName = "damage";
    [SerializeField] private string dieStateName = "die";

    [Header("Scene Names")]
    [SerializeField] private string gameOverSceneName = "GameOver";

    [Header("Transitions")]
    [SerializeField] private float fadeDuration = 0.6f;

    [Header("Lock Player Controls")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;
    [SerializeField] private bool disableRigidbodySimulation = true;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip damageSfx;

    private bool hitLocked;
    private Rigidbody2D rb;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody2D>();

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    public void NotifyEnemyHit(GameObject other)
    {
        if (hitLocked) return;
        if (!IsValidDamageSource(other)) return;

        TakeHit();
    }

    private bool IsValidDamageSource(GameObject obj)
    {
        if (obj == null) return false;

        if (useLayerMask)
            return (enemyDamageLayers.value & (1 << obj.layer)) != 0;

        return obj.CompareTag(enemyTag) || obj.CompareTag(deadzoneTag);
    }

    private void TakeHit()
    {
        hitLocked = true;

        if (LifeManager.Instance == null)
        {
            Debug.LogError("LifeManager no está en escena. Añádelo en MainMenu o escena inicial.");
            hitLocked = false;
            return;
        }

        PlayOneShot(damageSfx);

        LockPlayer();

        LifeManager.Instance.LoseLife(damagePerHit);
        bool outOfLives = (LifeManager.Instance.Lives <= 0);

        StopAllCoroutines();
        StartCoroutine(HitFlow(outOfLives));
    }

    private void LockPlayer()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;

            if (disableRigidbodySimulation)
                rb.simulated = false;
        }

        if (scriptsToDisable != null)
        {
            for (int i = 0; i < scriptsToDisable.Length; i++)
            {
                if (scriptsToDisable[i] != null)
                    scriptsToDisable[i].enabled = false;
            }
        }
    }

    private IEnumerator HitFlow(bool outOfLives)
    {
        if (animator != null)
            animator.SetBool(damageBool, true);

        while (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName(damageStateName))
            yield return null;

        while (animator != null &&
               animator.GetCurrentAnimatorStateInfo(0).IsName(damageStateName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        if (animator != null)
            animator.SetBool(damageBool, false);

        if (!outOfLives)
        {
            ReloadCurrentLevel();
            yield break;
        }

        if (animator != null)
            animator.SetBool(deadBool, true);

        while (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName(dieStateName))
            yield return null;

        while (animator != null &&
               animator.GetCurrentAnimatorStateInfo(0).IsName(dieStateName) &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        LoadGameOver();
    }

    private void ReloadCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(sceneName, fadeDuration);
        else
            SceneManager.LoadScene(sceneName);
    }

    private void LoadGameOver()
    {
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene(gameOverSceneName, fadeDuration);
        else
            SceneManager.LoadScene(gameOverSceneName);
    }

    private void PlayOneShot(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }
}