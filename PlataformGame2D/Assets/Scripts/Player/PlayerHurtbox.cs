using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerHurtbox : MonoBehaviour
{
    [SerializeField] private PlayerLifes playerLifes;

    private void Awake()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;

        if (playerLifes == null)
            playerLifes = GetComponentInParent<PlayerLifes>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerLifes == null) return;

        playerLifes.NotifyEnemyHit(other.gameObject);
    }
}
