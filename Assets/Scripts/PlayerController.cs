using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityCharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float movementMul = 10f;
    public float rbDrag = 4f;
    public float airDrag = 2f;
    public float playerHeight = 2f;
    [SerializeField] float airMul = 5f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;

    [Header("Jumping")]
    public float jumpForce = 15f;
    public float launchForce = 3f;

    [Header("Camera")]
    public float defaultFov;
    public float sprintFov;
    float fov;
    Camera cam;

    float horizontalMovement;
    float verticalMovement;

    bool isGrounded;

    Vector3 moveDirection;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent < Rigidbody>();
        rb.freezeRotation = true;
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f);

        HandleInput();
        HandleDrag();

        if(Input.GetKeyDown(jumpKey) && isGrounded)
        {
            //Jump
            Jump();
        }

    }

    void Jump()
    {
        
        if (Input.GetKeyDown(sprintKey))

        {
            rb.AddForce(transform.up * jumpForce * 0.05f, ForceMode.Impulse);
            rb.AddForce(transform.forward * launchForce, ForceMode.Impulse);
        } else
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        rb.AddForce(moveDirection.normalized * moveSpeed * (isGrounded ? movementMul : airMul), ForceMode.Acceleration);

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
}