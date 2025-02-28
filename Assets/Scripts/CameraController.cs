using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera cam;
    [SerializeField] Transform cameraParent;

    [SerializeField] private float targetTilt;
    [SerializeField] private float maxTilt;
    private float pitch, roll;
    private float xRot;
    private float lastYRot;
    [SerializeField] private float slerpFactor = 3f;
    [SerializeField] private bool enableViewBobbing = true;
    [SerializeField] private float viewBobAmplitude = 0.0004f;
    [SerializeField] private float viewBobFrequency = 7f;
    
    private Vector3 startPos;
    private Quaternion _targetRotation;


    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        startPos = cam.transform.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        cameraParent.transform.localRotation = Quaternion.Slerp(cameraParent.transform.localRotation, _targetRotation,
            Time.deltaTime * slerpFactor);
    }

    public void AddPitchInput(float amount)
    {
        xRot -= amount;
        cam.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
    }

    #region Cosmetic Camera Effects

    public void HandleMovementTilt(Vector3 velocity, float forward, float right, float lookRight)
    {
        velocity.Normalize();

        float forwardFactor = forward;
        if (Mathf.Abs(velocity.z) < Mathf.Epsilon) forwardFactor = 0;
        float rightFactor = -right;
        if (Mathf.Abs(velocity.x) < Mathf.Epsilon) rightFactor = 0;

        pitch = Mathf.Clamp(Mathf.Lerp(-targetTilt, targetTilt, (forwardFactor + 0.5f)), -maxTilt, maxTilt);
        roll = Mathf.Clamp(Mathf.Lerp(-targetTilt, targetTilt, (rightFactor + 0.5f)), -maxTilt, maxTilt);

        _targetRotation = Quaternion.Euler(pitch, 0, roll);
        lastYRot = transform.rotation.y;
    }

    private void ResetPosition()
    {
        if (cam.transform.localPosition != startPos)
        {
            cam.transform.localPosition = Vector3.Slerp(cam.transform.localPosition, startPos, Time.deltaTime);
        }
    }

    public void JumpSpasm()
    {
        _targetRotation = Quaternion.Euler(-maxTilt * 10, 0, 0);
    }

    public void ImpactJerk()
    {
        _targetRotation = Quaternion.Euler(maxTilt * 10, 0, 0);
    }

    public void ViewBobbing()
    { 
        Vector3 offset = Vector3.zero;
        offset.y += Mathf.Sin(Time.time * viewBobFrequency) * viewBobAmplitude;
        offset.x += Mathf.Cos(Time.time * viewBobFrequency / 2) * viewBobAmplitude * 2;
        cam.transform.localPosition += offset;
        ResetPosition();
    }
    
    #endregion
}