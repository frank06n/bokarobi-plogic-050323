using UnityEngine;

public class PlayerLook : MonoBehaviour
{

    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Vector3 offset;
    private Transform cameraTransform;

    private float cameraVerticalRotation = 0f;

    void Awake()
    {
        cameraTransform = Camera.main.transform;
    }
    void Update()
    {
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        RotateCameraVertically(inputY);
        RotateHorizontally(inputX);
    }

    private void RotateCameraVertically(float inputY)
    {
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation - inputY, -90f, 90f);

        Vector3 angles = cameraTransform.eulerAngles;
        angles.x = cameraVerticalRotation;
        angles.z = 0;
        cameraTransform.eulerAngles = angles;
    }
    private void RotateHorizontally(float inputX)
    {
        transform.Rotate(Vector3.up * inputX);
    }
}
