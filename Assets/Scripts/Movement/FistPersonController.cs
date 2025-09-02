using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speed")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 1.6f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 9.81f;

    [Header("Air Control")]
    [SerializeField, Range(0f, 1f), Tooltip("control in the air 0 is none 1 full control")] private float airControl = 0.2f;

    [Header("Coyote Time Parameters")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSensitivity = 0.125f;
    [SerializeField] private float upDownLookRange = 85.0f;

    [Header("Ground Check Parameters")]
    [SerializeField, Tooltip("radius of ground-check")] private float sphereRadius = 0.4f;
    [SerializeField, Tooltip("distance between ray and player")] private float groundCheckDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    public bool isGround;

    private Vector3 currentMovement;
    private float verticalRotation;
    private float airSpeed;
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool wasGrounded;

    private float CurrentSpeed => GetMovementSpeed();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    #region Update
    void Update()
    {
        GroundCheck(); // Call ground check first to update isGround
        HandleCoyoteTime();
        HandleJumpBuffer();
        HandleMovement();
        HandleRotation();
        WallCheck();
     //   Debug.Log("Ist auf boden?(cc´s) " + characterController.isGrounded);
        Debug.Log("Ist auf boden?(custom) " + isGround);
    }
    #endregion

    #region CoyoteTime
    private void HandleCoyoteTime()
    {
        bool isGrounded = isGround; // Use custom ground detection

        // Start coyote timer when leaving ground (not from jumping)
        if (wasGrounded && !isGrounded && currentMovement.y <= 0)
        {
            coyoteTimer = coyoteTime;
        }

        // Reset timer when grounded
        if (isGrounded)
        {
            coyoteTimer = 0f;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        wasGrounded = isGrounded;
    }
    #endregion

    #region JumpBuffer
    private void HandleJumpBuffer()
    {
        // Start jump buffer when jump input is pressed
        if (playerInputHandler.JumpPressed)
        {
            jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
    }
    // Can jump if grounded OR within coyote time
    #endregion

    #region CanJump/HasJumpInput/GetMovementSpeed
    private bool CanJump()
    {
        return isGround || coyoteTimer > 0f; // Use custom ground detection
    }

    private bool HasJumpInput()
    {
        return playerInputHandler.JumpPressed;
    }

    private float GetMovementSpeed()
    {
        if (isGround) // Use custom ground detection
        {
            return walkSpeed * (playerInputHandler.SprintPressed ? sprintMultiplier : 1);
        }
        else
        {
            return airSpeed;
        }
    }
    #endregion

    #region worldDirection
    private Vector3 CalculateWorldDirection()
    {
        Vector3 inputDirection = new Vector3(playerInputHandler.MovementInput.x, 0f, playerInputHandler.MovementInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        return worldDirection.normalized;
    }
    #endregion

    #region handleJump
    private void HandleJump()
    {
        if (isGround) // Use custom ground detection
        {
            currentMovement.y = -0.5f;
        }

        // Jump if we can jump AND have jump input
        if (CanJump() && HasJumpInput())
        {
            // Store current speed for air movement
            airSpeed = walkSpeed * (playerInputHandler.SprintPressed ? sprintMultiplier : 1);
            currentMovement.y = jumpForce;

            // Reset timers after jumping
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
        }
        else if (!isGround) // Use custom ground detection
        {
            // Apply gravity when not grounded
            currentMovement.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }
    }
    #endregion

    #region handleMovement
    private void HandleMovement()
    {
        Vector3 worldDirection = CalculateWorldDirection();
        float currentSpeed = GetMovementSpeed();

        if (isGround) // Use custom ground detection
        {
            // Am Boden: volle Kontrolle
            currentMovement.x = worldDirection.x * currentSpeed;
            currentMovement.z = worldDirection.z * currentSpeed;
        }
        else
        {
            // In der Luft: mische alte Richtung mit Input
            Vector3 airMove = worldDirection * currentSpeed;
            currentMovement.x = Mathf.Lerp(currentMovement.x, airMove.x, airControl * Time.deltaTime * 10f);
            currentMovement.z = Mathf.Lerp(currentMovement.z, airMove.z, airControl * Time.deltaTime * 10f);
        }

        HandleJump();
        characterController.Move(currentMovement * Time.deltaTime);
    }
    #endregion

    #region handleRotation
    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);
    }

    private void ApplyVerticalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation()
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSensitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSensitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVerticalRotation(mouseYRotation);
    }
    #endregion

    #region IsGrounded
    private void GroundCheck()
    {
        Vector3 sphereOrigin = transform.position + Vector3.up * 0.7f; // Start etwas über dem Boden
        isGround = Physics.SphereCast(sphereOrigin, sphereRadius, Vector3.down, out RaycastHit hitInfo, groundCheckDistance, groundMask);
       // Debug.Log("Ist auf Boden?(meiner) " + isGround);
#if UNITY_EDITOR
            // Debug: Richtung anzeigen
            Debug.DrawRay(sphereOrigin, Vector3.down * groundCheckDistance, isGround ? Color.green : Color.red);
#endif
    }
    #endregion

    #region WallCheck
    private bool WallCheck()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.3f;
        Vector3 dir = transform.forward;

        if (Physics.Raycast(rayOrigin, dir, out RaycastHit hitInfo, 0.7f))
        {
            float angle = Vector3.Angle(-dir, hitInfo.normal);

            Debug.Log($"Schaut Wand an? true | Winkel zur Wand-Normal: {angle:F1}°");

#if UNITY_EDITOR
            Debug.DrawRay(rayOrigin, dir * 0.7f, Color.green);
            Debug.DrawRay(hitInfo.point, hitInfo.normal * 0.5f, Color.yellow); // Normal sichtbar
#endif
            return true;
        }

#if UNITY_EDITOR
        Debug.DrawRay(rayOrigin, dir * 0.7f, Color.red);
#endif

        Debug.Log("Schaut Wand an? false");
        return false;
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 sphereOrigin = transform.position + Vector3.up * 0.7f;

        // Position der Kugel am Ende des SphereCasts
        Vector3 sphereEnd = sphereOrigin + Vector3.down * groundCheckDistance;

        // Kugel am Start
        Gizmos.DrawWireSphere(sphereOrigin, sphereRadius);

        // Kugel am Ende (falls Bodenkontrolle bis dahin reicht)
        Gizmos.DrawWireSphere(sphereEnd, sphereRadius);
    }
    #endregion
}