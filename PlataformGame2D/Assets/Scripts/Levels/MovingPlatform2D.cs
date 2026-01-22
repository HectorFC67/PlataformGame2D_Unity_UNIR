using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform2D : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;

    private Rigidbody2D rb;
    private Vector2 target;

    public Vector2 FrameDelta { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (pointA == null || pointB == null)
        {
            Debug.LogError($"{name}: Asigna pointA y pointB.");
            enabled = false;
            return;
        }

        target = pointB.position;
    }

    private void FixedUpdate()
    {
        Vector2 prevPos = rb.position;

        Vector2 next = Vector2.MoveTowards(prevPos, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        FrameDelta = next - prevPos;

        if (Vector2.Distance(next, target) < 0.01f)
            target = (target == (Vector2)pointA.position) ? (Vector2)pointB.position : (Vector2)pointA.position;
    }
}
