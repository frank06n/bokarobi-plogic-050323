using UnityEngine;

public class PickupLogic : MonoBehaviour
{
    [SerializeField] private Transform innerObject;

    [Header("Hover Settings")]
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float maxYChange;
    [SerializeField] private bool randomizeInitialState;

    private Vector3 initialPosition;
    private Vector3 eulerAngles;
    private float timeElapsed;

    private bool alreadyCollected = false;
    
    void Awake()
    {
        InitializeHoveringValues();
    }

    void Update()
    {
        PerformHovering();
        PerformInnerRotation();
        
        
        timeElapsed += Time.deltaTime;
    }

    private void InitializeHoveringValues()
    {
        initialPosition = transform.position;
        eulerAngles = innerObject.rotation.eulerAngles;

        if (randomizeInitialState)
        {
            timeElapsed = Random.Range(1f, 4f);
        }
    }

    private void PerformHovering()
    {
        if (hoverSpeed == 0 || maxYChange == 0) return;
        transform.position = initialPosition + Vector3.up * maxYChange * Mathf.Sin(hoverSpeed * timeElapsed);
    }

    private void PerformInnerRotation()
    {
        if (rotationSpeed == 0) return;
        eulerAngles.y += rotationSpeed * Time.deltaTime * 45;
        innerObject.rotation = Quaternion.Euler(eulerAngles * Mathf.PI);
    }

    public bool TryToCollect()
    {
        if (alreadyCollected) return false;
        return true;
    }

}
