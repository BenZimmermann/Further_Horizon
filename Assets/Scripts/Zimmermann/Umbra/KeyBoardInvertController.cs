using UnityEngine;

// Ensure a CharacterController is attached to this GameObject
[RequireComponent(typeof(CharacterController))]
public class KeyBoardInvertController : MonoBehaviour
{
    [Header("Movement Speed")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 1.6f;

    [Header("References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;     // Handles player input
    [SerializeField] private CharacterController characterController;   // CharacterController component

    private Vector3 currentMovement;    // Stores current movement vector

    private void Awake()
    {
        // Automatically assign references if not set in Inspector
        if (playerInputHandler == null)
            playerInputHandler = GetComponent<PlayerInputHandler>();
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Handle movement every frame
        HandleMovement();
    }

    /// <summary>
    /// Handles player movement with keyboard input inversion.
    /// </summary>
    private void HandleMovement()
    {
        // Get standard movement input from player (x = left/right, y = forward/back)
        Vector2 input = playerInputHandler.MovementInput;

        // Invert input: W<->S (y-axis), A<->D (x-axis)
        Vector2 inverted = new Vector2(-input.x, -input.y);

        // Convert 2D input into world space movement relative to player rotation
        Vector3 move = transform.TransformDirection(new Vector3(inverted.x, 0, inverted.y));

        float speed = walkSpeed * (playerInputHandler.SprintPressed ? sprintMultiplier : 1f);
        currentMovement = move * speed;

        // Move the CharacterController
        characterController.Move(currentMovement * Time.deltaTime);
    }
}
