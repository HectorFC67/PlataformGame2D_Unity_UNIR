using UnityEngine;

namespace EnemyPatrolling
{
    public class Patrolling : MonoBehaviour
    {
        public Transform[] patrolPoints;
        public float moveSpeed = 3.0f;

        public Vector3 rotationAngles = new Vector3(0f, 180f, 0f);

        private int currentPointIndex = 0;

        private Transform currentTarget;

        private bool isDead = false;

        private Animator anim;

        [SerializeField] private AudioClip PatrolSound;
        [SerializeField] private AudioSource audioSource;

        private void Start()
        {
            if (patrolPoints.Length > 0)
            {
                currentTarget = patrolPoints[currentPointIndex];
                transform.position = currentTarget.position;

                InvokeRepeating("PlayPatrolSound", 0f, 10f);

                anim = GetComponent<Animator>();
            }
        }

        private void PlayPatrolSound()
        {
            if (!isDead)
            {
                audioSource.PlayOneShot(PatrolSound);
            }
        }

        private void FixedUpdate()
        {
            if (!isDead && patrolPoints.Length > 0)
            {
                Patrol();
            }
        }

        private void Patrol()
        {
            Vector3 moveDirectionVector = (currentTarget.position - transform.position).normalized;

            transform.position += moveDirectionVector * moveSpeed * Time.deltaTime;

            if (Vector2.Distance(transform.position, currentTarget.position) < 0.2f)
            {
                RotateAtPoint();
                currentPointIndex++;

                if (currentPointIndex >= patrolPoints.Length)
                {
                    currentPointIndex = 0;
                }
                currentTarget = patrolPoints[currentPointIndex];
            }
        }

        private void RotateAtPoint()
        {
            transform.eulerAngles += rotationAngles;
        }
    }
}
