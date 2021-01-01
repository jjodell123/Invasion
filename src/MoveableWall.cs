using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableWall : MonoBehaviour
{
    public float speed = 3f;
    public float distance = 5f;

    public bool moveOnZ = true;

    Vector3 startPos;
    float offset;

    void Start()
    {
        startPos = transform.position;

        // Adds slight 
        offset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        Vector3 newPos = transform.position;

        // Moves wall
        if (moveOnZ)
            newPos.z = startPos.z + (Mathf.Sin(Time.time * speed + offset) * distance);
        else
            newPos.x = startPos.x + (Mathf.Sin(Time.time * speed + offset) * distance);

        transform.position = newPos;
    }
}
