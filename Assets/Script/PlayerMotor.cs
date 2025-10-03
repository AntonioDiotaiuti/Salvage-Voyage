using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;

    [Header("Movimento")]
    public float speed = 5f;
    public float gravity = -9.81f;
    public float groundStickForce = -2f;

    [Header("Ground Check")]
    public LayerMask groundMask;         // layer terreno + ponte
    public float groundCheckDistance = 1.2f;
    private bool isGrounded;

    [Header("Boat Sync")]
    public Transform boatRoot;           // la root della barca (quella che segue AlignToWater)
    private Vector3 lastBoatPos;
    private Quaternion lastBoatRot;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (boatRoot != null)
        {
            lastBoatPos = boatRoot.position;
            lastBoatRot = boatRoot.rotation;
        }
    }

    void Update()
    {
        GroundCheck();

        if (boatRoot != null)
        {
            ApplyBoatOffset();
        }
    }

    // Riceve input dal tuo InputManager
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        // Gravità/Stick to ground
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = groundStickForce;
        }
        else
        {
            playerVelocity.y += gravity * Time.deltaTime;
        }

        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void GroundCheck()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(origin, Vector3.down, groundCheckDistance, groundMask);
    }

    /// <summary>
    /// Applica al player lo stesso delta movimento/rotazione della barca
    /// </summary>
    private void ApplyBoatOffset()
    {
        // calcola differenza posizione/rotazione della barca
        Vector3 deltaPos = boatRoot.position - lastBoatPos;
        Quaternion deltaRot = boatRoot.rotation * Quaternion.Inverse(lastBoatRot);

        // applica al player
        transform.position += deltaPos;
        transform.rotation = deltaRot * transform.rotation;

        // aggiorna riferimenti
        lastBoatPos = boatRoot.position;
        lastBoatRot = boatRoot.rotation;
    }
}
