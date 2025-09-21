using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Velocità")]
    public float moveSpeed = 10f;
    public float turnSpeed = 50f;

    [Header("Collision check")]
    public Collider modelCollider;
    public float checkDistanceMultiplier = 1.1f;
    public LayerMask obstacleMask;

    // Stato (true se c'è un ostacolo davanti)
    private bool blockedForward = false;

    void Start()
    {
        
        if (obstacleMask.value == 0)
        {
            int waterLayer = LayerMask.NameToLayer("Water");
            if (waterLayer != -1)
            {
                // tutto tranne Water
                obstacleMask = ~(1 << waterLayer);
            }
            else
            {
                // se non esiste layer Water, lascia tutto 
                obstacleMask = ~0;
            }
        }
    }

    void Update()
    {
        float forward = Input.GetAxis("Vertical");   // W/S
        float turn = Input.GetAxis("Horizontal");    // A/D

        if (blockedForward && forward < 0f)
        {
            forward = 0f;
        }

        // Usa l’asse X come prua 
        Vector3 moveDir = transform.right; 

        // Movimento avanti/indietro lungo l’asse forward
        Vector3 movement = moveDir * forward * moveSpeed * Time.deltaTime;
        transform.position += movement;

       
        transform.Rotate(Vector3.up, turn * turnSpeed * Time.deltaTime);
        
        UpdateForwardBlockState();
    }

    private void UpdateForwardBlockState()
    {
        Vector3 origin;
        Vector3 halfExtents;
        Quaternion orientation;

        if (modelCollider != null)
        {
            origin = modelCollider.bounds.center;
            halfExtents = modelCollider.bounds.extents;
            orientation = modelCollider.transform.rotation;
        }
        else
        {
            // fallback: usa il transform del root (puoi aggiustare i valori)
            origin = transform.position;
            halfExtents = new Vector3(1f, 0.5f, 1f);
            orientation = transform.rotation;
        }

        Vector3 castDirection = transform.right;
        float baseDistance = moveSpeed * Time.deltaTime;
        float castDistance = Mathf.Max(0.1f, baseDistance * checkDistanceMultiplier);

        // Se siamo già sovrapposti a un ostacolo (intersezione iniziale) consideriamola bloccante
        RaycastHit[] overlaps = Physics.BoxCastAll(origin, halfExtents, castDirection, orientation, 0f, obstacleMask, QueryTriggerInteraction.Ignore);
        bool overlappingObstacle = false;
        foreach (var o in overlaps)
        {
            if (o.collider != null && !IsWaterLayer(o.collider.gameObject.layer))
            {
                overlappingObstacle = true;
                break;
            }
        }

        if (overlappingObstacle)
        {
            blockedForward = true;
            return;
        }

        // BoxCast per vedere davanti alla nave
        RaycastHit hit;
        bool hasHit = Physics.BoxCast(origin, halfExtents, castDirection, out hit, orientation, castDistance, obstacleMask, QueryTriggerInteraction.Ignore);

        // Se ha colpito qualcosa e non è acqua => blocca
        blockedForward = hasHit && !IsWaterLayer(hit.collider.gameObject.layer);
    }

    private bool IsWaterLayer(int layer)
    {
        int water = LayerMask.NameToLayer("Water");
        return water != -1 && layer == water;
    }

    // Visualizzazione in scena per aiutarti a settare le dimensioni del cast
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, new Vector3(2, 1, 2));
        }

        if (modelCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(modelCollider.bounds.center, modelCollider.bounds.size);

           
            Vector3 origin = modelCollider.bounds.center;
            Vector3 halfExt = modelCollider.bounds.extents;
            Vector3 dir = transform.right;
            float baseDistance = moveSpeed * (Application.isPlaying ? Time.deltaTime : 0.02f);
            float castDistance = baseDistance * checkDistanceMultiplier;
            Gizmos.color = Color.red;
    
            Vector3 boxCenter = origin + dir * (castDistance * 0.5f);
            Gizmos.DrawWireCube(boxCenter, modelCollider.bounds.size + new Vector3(castDistance, 0f, 0f));
        }
    }
}
