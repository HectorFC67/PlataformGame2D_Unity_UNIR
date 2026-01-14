using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMovement : MonoBehaviour
{
    Transform cam;
    Vector3 camStartPos;
    Vector3 parentStartPos;
    float distance;

    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;

    float farthestBack;

    [Range(0.01f, 1f)]
    public float parallaxSpeed = 0.2f;

    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;
        parentStartPos = transform.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        farthestBack = 0f;

        for (int i = 0; i < backCount; i++)
        {
            float depth = backgrounds[i].transform.position.z - cam.position.z;
            if (depth > farthestBack) farthestBack = depth;
        }

        // Evita división por cero si todos están al mismo z
        if (Mathf.Approximately(farthestBack, 0f)) farthestBack = 1f;

        for (int i = 0; i < backCount; i++)
        {
            float depth = backgrounds[i].transform.position.z - cam.position.z;
            backSpeed[i] = 1f - (depth / farthestBack);
        }
    }

    private void LateUpdate()
    {
        distance = cam.position.x - camStartPos.x;

        // Mantén el offset inicial exacto
        transform.position = new Vector3(parentStartPos.x + distance, parentStartPos.y, parentStartPos.z);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            mat[i].SetTextureOffset("_MainTex", new Vector2(distance * speed, 0f));
        }
    }
}
