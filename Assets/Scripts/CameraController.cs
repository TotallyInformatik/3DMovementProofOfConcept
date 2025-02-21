using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float xSensitivity;
    [SerializeField] float ySensitivity;

    Camera cam;

    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRot;
    float yRot;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() 
    {
        HandleInput();

        cam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    void HandleInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
        
        yRot += mouseX * xSensitivity * multiplier;
        xRot -= mouseY * ySensitivity * multiplier;

        xRot = Mathf.Clamp(xRot, -90f, 90f);
    }
}