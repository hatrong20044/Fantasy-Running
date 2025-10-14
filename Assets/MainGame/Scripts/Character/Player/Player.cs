using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float laneDistance = 2.5f;
    public float laneChangeSpeed = 10f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float jumpForwardBoost = 2f;

    [Header("Slide Settings")]
    public float slideDuration = 1f;
    [Tooltip("Chiều cao khi slide (cả Controller và visual Collider)")]
    public float slideHeight = 1.4f;
    [Tooltip("Center Y khi slide")]
    public float slideCenter = -0.4f;

    [Header("Animation")]
    [SerializeField] private Animator anim;
    private string currentAnim;

    private CharacterController controller;
    private CapsuleCollider capsuleCollider;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private int currentLane = 1;
    private bool isDirectionReversed = false;
    private bool isJumping = false;
    private bool isSliding = false;
    private float slideTimer = 0f;

    
    private float normalHeight;
    private Vector3 normalCenter;
    private float capsuleNormalHeight;
    private Vector3 capsuleNormalCenter;

    // Swipe
    private Vector2 touchStart;
    private Vector2 touchEnd;
    private bool swipeLeft, swipeRight, swipeUp, swipeDown;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        normalHeight = controller.height;
        normalCenter = controller.center;

        // Lưu giá trị ban đầu của Capsule Collider
        if (capsuleCollider != null)
        {
            capsuleNormalHeight = capsuleCollider.height;
            capsuleNormalCenter = capsuleCollider.center;
        }
    }

    private void Update()
    {
        HandleSwipe();
        HandleSlide();

        moveDirection = Vector3.forward * forwardSpeed;

        if (controller.isGrounded)
        {
            if (isJumping)
            {
                isJumping = false;
            }

            
            if (!isSliding && !isJumping)
            {
                ChangeAnim("Run");
            }

            
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || swipeUp) && !isSliding)
            {
                verticalVelocity = jumpForce;
                isJumping = true;
                ChangeAnim("Jump");
            }

            
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || swipeDown) && !isJumping && !isSliding)
            {
                StartSlide();
            }

            
            if (!isJumping)
            {
                verticalVelocity = -2f;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (isJumping)
        {
            moveDirection += Vector3.forward * jumpForwardBoost;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || swipeLeft)
        {
            if (isDirectionReversed)
                MoveRight();
            else
                MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || swipeRight)
        {
            if (isDirectionReversed)
                MoveLeft();
            else
                MoveRight();
        }

        float targetX = (currentLane - 1) * laneDistance;
        float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
        moveDirection.x = (newX - transform.position.x) / Time.deltaTime;

        moveDirection.y = verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void HandleSwipe()
    {
        swipeLeft = swipeRight = swipeUp = swipeDown = false;

        if (Input.GetMouseButtonDown(0))
            touchStart = Input.mousePosition;

        if (Input.GetMouseButtonUp(0))
        {
            touchEnd = Input.mousePosition;
            Vector2 delta = touchEnd - touchStart;

            if (delta.magnitude > 100)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    if (delta.x > 0) swipeRight = true;
                    else swipeLeft = true;
                }
                else
                {
                    if (delta.y > 0) swipeUp = true;
                    else swipeDown = true;
                }
            }
        }
    }

    private void HandleSlide()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;

            if (slideTimer <= 0f)
            {
                EndSlide();
            }
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

       
        controller.height = slideHeight;
        controller.center = new Vector3(0, slideCenter, 0);

        if (capsuleCollider != null)
        {
            capsuleCollider.height = slideHeight;
            capsuleCollider.center = new Vector3(0, slideCenter, 0);
        }

        ChangeAnim("Slide");
    }

    private void EndSlide()
    {
        isSliding = false;

        // Khôi phục Character Controller
        controller.height = normalHeight;
        controller.center = normalCenter;

        // Khôi phục Capsule Collider
        if (capsuleCollider != null)
        {
            capsuleCollider.height = capsuleNormalHeight;
            capsuleCollider.center = capsuleNormalCenter;
        }

        if (controller.isGrounded)
        {
            ChangeAnim("Run");
        }
    }

    private void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;

            if (!isSliding && !isJumping)
            {
                ChangeAnim("RunLeft");
            }
        }
    }

    private void MoveRight()
    {
        if (currentLane < 2)
        {
            currentLane++;

            if (!isSliding && !isJumping)
            {
                ChangeAnim("RunRight");
            }
        }
    }

    public void ReverseInput(bool reversed)
    {
        isDirectionReversed = reversed;
        Debug.Log("Reverse Input: " + reversed);
    }

    private void ChangeAnim(string animName)
    {
        if (currentAnim == animName) return;

        anim.ResetTrigger(currentAnim);
        currentAnim = animName;
        anim.SetTrigger(currentAnim);
    }
}