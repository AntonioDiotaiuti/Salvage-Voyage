using UnityEngine;

public class PlayerOnBoatHandler : MonoBehaviour
{
    [Header("Riferimenti")]
    public Transform boatAnchor;
    public PlayerMotor motor;

    [Header("Impostazioni")]
    public LayerMask ponteMask;        // Tutti i collider dei ponti
    public float groundCheckDistance = 3f;
    public float snapThreshold = 1.2f; // quanto distante il “pavimento” può essere considerato valido
    public float smoothFollow = 10f;

    private bool onBoat = false;

    void Update()
    {
        if (onBoat)
        {
            StickToBoat();
        }
    }

    public void EnterBoat()
    {
        onBoat = true;
        motor.enabled = true;
        transform.SetParent(boatAnchor, true);
        SnapToAnchor();
    }

    public void ExitBoat()
    {
        onBoat = false;
        transform.SetParent(null, true);
    }

    private void SnapToAnchor()
    {
        transform.position = boatAnchor.position;
        transform.rotation = boatAnchor.rotation;
    }

    private void StickToBoat()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundCheckDistance, ponteMask))
        {
            // Usa solo il ponte se non è troppo distante dalla posizione attuale
            float diff = transform.position.y - hit.point.y;
            if (diff >= -0.1f && diff <= snapThreshold)
            {
                Vector3 targetPos = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothFollow);
            }
        }
    }
}
