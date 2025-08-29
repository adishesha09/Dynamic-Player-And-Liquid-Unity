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
    public float rotationSmoothness = 8f;
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

        // Calculate movement direction relative to camera orientation
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        
        // Ignore camera pitch for ground movement
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement vector relative to camera
        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
                rotationSmoothness * Time.deltaTime);
        }

        // Apply movement in world space
        controller.Move(moveDirection * speed * Time.deltaTime);

        // Pass movement to the Animator (use relative speed)
        animator.SetFloat("Speed", Mathf.Clamp01(moveDirection.magnitude) * (speed / walkSpeed));
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetKeyDown(KeyCode.Z))
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
