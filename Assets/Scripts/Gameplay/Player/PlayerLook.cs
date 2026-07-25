using UnityEngine;

public class PlayerLook : MonoBehaviour
{

    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Vector3 offset;
    private Transform cameraTransform;
    private float sensitivityMultiplier = 1f;

    private float cameraVerticalRotation = 0f;

    void Awake()
    {
        cameraTransform = Camera.main.transform;
    }
    void Update()
    {
        if (sensitivityMultiplier < 0.01f) return;

        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity * sensitivityMultiplier;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity * sensitivityMultiplier;

        RotateCameraVertically(inputY);
        RotateHorizontally(inputX);
    }

    public void SetSensitivityMultiplier(float m)
    {
        sensitivityMultiplier = m;
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
