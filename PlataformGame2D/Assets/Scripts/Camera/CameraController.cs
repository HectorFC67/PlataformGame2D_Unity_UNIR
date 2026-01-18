using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    [Range(0f, 1f)]
    public float followSmoothness = 0.025f;

    [Tooltip("Offset from the target position (useful to keep the player slightly centered).")]
    public Vector3 followOffset = new Vector3(0f, 2f, -10f);

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + followOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, followSmoothness);

        transform.position = smoothedPosition;
    }
}
