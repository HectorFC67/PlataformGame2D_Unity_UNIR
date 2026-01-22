using UnityEngine;

public class PlatformCarrier2D : MonoBehaviour
{
    [Header("Tag")]
    [SerializeField] private string playerTag = "Player";

    [Header("Detection")]
    [SerializeField] private Vector2 boxSize = new Vector2(1.0f, 0.2f);
    [SerializeField] private Vector2 boxOffset = new Vector2(0f, 0.6f);

    private MovingPlatform2D platform;

    private void Awake()
    {
        platform = GetComponent<MovingPlatform2D>();
    }

    private void FixedUpdate()
    {
        Vector2 origin = (Vector2)transform.position + boxOffset;

        Collider2D[] hits = Physics2D.OverlapBoxAll(origin, boxSize, 0f);

        if (hits == null || hits.Length == 0) return;

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];
            if (col == null) continue;

            if (!IsPlayer(col)) continue;

            Rigidbody2D playerRb = col.attachedRigidbody;
            if (playerRb == null) continue;

            Vector2 d = platform.FrameDelta;

            if (d.y > 0f) d.y = 0f;

            playerRb.position += d;

            break;
        }
    }

    private bool IsPlayer(Collider2D col)
    {
        if (col.CompareTag(playerTag)) return true;

        if (col.attachedRigidbody != null && col.attachedRigidbody.CompareTag(playerTag)) return true;

        Transform t = col.transform;
        while (t != null)
        {
            if (t.CompareTag(playerTag)) return true;
            t = t.parent;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube((Vector2)transform.position + boxOffset, boxSize);
    }
}