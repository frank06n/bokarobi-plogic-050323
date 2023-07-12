using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    [SerializeField] private float rotSpeed;
    [SerializeField] private float hoverYChange;
    [SerializeField] private float hoverRate;
    [SerializeField] private bool randomStart;
    // bool use rigidbody

    private Vector3 initialYPos;
    private Vector3 euler;
    private float delta;
    
    void Awake()
    {
        initialYPos = transform.position;
        euler = transform.rotation.eulerAngles;
        if (randomStart)
        {
            delta = Random.Range(1f, 4f);
            euler.y += delta*45;
        }
    }

    void Update()
    {
        transform.position = initialYPos + Vector3.up * hoverYChange * Mathf.Sin(hoverRate * delta);
        euler.y += rotSpeed * Time.deltaTime * 45;
        transform.rotation = Quaternion.Euler(euler*Mathf.PI);
        delta += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // if this is pickup, set player picked up
    }
}
