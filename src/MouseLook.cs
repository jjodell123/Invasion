using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private float mouseSensitivity = 360f;
    [SerializeField]
    private float cameraOffsetRotationY = -45f;
    Transform playerBody;
    float pitch = 0;
    // Start is called before the first frame update
    void Start()
    {
        playerBody = transform.parent.transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerBody.gameObject.GetComponent<PlayerHealth>().GetPlayerDead())
        {
            float moveX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float moveY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Adjust yaw
            playerBody.Rotate(Vector3.up * moveX);

            // Adjust pitch
            pitch -= moveY;
            pitch = Mathf.Clamp(pitch, -90f, 90f);

            transform.localRotation = Quaternion.Euler(pitch, cameraOffsetRotationY, 0);
        }
    }
}
