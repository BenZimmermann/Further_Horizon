using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class EnhancedFirstPersonController : MonoBehaviour
{
    [Header("Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 1.6f;

    [Header("Jump / Gravity")]
    [SerializeField] private float jumpForce = 5.0f;
    [Tooltip("Multiplikator für Physics.gravity (1 = echtes -9.81 m/s²)")]
    [SerializeField] private float gravityMultiplier = 1.0f;

    [Header("Coyote / Buffer")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    [Header("Ground & Wall Detection")]
    [SerializeField, Tooltip("Max. Distanz für Center-Down Raycast")] private float groundCheckDistance = 0.15f;
   // [SerializeField, Tooltip("Distanz für Knie-Raycast in Bewegungsrichtung")] private float kneeCheckDistance = 0.6f;
    [SerializeField, Tooltip("Max Winkel (in Grad) bis zu dem eine Fläche als begehbar gilt")] private float slopeLimitDegrees = 45f;
    [SerializeField] private LayerMask groundLayer = ~0;

    [Header("Air Control / Responsiveness")]
    [SerializeField, Tooltip("Wie stark kann der Spieler in der Luft seine horizontale Geschwindigkeit ändern")] private float airAcceleration = 4f;
    [SerializeField, Tooltip("Wie schnell auf dem Boden die Geschwindigkeit erreicht wird")] private float groundAcceleration = 80f;
    [SerializeField, Tooltip("Faktor [0..1] der Input-Einfluss in der Luft (1 = volle Kontrolle)")] private float airControlFactor = 0.5f;
    [SerializeField, Tooltip("Maximale horizontale Geschw. in der Luft (wird beim Absprung gespeichert)")] private float maxAirSpeed = 7f;

    [Header("Look")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float mouseSensitivity = 0.125f;
    [SerializeField] private float upDownLookRange = 85f;

    [Header("References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;

    // intern
    private CharacterController cc;
    private Vector3 horizontalVelocity = Vector3.zero; // nur x,z
    private float verticalVelocity = 0f; // y-Komponente (m/s)
    private float storedAirSpeed = 0f; // horizontale max speed beim absprung
    private float coyoteTimer = 0f;
    private float jumpBufferTimer = 0f;
    private bool wasGroundedLastFrame = false;
    private float verticalRotation = 0f;
    private Vector3 lastGroundNormal = Vector3.up;
    private bool onTooSteepSlope = false;


    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!playerInputHandler) Debug.LogWarning("PlayerInputHandler nicht zugewiesen!");
        if (!mainCamera) mainCamera = Camera.main;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Inputs
        Vector2 movementInput = playerInputHandler != null ? playerInputHandler.MovementInput : Vector2.zero;
        Vector2 rotationInput = playerInputHandler != null ? playerInputHandler.RotationInput : Vector2.zero;
        bool jumpPressed = playerInputHandler != null ? playerInputHandler.JumpPressed : false;
        bool sprint = playerInputHandler != null ? playerInputHandler.SprintPressed : false;

        // Rotation
        HandleRotation(rotationInput);

        // Ground detection (eigene, zuverlässige)
        Vector3 desiredWorldDir = CalculateWorldDirection(movementInput);
        bool isGrounded = CustomGroundCheck(desiredWorldDir, out Vector3 groundNormal);

        // Coyote & Jump buffer
        UpdateCoyoteAndBuffer(isGrounded, jumpPressed);

        // Horizontal movement - different behavior on ground vs air
        float currentWalkSpeed = walkSpeed * (sprint ? sprintMultiplier : 1f);
        // prüfen, ob der Boden steiler als erlaubt ist
        float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);

        if (isGrounded && !onTooSteepSlope)
        {
            //  Normale Bodenbewegung
            Vector3 desiredOnPlane = Vector3.ProjectOnPlane(desiredWorldDir, groundNormal).normalized;
            Vector3 targetVelocity = desiredOnPlane * currentWalkSpeed;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, groundAcceleration * Time.deltaTime);

            if (verticalVelocity < 0f) verticalVelocity = -0.5f;
        }
        else if (isGrounded && onTooSteepSlope)
        {
            //  Steile Fläche -> nur nach unten rutschen
            Vector3 slideDir = Vector3.ProjectOnPlane(Vector3.down, groundNormal).normalized;
            horizontalVelocity = slideDir * currentWalkSpeed;

            // etwas extra nach unten drücken, damit man wirklich runterrutscht
            verticalVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime * 2f;
        }
        else
        {
            //  Luftbewegung
            Vector3 targetVelocity = desiredWorldDir * storedAirSpeed;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, airAcceleration * airControlFactor * Time.deltaTime);
            if (horizontalVelocity.magnitude > maxAirSpeed)
                horizontalVelocity = horizontalVelocity.normalized * maxAirSpeed;

            verticalVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            Debug.Log("Character controller:" + cc.isGrounded);
            Debug.Log("meiner: " + isGrounded);
        }



        // Handle jump
        if (CanPerformJump(isGrounded, groundNormal) && (jumpPressed || jumpBufferTimer > 0f))
        {
            PerformJump(currentWalkSpeed);
        }


        // Compose final movement vector and move CharacterController
        Vector3 move = horizontalVelocity + Vector3.up * verticalVelocity;
        CollisionFlags flags = cc.Move(move * Time.deltaTime);

        // If we hit ceiling, zero vertical upward velocity
        if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0f)
        {
            verticalVelocity = 0f;
        }

        // Update wasGroundedLastFrame using our custom check (not cc.isGrounded)
        wasGroundedLastFrame = isGrounded;
    }

    private Vector3 CalculateWorldDirection(Vector2 input)
    {
        Vector3 inputDir = new Vector3(input.x, 0f, input.y);
        Vector3 worldDir = transform.TransformDirection(inputDir);
        worldDir.y = 0f;
        return worldDir.normalized;
    }

    private bool CustomGroundCheck(Vector3 movementDir, out Vector3 groundNormal)
    {
        Vector3 origin = transform.position + cc.center;
        groundNormal = Vector3.up;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore))
        {
            groundNormal = hit.normal;

            // berechne den Winkel zwischen Welt-Up und Boden-Normale
            float angle = Vector3.Angle(Vector3.up, groundNormal);

            if (angle <= slopeLimitDegrees)
            {
                // begehbarer Boden
                onTooSteepSlope = false;
                return true;
            }
            else if (angle > slopeLimitDegrees && angle < 89f)
            {
                //  zu steil
                Debug.Log($"Zu steile Fläche! angle={angle:F1}° an Position {hit.point}");
                onTooSteepSlope = true;
                return false;
            }
            else if (angle >= 89f)
            {
                //  Wand
                Debug.Log($"Wand vor Spieler! angle={angle:F1}° an Position {hit.point}");
                onTooSteepSlope = false;
                return false;
            }
        }

        // gar nichts unter den Füßen = Abgrund
        Debug.Log("Abgrund erkannt: kein Boden unter Spieler!");
        onTooSteepSlope = false;
        return false;
    }






    private void UpdateCoyoteAndBuffer(bool isGrounded, bool jumpPressed)
    {
        // Coyote
        if (wasGroundedLastFrame && !isGrounded && verticalVelocity <= 0f)
            coyoteTimer = coyoteTime;

        if (isGrounded)
            coyoteTimer = 0f;
        else
            coyoteTimer -= Time.deltaTime;

        // Jump buffer
        if (jumpPressed)
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;
    }

    private bool CanPerformJump(bool isGrounded, Vector3 groundNormal)
    {
        float slopeAngle = Vector3.Angle(groundNormal, Vector3.up);
        if (!isGrounded || slopeAngle > slopeLimitDegrees)
            return false; // kein Sprung auf zu steilen Flächen

        return (coyoteTimer > 0f) || cc.isGrounded;
    }



    private void PerformJump(float currentWalkSpeed)
    {
        // Only allow jump if either cc.isGrounded via the real controller OR coyote timer positive
        // Prevent multiple triggers by consuming jump buffer
        if (jumpBufferTimer <= 0f && coyoteTimer <= 0f) return;

        // set vertical
        verticalVelocity = jumpForce;
        // store current horizontal max for air behaviour
        storedAirSpeed = Mathf.Max(currentWalkSpeed, horizontalVelocity.magnitude);
        if (storedAirSpeed <= 0.01f) storedAirSpeed = walkSpeed;

        // consume timers
        coyoteTimer = 0f;
        jumpBufferTimer = 0f;
    }

    private void HandleRotation(Vector2 rotationInput)
    {
        float mouseX = rotationInput.x * mouseSensitivity;
        float mouseY = rotationInput.y * mouseSensitivity;

        // horizontal
        transform.Rotate(0f, mouseX, 0f);

        // vertical
        verticalRotation = Mathf.Clamp(verticalRotation - mouseY, -upDownLookRange, upDownLookRange);
        if (mainCamera) mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    // Debug-visualisierung im Editor
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            //// draw expected rays for config
            //Vector3 centerOrigin = transform.position + (cc ? cc.center : Vector3.up * 1f);
            //Gizmos.color = Color.cyan;
            //Gizmos.DrawLine(centerOrigin, centerOrigin + Vector3.down * groundCheckDistance);

            //Vector3 kneeOrigin = transform.position + Vector3.up * ((cc) ? cc.height * 0.4f : 0.8f);
            //Gizmos.color = Color.yellow;
            //// Use forward for visualization
            //Gizmos.DrawLine(kneeOrigin, kneeOrigin + transform.forward * kneeCheckDistance);
        }
    }
}
