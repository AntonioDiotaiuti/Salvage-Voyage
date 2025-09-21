using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BoatMovement : MonoBehaviour
{
    [Header("Impostazioni")]
    public float speed = 10f;       // Velocità avanti/indietro
    public float turnSpeed = 100f;  // Velocità di rotazione

    private Rigidbody rb;
    private Vector2 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Leggi input tastiera (WASD)
        float horizontal = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
        float vertical = (Input.GetKey(KeyCode.W) ? 1 : 0) + (Input.GetKey(KeyCode.S) ? -1 : 0);

        // Normalizza per evitare che la diagonale sia più veloce
        movementInput = new Vector2(horizontal, vertical).normalized;
    }

    void FixedUpdate()
    {
        // Movimento avanti/indietro
        Vector3 moveDirection = transform.forward * movementInput.y * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Rotazione (solo sull'asse Y)
        float turn = movementInput.x * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
