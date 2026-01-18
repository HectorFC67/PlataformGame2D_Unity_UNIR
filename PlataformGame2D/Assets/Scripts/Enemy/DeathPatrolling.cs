using UnityEngine;

namespace EnemyPatrolling
{
    public class DeathPatrolling : MonoBehaviour
    {
        private bool isDead = false;

        [SerializeField] private AudioClip deathClip;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private ParticleSystem deathParticle;

        private Collider2D col;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryDieFromSwordTrigger(other);
        }

        private void TryDieFromSwordTrigger(Collider2D other)
        {
            Debug.Log("Trigger Hit: " + other.gameObject.name + " Tag: " + other.gameObject.tag);

            if (isDead) return;

            if (other.CompareTag("Sword"))
            {
                isDead = true;

                if (audioSource != null && deathClip != null)
                    audioSource.PlayOneShot(deathClip);

                CreateDeathParticle();

                if (col != null) col.enabled = false;
                if (spriteRenderer != null) spriteRenderer.enabled = false;

                Destroy(gameObject, 0.5f);
            }
        }

        private void CreateDeathParticle()
        {
            if (deathParticle != null)
            {
                Instantiate(deathParticle, transform.position, transform.rotation).Play();
            }
        }
    }
}
