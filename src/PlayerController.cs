using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 8f;
    [SerializeField]
    private float jumpHeight = 3f;
    [SerializeField]
    private float gravity = 9.81f;

    [SerializeField]
    private float airControl = 10f;

    [SerializeField]
    private float cameraOffsetRotationY = -58f;

    [SerializeField]
    private float maxDrawTime = 1f;

    [SerializeField]
    private float deathTime = 1f;

    [SerializeField]
    private AudioClip arrowShotSound;

    private float drawTime = 0;

    CharacterController controller;
    Vector3 input, moveDirection;
    private Animator animator;

    private ShootProjectile shooter;

    private bool isFullDrawn = false;
    private bool isDrawingBow = false;

    private PlayerHealth health;
    private bool killedPlayer = false;
    private Quaternion startRot;
    private Quaternion endRot;
    private float timeSinceDeath = 0;

    private Vector3 groundedPostion;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();
        startRot = transform.rotation;
        endRot = startRot * Quaternion.AngleAxis(-90, transform.forward);

        shooter = GetComponentInChildren<ShootProjectile>();
    }

    // Update is called once per frame
    void Update()
    {
        // If player falls off the map before the game is already over, end the game.
        if (transform.position.y < -1f && !LevelManager.isGameOver)
        {
            FindObjectOfType<LevelManager>().LevelLost();
        }
        if (!health.GetPlayerDead())
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            input = Vector3.ClampMagnitude(transform.right * moveHorizontal + transform.forward * moveVertical, 1);
            input = Quaternion.AngleAxis(cameraOffsetRotationY, Vector3.up) * input;
            input *= speed;

            if (isDrawingBow)
            {
                drawTime = Mathf.Clamp(drawTime + Time.deltaTime, 0, maxDrawTime);
                if (drawTime == maxDrawTime)
                {
                    isFullDrawn = true;
                    isDrawingBow = false;
                }
            }

            if (Input.GetButtonDown("Fire1"))
            {
                animator.SetInteger("activeState", 5);
                isDrawingBow = true;
            }

            if (controller.isGrounded)
            {
                moveDirection = input;
                groundedPostion = transform.position;
                if (isFullDrawn)
                {
                    if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                    {
                        animator.SetInteger("activeState", 6);
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        animator.SetInteger("activeState", 7);
                    }
                    else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                    {
                        animator.SetInteger("activeState", 8);
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        animator.SetInteger("activeState", 9);
                    }
                    else
                    {
                        animator.SetInteger("activeState", 5);
                    }

                }
                else if (!isDrawingBow)
                {
                    if (Input.GetButton("Jump"))
                    {
                        moveDirection.y = Mathf.Sqrt(2 * jumpHeight * gravity);
                    }
                    else if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                    {
                        animator.SetInteger("activeState", 1);
                    }
                    else if (Input.GetKey(KeyCode.S))
                    {
                        animator.SetInteger("activeState", 4);
                    }
                    else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                    {
                        animator.SetInteger("activeState", 2);
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        animator.SetInteger("activeState", 3);
                    }
                    else
                    {
                        moveDirection.y = 0.0f;
                        animator.SetInteger("activeState", 0);
                    }
                }
            }
            else if (!controller.isGrounded)
            {
                RaycastHit hit;
                bool hitObject = Physics.Raycast(transform.position, transform.up * -1, out hit, Mathf.Infinity);
                if (hitObject) // If there is an object below the player, set it to the grounded position.
                {
                    groundedPostion = hit.point;
                }
                else
                {
                    groundedPostion = transform.position;
                }
                input.y = moveDirection.y;
                moveDirection = Vector3.Lerp(moveDirection, input, Time.deltaTime * airControl);
            }

            if (Input.GetButtonUp("Fire1"))
            {
                AudioSource.PlayClipAtPoint(arrowShotSound, transform.position);
                isDrawingBow = false;
                if (isFullDrawn)
                    animator.SetInteger("activeState", 10);
                else
                    animator.SetInteger("activeState", 0);
                isFullDrawn = false;
                shooter.FireArrow(drawTime);
                drawTime = 0;
            }

            moveDirection.y -= gravity * Time.deltaTime;

            controller.Move(moveDirection * Time.deltaTime);
        }
        else if (!killedPlayer)
        {
            animator.SetInteger("activeState", 0);
            timeSinceDeath += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, endRot, timeSinceDeath / deathTime);
            if (timeSinceDeath > deathTime)
                killedPlayer = true;
        }
        
    }

    public float GetDrawTime()
    {
        return drawTime;
    }

    public float GetMaxDrawTime()
    {
        return maxDrawTime;
    }

    public Vector3 GetGroundedPosition()
    {
        return groundedPostion;
    }

    public bool CanChangeArrow()
    {
        print (drawTime);
        return drawTime == 0f;
    }
}
