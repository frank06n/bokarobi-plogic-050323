using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{

    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform handsTransform;
    [SerializeField] private Vector3 offset;
    private Transform camTransform;

    private float camRotVert = 0f;
    private Vector3 handsRotSVel; // hands rotating smooth velocity

    // Start is called before the first frame update
    void Awake()
    {
        camTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Look();
    }

    private void LateUpdate ()
    {
        handsTransform.forward = Vector3.SmoothDamp(handsTransform.forward, camTransform.forward, ref handsRotSVel, 0.05f);
    }

    void Look()
    {
        // Collect Mouse Input
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity;


        camRotVert = Mathf.Clamp(camRotVert - inputY, -90f, 90f);

        Vector3 camEuler = camTransform.eulerAngles;
        camEuler.x = camRotVert;
        //camEuler.y += inputX;
        camEuler.z = 0;
        camTransform.eulerAngles = camEuler;




        transform.Rotate(Vector3.up * inputX);
    }
}
