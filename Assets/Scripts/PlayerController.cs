using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Movement variables

    [Header("Movement")] [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float movementMul = 10f;
    [SerializeField] private float rbDrag = 4f;
    [SerializeField] private float airDrag = 2f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float airMul = 5f;

    [Header("Jumping")] [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float apexStrength = 10f;
    [SerializeField] private float apexCriticalEdge = 0.2f;

    #endregion

    #region Keybinds

    [Header("Keybinds")] [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode meleeKey = KeyCode.Mouse0;

    #endregion

    #region Combat

    [Header("Melee")] public float meleeCooldown = 20f;
    public float meleeDelay = 4f;
    public float meleeRange = 3f;
    public int meleeDamage = 1;
    public LayerMask attackLayer;

    #endregion

    #region Camera

    [Header("Camera")] public float defaultFov;
    public float sprintFov;
    float _fov;
    [SerializeField] float xSensitivity = 200.0f;
    [SerializeField] float ySensitivity = 200.0f;
    private CameraController _cameraController;

    #endregion

    #region Private Variables

    float _horizontalMovement;
    float _verticalMovement;
    private bool _reachingApex = false;

    bool _meleeOnCooldown;
    bool _isGrounded;

    Vector3 _moveDirection;
    private float _yaw;

    Rigidbody _rb;

    #endregion

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _cameraController = GetComponent<CameraController>();
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

        if (_isGrounded)
        {
            if (new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.z).magnitude > 0.05f)
            {
                _cameraController.ViewBobbing();
            }
            if (Input.GetKeyDown(jumpKey))
            {
                //Jump
                Jump();
                //_cameraController.JumpSpasm();
            }
        }
        
        if (Input.GetKeyDown(meleeKey) && !(_meleeOnCooldown))
        {
            //Jump
            Melee();
        }
    }

    #region movement

    void Jump()
    {
        _reachingApex = true;
        if (Input.GetKeyDown(sprintKey))
        {
            _rb.AddForce(transform.up * (jumpForce * 0.05f), ForceMode.Impulse);
        }
        else
        {
            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
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
        MovePlayer();
        _cameraController.HandleMovementTilt(transform.InverseTransformDirection(_rb.linearVelocity),
            Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Mouse X"));

        if (_reachingApex && _rb.linearVelocity.y < apexCriticalEdge)
        {
            _reachingApex = false;
            _rb.AddForce(Vector3.down * apexStrength, ForceMode.VelocityChange);
            //Debug.Log("will this affect lebron's legacy?");
        }
    }

    void MovePlayer()
    {
        _rb.AddForce(_moveDirection.normalized * (moveSpeed * (_isGrounded ? movementMul : airMul)),
            ForceMode.Acceleration);
    }

    void HandleDrag()
    {
        if (_isGrounded)
        {
            _rb.linearDamping = rbDrag;
        }
        else
        {
            _rb.linearDamping = airDrag;
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
}