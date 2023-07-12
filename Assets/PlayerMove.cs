using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 velocity;

    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float sphereRadius;
    [SerializeField] private float footYoffset;

    [SerializeField] private Transform handsTransform;
    private Transform handsL;
    private Transform handsR;
    [SerializeField] private Vector3 offset;
    private Transform camTransform;

    private bool isWalking;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool jumping;
    private bool doubleJumping;

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

        camTransform = Camera.main.transform;

        handsL = handsTransform.GetChild(0);
        handsR = handsTransform.GetChild(1);

        handsEulerL = handsL.localEulerAngles;
        handsEulerR = handsR.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = IsPlayerGrounded();
        if (jumpPause>0f)
        {
            jumpPause -= Time.deltaTime;
            return;
        }
        CheckJump();
        Move();
        //CheckFallToDeath();
    }


    bool IsPlayerGrounded()
    {
        return Physics.CheckSphere(transform.position + Vector3.up * footYoffset, sphereRadius, LevelManager.instance.Ground) && velocity.y <= 0;
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
        velocity = (getDirectionInput() * speed) + (Vector3.up * velocity.y);
        if (!isGrounded) applyGravity();
        controller.Move(velocity * Time.deltaTime);

        walkCheck();
        handBobbing();
    }

    void CheckJump()
    {
        if (isGrounded && jumping)
        {
            jumping = doubleJumping = false;
            //float fcx = (-velocity.y / 15f); // factor
            //StartCoroutine(JumpShake(0.6f, 0.1f * fcx*fcx));
            LevelManager.instance.audioMng.Play("sfx_jump_impact");
            jumpPause = 0.5f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                jumping = true;
                velocity = Vector3.up * Mathf.Sqrt(2 * LevelManager.instance.Gravity * jumpHeight);
                LevelManager.instance.audioMng.Play("sfx_jump");
            }
            else if (jumping && !doubleJumping && canDoubleJump)
            {
                doubleJumping = true;
                velocity = Vector3.up * Mathf.Sqrt(2 * LevelManager.instance.Gravity * jumpHeight);
                LevelManager.instance.audioMng.Play("sfx_jump");
                //play double jump sfx
            }
        }
    }

}
