using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    #region Movement variables

    [Header("Movement")] [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float airMovementMul = 0.02f;
    [SerializeField] private float rbDrag = 10f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float maxSpeed = 10;

    [Header("Jumping")] 
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    private bool readyToJump = true;

    #endregion

    #region Keybinds

    [Header("Keybinds")] [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode meleeKey = KeyCode.Mouse0;
    [SerializeField] KeyCode dashKey = KeyCode.LeftControl;
    [SerializeField] KeyCode dropKey = KeyCode.Q;

    #endregion

    #region Combat

    [Header("Melee")] public float meleeCooldown = 0.5f;
    public float meleeDelay = 0.04f;
    public float meleeRange = 3f;
    public int meleeDamage = 1;
    public LayerMask attackLayer;

    #endregion

    #region Dash

    [Header("Dash")] public float dashCooldown = 2f;
    public float dashDelay = 0.1f;
    public float dashDuration = 0.1f;
    public float dashForce = 90f;

    #endregion

    #region Camera

    [Header("Camera")] public float defaultFov;
    public float sprintFov;
    float _fov;
    [SerializeField] float xSensitivity = 200.0f;
    [SerializeField] float ySensitivity = 200.0f;
    private CameraController _cameraController;

    #endregion

    #region Drop

    [Header("Drop")] public float dropSpeed = 20;
    public float dropDelay = 0.1f;
    public float dropCooldown = 1.5f;

    #endregion

    #region Private Variables

    float _horizontalMovement;
    float _verticalMovement;

    bool _meleeOnCooldown;
    bool _dashOnCooldown;
    bool _dropOnCooldown;
    bool _isGrounded;
    bool _imDropping;
    bool _dashing = false;

    Vector3 _moveDirection;
    private float _yaw;

    Rigidbody _rb;

    #endregion

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _cameraController = GetComponent<CameraController>();
        _cameraController.SetFOV(defaultFov);
    }

    void Update()
    {


        bool newIsGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f);

        if (newIsGrounded && !_isGrounded)
        {
            _cameraController.ImpactJerk();
        }
             
        _isGrounded = newIsGrounded;

        HandleInput();
        HandleDrag();


        if (Input.GetKey(jumpKey) && _isGrounded && readyToJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }


        if (_isGrounded)
        {
            if (_imDropping) {
                _imDropping = false;
                _rb.AddForce(new Vector3(0, 30, 0), ForceMode.VelocityChange);
            }
            if (new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.z).magnitude > 0.05f)
            {
                _cameraController.ViewBobbing();
            }
        }
        

        if (Input.GetKeyDown(meleeKey) && !(_meleeOnCooldown))
        {
            Melee();
        }

        if (Input.GetKeyDown(dashKey) && !(_dashOnCooldown))
        {
            Dash();
        }

        if (Input.GetKeyDown(dropKey) && !(_imDropping) && !(_dropOnCooldown))
        {
            Drop();
        }
    }

    #region movement

    void Jump()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    void HandleInput()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        _cameraController.AddPitchInput(mouseY * ySensitivity * 0.01f);

        _yaw += mouseX * xSensitivity * 0.01f;
        transform.rotation = Quaternion.Euler(0, _yaw, 0);

        _moveDirection = transform.forward * _verticalMovement + transform.right * _horizontalMovement;


    }

    private void FixedUpdate()
    {
        Debug.Log(_rb.linearVelocity.magnitude);
        MovePlayer();

        _cameraController.HandleMovementTilt(transform.InverseTransformDirection(_rb.linearVelocity),
            Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Mouse X"));

    }

    void MovePlayer()
    {
        // on ground
        if(_isGrounded)
        {
            _rb.AddForce(_moveDirection.normalized * moveSpeed * 10f, ForceMode.Impulse);
        }

        // in air

        else if(!_isGrounded && (_rb.linearVelocity.magnitude < maxSpeed || (_rb.linearVelocity + _moveDirection).magnitude <= _rb.linearVelocity.magnitude))
        {
            _rb.AddForce(_moveDirection.normalized * moveSpeed * 10f * airMovementMul, ForceMode.Impulse);
        }
        
    }

    
    void HandleDrag()
    {
        if (_isGrounded)
        {
            _rb.linearDamping = rbDrag;
        }
        else
        {
            _rb.linearDamping = 0;
        }
    }

    #endregion


    #region combat

    void Melee()
    {
        _meleeOnCooldown = true;

        Invoke(nameof(MeleeRaycast), meleeDelay);
        Invoke(nameof(ResetMelee), meleeCooldown);
    }

    void MeleeRaycast()
    {
        if (Physics.Raycast(_cameraController.cam.transform.position, _cameraController.cam.transform.forward,
                out RaycastHit hit, meleeRange, attackLayer))
        {
            HitTarget(hit.point);
            if (hit.transform.TryGetComponent<Rigidbody>(out Rigidbody R))
            {
                hit.transform.GetComponent<EnemyFollow>().Hit();
                R.AddForceAtPosition(hit.point, _cameraController.cam.transform.forward * 600f);
            }
        }
    }

    void HitTarget(Vector3 point)
    {
        Debug.Log("something got hit");
    }

    void ResetMelee()
    {
        _meleeOnCooldown = false;
    }

    #endregion

    #region dash

    void Dash()
    {
        _dashOnCooldown = true;

        Invoke(nameof(DashAction), dashDelay);
        Invoke(nameof(ResetDash), dashCooldown);
    }

    void DashAction()
    {
        Debug.Log("Dash!");
        _imDropping = false;
        _dashing = true;

        _rb.useGravity = false;
        _rb.linearDamping = 0;
        _rb.AddForce(new Vector3(0, _rb.linearVelocity.y * -1f, 0), ForceMode.VelocityChange);
        _rb.AddForce(_cameraController.cam.transform.forward * dashForce, ForceMode.VelocityChange);

        _cameraController.AlterFOV(5);

        Invoke(nameof(EndDash), dashDuration);

    }
    void EndDash() {
        _dashing = false;
        _rb.useGravity = true;
        Debug.Log("Lebown Games");
        _rb.AddForce(new Vector3(_rb.linearVelocity.x * -.90f, _rb.linearVelocity.y * -1f, _rb.linearVelocity.z * -.9f), ForceMode.VelocityChange);
        _cameraController.AlterFOV(-5);
    }

    void ResetDash()
    {
        _dashOnCooldown = false;
    }

    #endregion

    #region drop

    void Drop()
    {
        _dropOnCooldown = true;
        _imDropping = true;

        Invoke(nameof(DropAction), dropDelay);
        Invoke(nameof(ResetDrop), dropCooldown);
    }

    void DropAction()
    {
        _rb.AddForce(new Vector3(_rb.linearVelocity.x * -1f, -dropSpeed, _rb.linearVelocity.z * -1f), ForceMode.VelocityChange);
    }

    void ResetDrop()
    {
        _dropOnCooldown = false;
        _imDropping = false;
    }

    #endregion

}