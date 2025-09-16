using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class KeyBoardInvertController : MonoBehaviour
{
    [Header("Movement Speed")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 1.6f;

    [Header("References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private CharacterController characterController;

    private Vector3 currentMovement;

    private void Awake()
    {
        if (playerInputHandler == null)
            playerInputHandler = GetComponent<PlayerInputHandler>();
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Normalen Input holen
        Vector2 input = playerInputHandler.MovementInput;

        // Spiegeln: W<->S (y-Achse), A<->D (x-Achse)
        Vector2 inverted = new Vector2(-input.x, -input.y);

        // In Welt umrechnen
        Vector3 move = transform.TransformDirection(new Vector3(inverted.x, 0, inverted.y));

        float speed = walkSpeed * (playerInputHandler.SprintPressed ? sprintMultiplier : 1f);
        currentMovement = move * speed;

        characterController.Move(currentMovement * Time.deltaTime);
    }
}
