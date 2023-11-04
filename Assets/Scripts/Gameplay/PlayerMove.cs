using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private AnimationCurve JumpShakeCurve;
    [SerializeField] private float JumpShakeDuration;
    [SerializeField] private float JumpShakeMagnitude;

    private Transform handsTransform;
    private Transform handsL;
    private Transform handsR;
    private Transform cameraTransform;

    private bool isWalking;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool isJumping;
    private bool isDoubleJumping;

    private Vector3 handsEulerL;
    private Vector3 handsEulerR;
    private float bobbingDelta;


    void Awake()
    {
        controller = GetComponent<CharacterController>();
        canDoubleJump = false;
        isWalking = false;

        cameraTransform = transform.GetChild(1);
        handsTransform = transform.GetChild(2);

        handsL = handsTransform.GetChild(0);
        handsR = handsTransform.GetChild(1);

        handsEulerL = handsL.localEulerAngles;
        handsEulerR = handsR.localEulerAngles;
    }

    void Update()
    {
        bool wasJumping = isJumping;
        isGrounded = IsGrounded();

        if (wasJumping && isGrounded) OnJumpLanded();

        CheckJump();
        CheckMove();
        CheckWalk();

        if (isWalking) DoHandBobbing();
    }


    private bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position + Vector3.up * gc_FootOffsetY, gc_SphereRadius, LevelManager.instance.ground) && velocity.y <= 0;
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


    private Vector3 _bobFunc(float extent, float time, float offset)
    {
        return Vector3.LerpUnclamped(Vector3.zero,
            extent * Vector3.right, Mathf.Sin(offset + time));
    }
    private void DoHandBobbing()
    {
        handsL.localEulerAngles = handsEulerL + _bobFunc(15, bobbingDelta, 0);
        handsR.localEulerAngles = handsEulerR + _bobFunc(15, bobbingDelta, Mathf.PI);
        bobbingDelta += Time.deltaTime * 8;
    }

    void CheckJump()
    {
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        if      (!jumpPressed) return;
        else if (isGrounded)   DoJump();
        else if (isJumping && canDoubleJump && !isDoubleJumping)
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

        if (wasWalking != isWalking)
            AudioManager.instance.Play(Audio.WALK);
        else
            AudioManager.instance.Stop(Audio.WALK);
    }

    void OnJumpLanded()
    {
        isJumping = isDoubleJumping = false;
        StartCoroutine(JumpShake());
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
        float magnitude = JumpShakeMagnitude * velocity.y / -100f;
        float elapsed = 0;
        Vector3 OriginalPos = cameraTransform.localPosition; // assuming [camera, hands] same local pos

        while (elapsed < JumpShakeDuration)
        {
            Vector3 offset = Vector3.down * magnitude * JumpShakeCurve.Evaluate(elapsed / JumpShakeDuration);
            
            cameraTransform.localPosition = OriginalPos + offset;
            handsTransform.localPosition = OriginalPos + 1.2f*offset;

            elapsed += Time.deltaTime;
            yield return null;
        }
        cameraTransform.localPosition = OriginalPos;
        handsTransform.localPosition = OriginalPos;
    }

    public void SetCanDoubleJump()
    {
        canDoubleJump = true;
    }
}
