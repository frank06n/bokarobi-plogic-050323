using UnityEngine;

// Runs before the player so LastMoveDelta is updated before the player reads it.
[DefaultExecutionOrder(-100)]
public class Platform : MonoBehaviour
{
    [Header("Surface Properties")]
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float jumpMultiplier = 1f;
    [SerializeField] private float bounciness = 0f;
    [SerializeField] private float bounceVelocityThreshold = 5f;

    [Header("Movement")]
    [SerializeField] private bool isMoving = false;

    [Min(0.01f)]
    [SerializeField] private float movementSpeed = 1f;

    [Min(0.01f)]
    [SerializeField] private float movementTime = 1f;

    [SerializeField] private Vector3 finalPosition = Vector3.up;

    // [Header("Sfx")]
    // [SerializeField] private Audio walkSound;
    // [SerializeField] private Audio jumpLandSound;
    // [SerializeField] private Audio jumpBounceSound;

    private Vector3 initialPosition;
    private Vector3 lastMoveDelta;
    private float timeElapsed;

    public Vector3 LastMoveDelta => lastMoveDelta;
    public float SpeedMultiplier => speedMultiplier;
    public float JumpMultiplier => jumpMultiplier;
    public float Bounciness => bounciness;
    public float BounceVelocityThreshold => bounceVelocityThreshold;
    // public Audio WalkSound => walkSound;
    // public Audio JumpLandSound => jumpLandSound;
    // public Audio JumpBounceSound => jumpBounceSound;

    private void Awake()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        if (!isMoving)
        {
            lastMoveDelta = Vector3.zero;
            return;
        }

        float offset = Mathf.Sin(timeElapsed * movementSpeed);

        Vector3 newPosition = initialPosition + finalPosition * offset;

        lastMoveDelta = newPosition - transform.position;
        transform.position = newPosition;

        timeElapsed += Time.deltaTime;
    }

    // Editor-only: Keeps Movement Speed and Movement Time synchronized in the Inspector.
#if UNITY_EDITOR
    private float previousMovementSpeed;
    private float previousMovementTime;
    private Vector3 previousFinalPosition;

    private void OnValidate()
    {
        float distance = finalPosition.magnitude;

        movementSpeed = Mathf.Max(0.01f, movementSpeed);
        movementTime = Mathf.Max(0.01f, movementTime);

        if (distance <= Mathf.Epsilon)
        {
            movementTime = 0f;
            previousMovementSpeed = movementSpeed;
            previousMovementTime = movementTime;
            previousFinalPosition = finalPosition;
            return;
        }

        if (previousMovementSpeed != movementSpeed)
        {
            movementTime = distance / movementSpeed;
        }
        else if (previousMovementTime != movementTime)
        {
            movementSpeed = distance / movementTime;
        }
        else if (previousFinalPosition != finalPosition)
        {
            movementTime = distance / movementSpeed;
        }

        previousMovementSpeed = movementSpeed;
        previousMovementTime = movementTime;
        previousFinalPosition = finalPosition;
    }
#endif

    private void OnDrawGizmos()
    {
        if (!isMoving)
            return;

        Vector3 start = Application.isPlaying ? initialPosition : transform.position;
        Vector3 end = start + finalPosition;

        Gizmos.color = Color.white;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.15f);
    }
}