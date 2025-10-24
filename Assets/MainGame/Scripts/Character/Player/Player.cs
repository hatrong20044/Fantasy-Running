using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed;// = 5f;
    public float laneDistance;// = 2.5f;
    public float laneChangeSpeed;// = 10f;
    public float jumpForce;// = 8f;
    public float gravity;// = -20f;
    public float jumpForwardBoost;// = 2f;

    [Header("Slide Settings")]
    public float slideDuration;// = 1f;
    [Tooltip("Chiều cao khi slide (cả Controller và visual Collider)")]
    public float slideHeight;// = 1.4f;
    [Tooltip("Center Y khi slide")]
    public float slideCenter;// = -0.4f;

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
    private bool isDead = false;
    private bool wantSlideAfterJump = false;

    private float slideTimer = 0f;

    private float normalHeight;
    private Vector3 normalCenter;
    private float capsuleNormalHeight;
    private Vector3 capsuleNormalCenter;

    // Swipe
    private Vector2 touchStart;
    private Vector2 touchEnd;
    private bool swipeLeft, swipeRight, swipeUp, swipeDown;
    private void Reset()
    {
        forwardSpeed = GameSetting.Instance.ForwardSpeed;
        laneDistance = GameSetting.Instance.LaneDistance;
        laneChangeSpeed = GameSetting.Instance.LaneChangeSpeed;
        jumpForce = GameSetting.Instance.JumpForce;
        gravity = GameSetting.Instance.Gravity;
        jumpForwardBoost = GameSetting.Instance.JumpForwardBoost;
        slideDuration = GameSetting.Instance.SlideDuration;
        slideHeight = GameSetting.Instance.SlideHeight;
        slideCenter = GameSetting.Instance.SlideCenter;
    }
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
        if (isDead) return;
        HandleSwipe();
        HandleSlide();

        moveDirection = Vector3.forward * forwardSpeed;

        // ✅ Nếu đang đứng trên mặt đất
        if (controller.isGrounded)
        {
            // Vừa chạm đất sau khi nhảy
            if (isJumping)
            {
                isJumping = false;

                // Nếu đang yêu cầu slide khi vừa chạm đất
                if (wantSlideAfterJump)
                {
                    wantSlideAfterJump = false;
                    StartSlide();
                    return;
                }
            }

            if (!isSliding && !isJumping)
            {
                ChangeAnim("Run");
            }

            // Nhảy
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || swipeUp) && !isSliding)
            {
                verticalVelocity = jumpForce;
                isJumping = true;
                ChangeAnim("Jump");
            }

            // Slide khi đang chạy
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || swipeDown) && !isJumping && !isSliding)
            {
                StartSlide();
            }

            if (!isJumping)
                verticalVelocity = -2f;
        }
        else // ✅ Khi đang nhảy
        {
            verticalVelocity += gravity * Time.deltaTime;

            // Nếu đang nhảy mà người chơi kéo xuống
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || swipeDown) && isJumping && !wantSlideAfterJump)
            {
                ChangeAnim("Slide");
                verticalVelocity = Mathf.Lerp(verticalVelocity, gravity, 0.5f);
                wantSlideAfterJump = true;       // Đánh dấu slide khi chạm đất
            }
        }

        // Khi đang nhảy, thêm lực tiến tới
        if (isJumping)
        {
            moveDirection += Vector3.forward * jumpForwardBoost;
        }

        // Di chuyển trái / phải
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || swipeLeft)
        {
            if (isDirectionReversed) MoveRight();
            else MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || swipeRight)
        {
            if (isDirectionReversed) MoveLeft();
            else MoveRight();
        }

        // Lane movement
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

    public void Die()
    {
        if (isDead) return; 
        isDead = true;

        forwardSpeed = 0f;
        moveDirection = Vector3.zero;
        verticalVelocity = 0f;
        ChangeAnim("Die");
        controller.enabled = false;
        Debug.Log("Player Died!");
        
    }


    public void ReverseInput(bool reversed)
    {
        isDirectionReversed = reversed;
        Debug.Log("Reverse Input: " + reversed);
    }

    private void ChangeAnim(string animName)
    {
        if (currentAnim == animName) return;
        currentAnim = animName;
        anim.CrossFadeInFixedTime(animName, 0.1f);
    }

}