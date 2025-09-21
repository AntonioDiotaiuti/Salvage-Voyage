using UnityEngine;

public class Movement : MonoBehaviour
{
   
    public float moveSpeed = 10f;
    public float turnSpeed = 50f;

    void Update()
    {
        float forward = Input.GetAxis("Vertical");   // W/S
        float turn = Input.GetAxis("Horizontal");    // A/D

        // Usa l’asse X come prua (invece di Z)
        Vector3 moveDir = transform.right; // red axis

        // Movimento avanti/indietro lungo l’asse forward
        Vector3 movement = moveDir * forward * moveSpeed * Time.fixedDeltaTime;
        transform.position += movement;

        // Rotazione (asse Y, timone)
        transform.Rotate(Vector3.up, turn * turnSpeed * Time.deltaTime);
    }
}
