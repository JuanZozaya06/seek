using UnityEngine;

[RequireComponent (typeof (CharacterController))]
public class ThirdPersonMovement : MonoBehaviour {
    const float StandHeight = 1.1f;
    const float StandRadius = 0.25f;
    const float CrouchHeight = 0.8f;
    const float CrouchRadius = 0.25f;

    static readonly int CrouchHash = Animator.StringToHash ("crouch");
    static readonly int JoggingHash = Animator.StringToHash ("jogging");
    static readonly int RunningHash = Animator.StringToHash ("running");
    static readonly int CrouchWalkHash = Animator.StringToHash ("crouchWalk");
    static readonly int JumpHash = Animator.StringToHash ("jump");

    public CharacterController controller;
    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    public Transform cam;
    public float runSpeed = 7.5f;
    public float speed = 5f;
    public float crouchSpeed = 2f;
    public float jumpHeight = 3f;
    public bool running = false;
    public bool jogging = false;
    public bool crouch = false;
    public bool crouchWalk = false;
    public bool jumping = false;
    public bool jumped = false;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.06f;
    public float inputSmoothTime = 0.08f;
    public float acceleration = 18f;
    public float deceleration = 24f;

    public Vector3 direction;
    public bool isGrounded;

    float turnSmoothVelocity;
    Vector2 smoothedInput;
    Vector2 inputSmoothVelocity;
    Vector3 velocity;
    Vector3 horizontalVelocity;
    Animator anim;
    CapsuleCollider capsuleCollider;

    void Awake () {
        controller = controller != null ? controller : GetComponent<CharacterController> ();
        capsuleCollider = GetComponent<CapsuleCollider> ();
        anim = GetComponent<Animator> ();

        if (cam == null && Camera.main != null) {
            cam = Camera.main.transform;
        }
    }

    void Update () {
        if (controller == null) {
            return;
        }

        HandleCrouchInput ();
        UpdateGroundedState ();
        MoveCharacter ();
        UpdateAnimator ();
    }

    void HandleCrouchInput () {
        if (Input.GetKeyDown (KeyCode.LeftControl) && !crouch) {
            crouch = true;
            ApplyColliderSize (CrouchHeight, CrouchRadius);
        }

        if (Input.GetKeyUp (KeyCode.LeftControl) && crouch) {
            crouch = false;
            crouchWalk = false;
            ApplyColliderSize (StandHeight, StandRadius);
        }
    }

    void UpdateGroundedState () {
        if (groundCheck != null) {
            isGrounded = Physics.CheckSphere (groundCheck.position, groundDistance, groundMask, QueryTriggerInteraction.Ignore);
        } else {
            isGrounded = controller.isGrounded;
        }

        if (isGrounded && velocity.y < 0f) {
            velocity.y = -2f;
            jumped = false;
        }
    }

    void MoveCharacter () {
        Vector2 rawInput = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
        rawInput = Vector2.ClampMagnitude (rawInput, 1f);
        smoothedInput = Vector2.SmoothDamp (smoothedInput, rawInput, ref inputSmoothVelocity, inputSmoothTime);

        direction = new Vector3 (smoothedInput.x, 0f, smoothedInput.y);
        bool hasMoveInput = direction.sqrMagnitude >= 0.01f;
        running = hasMoveInput && !crouch && Input.GetKey (KeyCode.LeftShift);
        jogging = hasMoveInput && !crouch && !running;
        crouchWalk = hasMoveInput && crouch;

        Vector3 desiredHorizontalVelocity = Vector3.zero;

        if (hasMoveInput) {
            float cameraYaw = cam != null ? cam.eulerAngles.y : transform.eulerAngles.y;
            float targetAngle = Mathf.Atan2 (direction.x, direction.z) * Mathf.Rad2Deg + cameraYaw;
            float angle = Mathf.SmoothDampAngle (transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler (0f, angle, 0f);

            float targetSpeed = crouch ? crouchSpeed : running ? runSpeed : speed;
            Vector3 moveDirection = Quaternion.Euler (0f, angle, 0f) * Vector3.forward;
            desiredHorizontalVelocity = moveDirection.normalized * targetSpeed;
        }

        float rate = hasMoveInput ? acceleration : deceleration;
        horizontalVelocity = Vector3.MoveTowards (horizontalVelocity, desiredHorizontalVelocity, rate * Time.deltaTime);

        if (Input.GetButtonDown ("Jump") && isGrounded && !crouch) {
            velocity.y = Mathf.Sqrt (jumpHeight * -2f * gravity);
            jumped = true;
        }

        jumping = !isGrounded || velocity.y > 0.1f;
        velocity.y += gravity * Time.deltaTime;

        Vector3 frameVelocity = horizontalVelocity + Vector3.up * velocity.y;
        controller.Move (frameVelocity * Time.deltaTime);
    }

    void UpdateAnimator () {
        if (anim == null) {
            return;
        }

        anim.SetBool (CrouchHash, crouch);
        anim.SetBool (JoggingHash, jogging);
        anim.SetBool (RunningHash, running);
        anim.SetBool (CrouchWalkHash, crouchWalk);
        anim.SetBool (JumpHash, jumping);
    }

    void ApplyColliderSize (float height, float radius) {
        Vector3 center = new Vector3 (0f, height * 0.5f, 0f);

        controller.height = height;
        controller.radius = radius;
        controller.center = center;

        if (capsuleCollider != null) {
            capsuleCollider.height = height;
            capsuleCollider.radius = radius;
            capsuleCollider.center = center;
        }
    }
}
