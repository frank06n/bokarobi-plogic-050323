using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float hoverSpeed;
    [SerializeField] private float maxYChange;
    [SerializeField] private bool randomizeInitialState;

    private Vector3 initialPosition;
    private Vector3 eulerAngles;
    private float timeElapsed;
    
    void Awake()
    {
        initialPosition = transform.position;
        eulerAngles = transform.rotation.eulerAngles;

        if (randomizeInitialState)
        {
            timeElapsed = Random.Range(1f, 4f);
        }
    }

    void Update()
    {
        transform.position = initialPosition + Vector3.up * maxYChange * Mathf.Sin(hoverSpeed * timeElapsed);
        
        eulerAngles.y += rotationSpeed * Time.deltaTime * 45;
        transform.rotation = Quaternion.Euler(eulerAngles*Mathf.PI);
        
        timeElapsed += Time.deltaTime;
    }
}
