using UnityEngine;

public class HandheldCamera : MonoBehaviour
{
    // Variables to control the intensity of the camera movement
    public float positionIntensity = 0.05f;  // How much the camera moves in position
    public float rotationIntensity = 0.1f;   // How much the camera rotates (handheld shake)

    // Variables to control the speed of the noise
    public float positionSpeed = 1.0f;
    public float rotationSpeed = 1.5f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        // Store the initial position and rotation of the camera
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Apply Perlin noise to simulate handheld camera shake for position
        float posX = Mathf.PerlinNoise(Time.time * positionSpeed, 0f) - 0.5f;
        float posY = Mathf.PerlinNoise(0f, Time.time * positionSpeed) - 0.5f;
        float posZ = Mathf.PerlinNoise(Time.time * positionSpeed, Time.time * positionSpeed) - 0.5f;

        // Update position by adding random noise
        transform.localPosition = initialPosition + new Vector3(posX, posY, posZ) * positionIntensity;

        // Apply Perlin noise to simulate handheld camera shake for rotation
        float rotX = (Mathf.PerlinNoise(Time.time * rotationSpeed, 0f) - 0.5f) * rotationIntensity;
        float rotY = (Mathf.PerlinNoise(0f, Time.time * rotationSpeed) - 0.5f) * rotationIntensity;
        float rotZ = (Mathf.PerlinNoise(Time.time * rotationSpeed, Time.time * rotationSpeed) - 0.5f) * rotationIntensity;

        // Update rotation by adding random noise
        transform.localRotation = initialRotation * Quaternion.Euler(rotX, rotY, rotZ);
    }
}
