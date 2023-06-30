using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 velocity;

    [SerializeField] private float speed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float sphereRadius;
    [SerializeField] private float footYoffset;


    [SerializeField] private float mouseSensitivity = 2f;
    private Transform camTransform;
    private float camVertRotation = 0f;

    private bool gameover;
    private bool wasWalking;
    private bool isGrounded;
    private bool canDoubleJump;
    private bool jumping;
    private bool doubleJumping;

    void Awake()
    {
        gameover = false;
        controller = GetComponent<CharacterController>();
        canDoubleJump = false;
        wasWalking = false;

        camTransform = Camera.main.transform;
    }

    void Update()
    {
        if (gameover) return;

        isGrounded = IsPlayerGrounded();
        CheckJump();
        PlayerLook();
        PlayerMove();
        CheckFallToDeath();
    }
    bool IsPlayerGrounded()
    {
        return Physics.CheckSphere(transform.position + Vector3.up*footYoffset, sphereRadius, LevelManager.instance.Ground) && velocity.y <=0;
    }

    void PlayerLook()
    {
        // Collect Mouse Input
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the Camera around its local X axis
        camVertRotation -= inputY;
        camVertRotation = Mathf.Clamp(camVertRotation, -90f, 90f);
        camTransform.localEulerAngles = Vector3.right * camVertRotation;

        // Rotate the Player Object and the Camera around its Y axis
        transform.Rotate(Vector3.up * inputX);
    }
    
    void PlayerMove()
    {
        float vy = velocity.y;
        float inputX = 0, inputY = 0;

        if (Input.GetKey(KeyCode.W)) inputY += 1;
        if (Input.GetKey(KeyCode.A)) inputX -= 1;
        if (Input.GetKey(KeyCode.D)) inputX += 1;
        if (Input.GetKey(KeyCode.S)) inputY -= 1;

        Vector3 direction = (transform.forward * inputY + transform.right * inputX).normalized;

        velocity = direction * speed;
        velocity.y = vy;

        if (!isGrounded)
        {
            velocity += Vector3.down * LevelManager.instance.Gravity * Time.deltaTime;
        }

        if (wasWalking)
        {
            if (!isGrounded || (inputX == 0 && inputY ==0))
            {
                LevelManager.instance.audioMng.Stop("sfx_walk");
                wasWalking = false;
            }
        }
        else
        {
            if (isGrounded && (inputX != 0 || inputY != 0))
            {
                LevelManager.instance.audioMng.Play("sfx_walk");
                wasWalking = true;
            }
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void CheckJump()
    {
        if (isGrounded && jumping)
        {
            jumping = doubleJumping = false;
            LevelManager.instance.audioMng.Play("sfx_jump_impact");
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

    void CheckFallToDeath()
    {
        if (transform.position.y < -100)
        {
            //LevelManager.instance.audioMng.Play("falldead");
            StartCoroutine(LevelManager.instance.DieAfter(0));
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (hit.gameObject.tag == "cactus")
        {
            LevelManager.instance.audioMng.Play("dead");
            StartCoroutine(LevelManager.instance.DieAfter(1));
        }
        else if (hit.gameObject.tag == "win")
        {
            if (LevelManager.instance.IsPortalOn())
            {
                LevelManager.instance.audioMng.Play("sfx_victory");
                //LevelManager.instance.audioMng.Stop("bg");
                StartCoroutine(LevelManager.instance.DieAfter(4)); // 23
            }
        }
        else if (hit.gameObject.tag == "pickup_dj")
        {
            // sfx double jump pickup collected
            LevelManager.instance.audioMng.Play("sfx_dj_picked");
            canDoubleJump = true;
            Destroy(hit.gameObject);
        }
        else if (hit.gameObject.tag == "pickup_sp")
        {
            // sfx double jump pickup collected
            LevelManager.instance.audioMng.Play("sfx_sp_picked");
            LevelManager.instance.ObtainedKey();// add score
            Destroy(hit.gameObject);
        }
    }


    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.DrawSphere(transform.position + Vector3.up*footYoffset, sphereRadius);
    //}
}
