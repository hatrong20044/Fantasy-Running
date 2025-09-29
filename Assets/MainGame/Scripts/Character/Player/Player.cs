using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float laneDistance = 2.5f;
    public float laneChangeSpeed = 10f;  
    public float jumpForce = 8f;
    public float gravity = -20f;

    [Header("Animation")]
    [SerializeField] private Animator anim;
    private string currentAnim;

    private CharacterController controller;
    private Vector3 moveDirection;
    private float verticalVelocity;

    private int currentLane = 1; 

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        
        moveDirection = Vector3.forward * forwardSpeed;

   
        if (controller.isGrounded)
        {
            ChangeAnim("Run");

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                verticalVelocity = jumpForce;
                ChangeAnim("Jump");
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentLane > 0)
            {
                currentLane--;
                ChangeAnim("RunLeft");
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentLane < 2)
            {
                currentLane++;
                ChangeAnim("RunRight");
            }
        }

        
        float targetX = (currentLane - 1) * laneDistance;

        
        float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);
        moveDirection.x = (newX - transform.position.x) / Time.deltaTime;
        moveDirection.y = verticalVelocity;
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void ChangeAnim(string animName)
    {
        if (currentAnim == animName) return;

        anim.ResetTrigger(currentAnim);
        currentAnim = animName;
        anim.SetTrigger(currentAnim);
    }
}
