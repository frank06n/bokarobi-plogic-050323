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
    [SerializeField] private float JumpBufferTime;
    [SerializeField] private float J_PauseMinVel;
    [SerializeField] private float J_PauseMaxVel;
    [SerializeField] private float J_PauseMax;

    private Transform handsTransform;
    private Transform handsL;
    private Transform handsR;
    private Transform camTransform;

    private bool isWalking;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool jumping;
    private bool doubleJumping;

    private bool bufferedJump;

    private float jumpPause = 0f;
    private Vector3 handsEulerL;
    private Vector3 handsEulerR;
    private float bobbingDelta;


    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<CharacterController>();
        canDoubleJump = false;
        isWalking = false;

        camTransform = transform.GetChild(1);
        handsTransform = transform.GetChild(2);

        handsL = handsTransform.GetChild(0);
        handsR = handsTransform.GetChild(1);

        handsEulerL = handsL.localEulerAngles;
        handsEulerR = handsR.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = IsPlayerGrounded();
        if (CheckJump())
        {
            Move();
        }
        //CheckFallToDeath();
    }


    bool IsPlayerGrounded()
    {
        return Physics.CheckSphere(transform.position + Vector3.up * gc_FootOffsetY, gc_SphereRadius, LevelManager.instance.Ground) && velocity.y <= 0;
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
    private void applyGravity()
    {
        velocity += Vector3.down * LevelManager.instance.Gravity * Time.deltaTime;
    }
    private void walkCheck()
    {
        bool walking = isGrounded && !(velocity.x == 0 && velocity.z == 0);
        if (walking == isWalking) return;
        isWalking = walking;
        if (walking)
            LevelManager.instance.audioMng.Play("sfx_walk");
        else
            LevelManager.instance.audioMng.Stop("sfx_walk");
    }
    
    private Vector3 _bobFunc(float extent, float time, float offset)
    {
        return Vector3.LerpUnclamped(Vector3.zero,
            extent * Vector3.right, Mathf.Sin(offset + time));
    }
    private void handBobbing()
    {
        if (!isWalking) return;
        handsL.localEulerAngles = handsEulerL + _bobFunc(15, bobbingDelta, 0);
        handsR.localEulerAngles = handsEulerR + _bobFunc(15, bobbingDelta, Mathf.PI);
        bobbingDelta += Time.deltaTime * 8;
    }

    void Move()
    {
        velocity = (getDirectionInput() * Speed) + (Vector3.up * velocity.y);
        if (!isGrounded) applyGravity();
        controller.Move(velocity * Time.deltaTime);

        walkCheck();
        handBobbing();
    }

    void onJumpLanded()
    {
        jumping = doubleJumping = false;
        StartCoroutine(JumpShake());
        LevelManager.instance.audioMng.Play("sfx_jump_impact");

        float vel = velocity.magnitude;
        if (vel > J_PauseMinVel)
        {
            jumpPause = Mathf.Lerp(0f, J_PauseMax, (vel - J_PauseMinVel) / J_PauseMaxVel);
        }
    }
    void performJump()
    {
        jumping = true;
        velocity = Vector3.up * Mathf.Sqrt(2 * LevelManager.instance.Gravity * JumpHeight);
        LevelManager.instance.audioMng.Play("sfx_jump");
    }
    void performDoubleJump()
    {
        doubleJumping = true;
        velocity = Vector3.up * Mathf.Sqrt(2 * LevelManager.instance.Gravity * JumpHeight);
        LevelManager.instance.audioMng.Play("sfx_jump");
        //play double jump sfx
    }
    bool CheckJump()
    {
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        if (jumpPause>0f)
        {
            jumpPause -= Time.deltaTime;
            if (jumpPause <= JumpBufferTime && jumpPressed) bufferedJump = true;
            return false;
        }

        if (isGrounded && jumping) onJumpLanded();

        if ((jumpPressed && isGrounded) || bufferedJump)
        {
            bufferedJump = false;
            performJump();
        }
        else if (jumpPressed && jumping && !doubleJumping && canDoubleJump)
        {
            performDoubleJump();
        }

        return true;
    }


    IEnumerator JumpShake()
    {
        float magnitude = JumpShakeMagnitude * velocity.y / -100f;
        float elapsed = 0;
        Vector3 OriginalPos = camTransform.localPosition; // assuming [camera, hands] same local pos

        while (elapsed < JumpShakeDuration)
        {
            Vector3 offset = Vector3.down * magnitude * JumpShakeCurve.Evaluate(elapsed / JumpShakeDuration);
            
            camTransform.localPosition = OriginalPos + offset;
            handsTransform.localPosition = OriginalPos + 1.2f*offset;

            elapsed += Time.deltaTime;
            yield return null;
        }
        camTransform.localPosition = OriginalPos;
        handsTransform.localPosition = OriginalPos;
    }

    public void SetDoubleJump(bool can)
    {
        canDoubleJump = can;
    }
}
