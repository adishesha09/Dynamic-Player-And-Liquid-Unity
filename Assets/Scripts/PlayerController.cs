using UnityEngine;

// Requires CharacterController and Animator components on the same GameObject.
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]

public class NewMonoBehaviourScript : MonoBehaviour
{

    [Header("References")]
    public CharacterController controller;
    public Animator animator;

    // Serialized fields for player movement and wall climbing parameters.
    [Header("Movement Variables")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 2f;
    public float slideSpeed = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Wall Climb Variables")]
    public float climbSpeed = 3f;
    public float climbCheckDistance = 0.6f;
    public LayerMask climbLayer;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isCrouching;
    private bool isSliding;
    private bool isClimbing;

    void Start()
    {
        // Automatically assign components
        if (!controller) controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponent<Animator>();
    }


    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleSlide();
        HandleClimb();
        ApplyGravity();
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        float speed = walkSpeed;
        if (Input.GetKey(KeyCode.LeftControl)) speed = sprintSpeed;
        if (isCrouching) speed = crouchSpeed;
        if (isSliding) speed = slideSpeed;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculates horizontal movement vector in local space.
        Vector3 movePlayer = transform.right * horizontal + transform.forward * vertical;
        controller.Move(movePlayer * speed * Time.deltaTime);

        // Pass movement to the Animator
        animator.SetFloat("Speed", movePlayer.magnitude * speed);
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetKey(KeyCode.Z))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            animator.SetBool("Crouch", isCrouching);
            Invoke("StopSlide", 1.0f); // Slide lasts around a second
        }
    }

    void HandleSlide()
    {
        if (Input.GetKey(KeyCode.LeftControl) && isGrounded && !isSliding)
        {
            isSliding = true;
            animator.SetTrigger("Slide");

        }
    }

    void StopSlide()
    {
        isSliding = false;
    }

    void HandleClimb()
    {
        RaycastHit rayHit;

        if (Physics.Raycast(transform.position, transform.forward, out rayHit, climbCheckDistance, climbLayer))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                isClimbing = true;
                Vector3 climbDirection = Vector3.up * climbSpeed * Time.deltaTime;
                controller.Move(climbDirection);
                animator.SetBool("Climb", true);
            }
        }
        else
        {
            isClimbing = false;
            animator.SetBool("Climb", false);
        }
    }

    void ApplyGravity()
    {
        if (!isGrounded && !isClimbing)
        {
            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
        else if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Ensure player stays grounded
        }
    } 
}