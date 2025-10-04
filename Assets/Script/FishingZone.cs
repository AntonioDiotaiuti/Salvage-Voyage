using UnityEngine;

public class FishingZone : MonoBehaviour
{
    [Header("Zona pesca attorno alla nave")]
    public float fishingRadius = 15f;
    public LayerMask waterMask;

    public static FishingZone Instance;

    private void Awake()
    {
        Instance = this;
    }

    public bool IsInFishingZone(Vector3 point)
    {
        float dist = Vector3.Distance(transform.position, point);

        if (waterMask.value != 0)
        {
            if (Physics.Raycast(point + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f, waterMask))
            {
                return dist <= fishingRadius;
            }
            return false;
        }

        return dist <= fishingRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, fishingRadius);
    }
}

