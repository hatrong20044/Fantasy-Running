using System.Collections;
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
    private bool canRun = false;


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
    private void OnEnable()
    {
        GameplayUI.OnPlayPressed += StartRunFromUI;
        GameplayUI.OnPlayPressed += RotateToForward; // thêm dòng này
    }

    private void OnDisable()
    {
        GameplayUI.OnPlayPressed -= StartRunFromUI;
        GameplayUI.OnPlayPressed -= RotateToForward; // thêm dòng này
    }


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        normalHeight = controller.height;
        normalCenter = controller.center;

        if (capsuleCollider != null)
        {
            capsuleNormalHeight = capsuleCollider.height;
            capsuleNormalCenter = capsuleCollider.center;
        }
        transform.rotation = Quaternion.Euler(0, -90, 0);
    }

    private void Update()
    {
        if (isDead) return;
        if (!canRun) return;
        HandleSwipe();
        HandleSlide();

        moveDirection = transform.forward * forwardSpeed;

        
        if (controller.isGrounded)
        {
           
            if (isJumping)
            {
                isJumping = false;

                
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
                verticalVelocity = -2f;
        }
        else 
        {
            verticalVelocity += gravity * Time.deltaTime;

            
            if ((Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || swipeDown) && isJumping && !wantSlideAfterJump)
            {
                ChangeAnim("Slide");
                verticalVelocity = Mathf.Lerp(verticalVelocity, gravity, 0.5f);
                wantSlideAfterJump = true;       
            }
        }

        if (isJumping)
        {
            moveDirection += Vector3.forward * jumpForwardBoost;
        }

        
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

       
        controller.height = normalHeight;
        controller.center = normalCenter;

        
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
    public void SetCanRun(bool value)
    {
        canRun = value;
        if (value)
            ChangeAnim("Run");
        else
            ChangeAnim("Idle"); 
    }

    public bool CanRun => canRun;
    private void RotateToForward()
    {
        StartCoroutine(RotateSmooth(Quaternion.Euler(0, 0, 0), 1f));
    }

    private void StartRunFromUI()
    {
        SetCanRun(true);
        currentLane = 0;

        
        Vector3 pos = transform.position;
        pos.x = 0f;
        transform.position = pos;

        
        moveDirection = Vector3.zero;
    }
    private IEnumerator RotateSmooth(Quaternion targetRot, float duration)
    {
        Quaternion startRot = transform.rotation;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.SmoothStep(0, 1, t / duration);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, normalized);
            yield return null;
        }

        transform.rotation = targetRot;
    }

    private void ChangeAnim(string animName)
    {
        if (currentAnim == animName) return;
        currentAnim = animName;
        anim.CrossFadeInFixedTime(animName, 0.1f);
    }

}