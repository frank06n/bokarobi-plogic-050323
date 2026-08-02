using System.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 velocity;

    [SerializeField] private float Speed;

    [Header("Ground Check Config")]
    [SerializeField] private float gc_SphereRadius;
    [SerializeField] private float gc_FootOffsetY;

    [Header("Jump Config")]
    [SerializeField] private float JumpHeight;
    [SerializeField] private float JumpAfterAerialBuffer;
    [SerializeField] private float JumpAfterPressBuffer;

    [Header("Jump Shake Config")]
    [SerializeField] private AnimationCurve JumpShakeCurve;
    [SerializeField] private float JumpShakeDuration;
    [SerializeField] private float JumpShakeMagnitude;

    private Transform handsTransform;
    private Transform cameraTransform;
    private Animator handsAnimator;

    private bool isWalking;
    private bool isGrounded;
    private bool canJump;
    private bool canDoubleJump;
    private bool isJumping;
    private bool isDoubleJumping;
    private bool immobile;

    private float lastGrounded = 99;
    private float lastJumpPressed = 99;



    void Awake()
    {
        controller = GetComponent<CharacterController>();
        canJump = true;
        canDoubleJump = false;
        isWalking = false;
        immobile = false;

        cameraTransform = transform.GetChild(0);
        handsTransform = transform.GetChild(1);
        handsAnimator = handsTransform.GetComponent<Animator>();
    }

    void Update()
    {
        if (immobile) return;

        if (standingOnPlatform != null) controller.Move(standingOnPlatform.LastMoveDelta);

        bool wasJumping = isJumping;
        CheckGrounded();

        if (wasJumping && isGrounded) OnJumpLanded();

        UpdateJumpBuffers();

        CheckJump();
        CheckMove();
        CheckWalk();

        DoHandBobbing();
    }

    private readonly Collider[] groundHits = new Collider[1];
    private Platform standingOnPlatform = null;

    private void CheckGrounded()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position + Vector3.up * gc_FootOffsetY,
            gc_SphereRadius,
            groundHits,
            LevelManager.instance.ground
        );
        isGrounded = hitCount > 0;
        standingOnPlatform = null;

        if (isGrounded)
        {
            standingOnPlatform = groundHits[0].GetComponent<Platform>();
            if (standingOnPlatform == null)
            {
                Debug.Log("Standing on non-platform object: " + groundHits[0].name);
            }
        }
    }
    private bool IsWalking()
    {
        return isGrounded && !(velocity.x == 0 && velocity.z == 0);
    }

    private Vector3 getDirectionInput()
    {
        float inputX = 0, inputY = 0;

        if (Input.GetKey(KeyCode.W)) inputY += 1;
        if (Input.GetKey(KeyCode.A)) inputX -= 1;
        if (Input.GetKey(KeyCode.D)) inputX += 1;
        if (Input.GetKey(KeyCode.S)) inputY -= 1;

        return (transform.forward * inputY + transform.right * inputX).normalized;
    }
    private Vector3 calculateVelocity()
    {
        Vector3 verticalVelocity = Vector3.up * velocity.y;
        Vector3 horizontalVelocity = getDirectionInput() * Speed;
        return horizontalVelocity + verticalVelocity;
    }
    private Vector3 calculateGravityVelocity()
    {
        return Vector3.down * LevelManager.instance.gravity * Time.deltaTime;
    }
    private Vector3 calculateJumpVelocity()
    {
        return Vector3.up * Mathf.Sqrt(2 * LevelManager.instance.gravity * JumpHeight);
    }

    private void DoHandBobbing()
    {
        handsAnimator.SetBool("Run", isWalking);
        handsAnimator.SetBool("Jump", isJumping);
    }

    void UpdateJumpBuffers()
    {
        lastGrounded = isGrounded ? 0 : (lastGrounded + Time.deltaTime);
        lastJumpPressed += Time.deltaTime;
    }

    void CheckJump()
    {
        if (!canJump) return;

        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        if (jumpPressed) lastJumpPressed = 0;

        bool validJumpPress = lastJumpPressed <= JumpAfterPressBuffer;
        bool validGrounded = lastGrounded <= JumpAfterAerialBuffer;

        if (jumpPressed && validGrounded)
        {
            DoJump();
        }
        else if (validJumpPress && isGrounded)
        {
            DoJump();
        }
        else if (jumpPressed && isJumping && canDoubleJump && !isDoubleJumping)
        {
            DoDoubleJump();
        }
    }
    void CheckMove()
    {
        velocity = calculateVelocity();
        if (!isGrounded) velocity += calculateGravityVelocity();

        controller.Move(velocity * Time.deltaTime);
    }
    void CheckWalk()
    {
        bool wasWalking = isWalking;
        isWalking = IsWalking();

        // later separate this for another method
        if (!wasWalking && isWalking)
        {
            AudioManager.instance.Play(Audio.WALK);
        }
        else if (wasWalking && !isWalking)
        {
            AudioManager.instance.Stop(Audio.WALK);
        }
    }

    void OnJumpLanded()
    {
        isJumping = isDoubleJumping = false;
        StartCoroutine(JumpShake());
        velocity.y = 0;
        AudioManager.instance.Play(Audio.JUMP_IMPACT);
    }
    void DoJump()
    {
        isJumping = true;
        velocity = calculateJumpVelocity();
        AudioManager.instance.Play(Audio.JUMP);
    }
    void DoDoubleJump()
    {
        isDoubleJumping = true;
        velocity = calculateJumpVelocity();
        AudioManager.instance.Play(Audio.JUMP);//play double jump sfx
    }


    IEnumerator JumpShake()
    {
        if (JumpShakeDuration <= 0f || JumpShakeMagnitude <= 0f)
            yield break; // Early exit if values don't make sense

        float shakeMagnitude = JumpShakeMagnitude * velocity.y / -100f;
        float elapsedTime = 0;

        Vector3 camPos = cameraTransform.localPosition;
        Vector3 handPos = handsTransform.localPosition;

        // for simulating different head/eye & hands movement
        const float handsShakeMultiplier = 1.2f;

        while (elapsedTime < JumpShakeDuration)
        {
            Vector3 offset = Vector3.down * shakeMagnitude * JumpShakeCurve.Evaluate(elapsedTime / JumpShakeDuration);

            cameraTransform.localPosition = camPos + offset;
            handsTransform.localPosition = handPos + handsShakeMultiplier * offset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cameraTransform.localPosition = camPos;
        handsTransform.localPosition = handPos;
    }

    IEnumerator SlowDownAndImmobilize(float slowTime)
    {
        float elapsedTime = 0, initialSpeed = Speed;
        PlayerLook pLook = GetComponent<PlayerLook>();
        canJump = false;

        while (elapsedTime < slowTime)
        {
            float inv_t = 1f - (elapsedTime / slowTime);
            Speed = initialSpeed * inv_t;
            pLook.SetSensitivityMultiplier(inv_t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Speed = 0;
        pLook.SetSensitivityMultiplier(0);
        immobile = true;

        handsAnimator.SetBool("Run", false);
        handsAnimator.SetBool("Jump", false);
        AudioManager.instance.Stop(Audio.WALK);
    }

    public void SetCanDoubleJump()
    {
        canDoubleJump = true;
    }

    public void SetImmobile(float slowTime)
    {
        StartCoroutine(SlowDownAndImmobilize(slowTime));
    }
}
