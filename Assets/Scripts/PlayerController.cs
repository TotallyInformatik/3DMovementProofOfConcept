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
    float fov;
    [SerializeField] float xSensitivity = 200.0f;
    [SerializeField] float ySensitivity = 200.0f;
    private CameraController _cameraController;

    #endregion

    #region Private Variables

    float horizontalMovement;
    float verticalMovement;

    bool meleeOnCooldown;
    bool isGrounded;

    Vector3 moveDirection;
    private float yaw;

    Rigidbody rb;

    #endregion

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        _cameraController = GetComponent<CameraController>();
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f);

        HandleInput();
        HandleDrag();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            //Jump
            Jump();
        }

        if (isGrounded && new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude > 0.05f)
        {
            _cameraController.ViewBobbing();
        }

        if (Input.GetKeyDown(meleeKey) && !(meleeOnCooldown))
        {
            //Jump
            Melee();
        }
    }

    #region movement

    void Jump()
    {
        if (Input.GetKeyDown(sprintKey))
        {
            rb.AddForce(transform.up * jumpForce * 0.05f, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        _cameraController.AddPitchInput(mouseY * ySensitivity * 0.01f);

        yaw += mouseX * xSensitivity * 0.01f;
        transform.rotation = Quaternion.Euler(0, yaw, 0);


        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        _cameraController.HandleMovementTilt(transform.InverseTransformDirection(rb.linearVelocity), Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Mouse X"));
    }

    void MovePlayer()
    {
        rb.AddForce(moveDirection.normalized * (moveSpeed * (isGrounded ? movementMul : airMul)), ForceMode.Acceleration);
    }

    void HandleDrag()
    {
        if (isGrounded)
        {
            rb.linearDamping = rbDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
        }
    }

    #endregion


    #region combat

    void Melee()
    {
        meleeOnCooldown = true;

        Invoke(nameof(MeleeRaycast), meleeDelay);
        Invoke(nameof(ResetMelee), meleeCooldown);
    }

    void MeleeRaycast()
    {
        if (Physics.Raycast(_cameraController.cam.transform.position, _cameraController.cam.transform.forward, out RaycastHit hit, meleeRange, attackLayer))
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
        meleeOnCooldown = false;
    }

    #endregion
}