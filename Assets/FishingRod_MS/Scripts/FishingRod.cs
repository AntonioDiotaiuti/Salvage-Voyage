using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingRod : MonoBehaviour
{
    [Header("Zona pesca")]
    [Tooltip("Centro della zona pesca (ad esempio la sfera trigger). Serve solo per calcolare la distanza XZ.")]
    public Transform fishingZone;

    [Tooltip("Altezza dell’acqua (ad esempio un empty all’altezza dell’acqua o BoatRoot).")]
    public Transform waterLevel;

    [Tooltip("Raggio entro cui è possibile pescare attorno alla barca (metri).")]
    public float fishingRadius = 20f;

    [Header("Stato (sola lettura)")]
    public bool isFishingAvailable;
    public bool isCasted;
    public bool isPulling;

    [Header("Riferimenti oggetti")]
    public GameObject baitPrefab;
    public GameObject endof_of_rope;  // fine corda (segue esca o canna)
    public GameObject start_of_rope;  // inizio corda (sulla canna)
    public GameObject start_of_rod;   // punto fisico della canna

    [Header("Debug")]
    public bool showGizmos = true;

    private Animator animator;
    private Transform baitPosition;

    private PlayerInput inputActions;
    private PlayerInput.OnFootActions onFoot;

    private Vector3? lastWaterPoint;

    private bool IsEquipped => isActiveAndEnabled;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputActions = new PlayerInput();
        onFoot = inputActions.OnFoot;
    }

    private void OnEnable()
    {
        onFoot.Enable();
        onFoot.Fishing.performed += OnFishingPerformed;
    }

    private void OnDisable()
    {
        onFoot.Fishing.performed -= OnFishingPerformed;
        onFoot.Disable();
    }

    private void Update()
    {
        if (!IsEquipped || !fishingZone || !waterLevel)
        {
            isFishingAvailable = false;
            lastWaterPoint = null;
            return;
        }

        // Calcola punto acqua davanti alla camera
        if (TryGetWaterPointFromView(out Vector3 waterPoint))
        {
            lastWaterPoint = waterPoint;
            isFishingAvailable = IsInsideRadiusXZ(waterPoint);
        }
        else
        {
            lastWaterPoint = null;
            isFishingAvailable = false;
        }

        // Aggiorna corda
        if (start_of_rope && start_of_rod && endof_of_rope)
        {
            start_of_rope.transform.position = start_of_rod.transform.position;

            if ((isCasted || isPulling) && baitPosition)
            {
                // rope segue esca
                endof_of_rope.transform.position = baitPosition.position;
            }
            else
            {
                // rope riattaccata alla canna
                endof_of_rope.transform.position = start_of_rod.transform.position;
            }
        }
    }

    private void OnFishingPerformed(InputAction.CallbackContext ctx)
    {
        if (!IsEquipped || !isFishingAvailable) return;

        if (!isCasted && !isPulling)
        {
            if (lastWaterPoint.HasValue && IsInsideRadiusXZ(lastWaterPoint.Value))
            {
                StartCoroutine(CastRod(lastWaterPoint.Value));
            }
        }
        else if (isCasted)
        {
            PullRod();
        }
    }

    // Calcolo punto acqua (piano orizzontale all’altezza di waterLevel)
    private bool TryGetWaterPointFromView(out Vector3 point)
    {
        point = default;

        var cam = Camera.main;
        if (!cam) return false;

        float waterY = waterLevel.position.y;
        Plane waterPlane = new Plane(Vector3.up, new Vector3(0f, waterY, 0f));

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));

        if (waterPlane.Raycast(ray, out float enter) && enter > 0f)
        {
            point = ray.GetPoint(enter);
            point.y = waterY;
            return true;
        }

        // fallback: forward XZ
        Vector3 fwdXZ = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
        if (fwdXZ.sqrMagnitude > 1e-4f)
        {
            Vector3 o = cam.transform.position;
            point = new Vector3(o.x, waterY, o.z) + fwdXZ * 10f;
            return true;
        }

        return false;
    }

    private bool IsInsideRadiusXZ(Vector3 p)
    {
        Vector2 a = new Vector2(fishingZone.position.x, fishingZone.position.z);
        Vector2 b = new Vector2(p.x, p.z);
        return Vector2.Distance(a, b) <= fishingRadius;
    }

    private IEnumerator CastRod(Vector3 targetPosition)
    {
        isCasted = true;
        animator?.SetTrigger("Cast");
        animator?.ResetTrigger("Pull");

        yield return new WaitForSeconds(1f);

        // forza Y alla quota dell’acqua
        targetPosition.y = waterLevel.position.y;

        GameObject bait = Instantiate(baitPrefab, targetPosition, Quaternion.identity);
        baitPosition = bait.transform;
    }

    private void PullRod()
    {
        animator?.SetTrigger("Pull");
        animator?.ResetTrigger("Cast");

        if (baitPosition)
        {
            Destroy(baitPosition.gameObject);
            baitPosition = null;
        }

        // Reset rope subito al ritiro
        if (endof_of_rope && start_of_rod)
        {
            endof_of_rope.transform.position = start_of_rod.transform.position;
        }

        isCasted = false;
        isPulling = false;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        if (fishingZone)
        {
            Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
            Gizmos.DrawWireSphere(fishingZone.position, fishingRadius);
        }

        if (lastWaterPoint.HasValue)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(lastWaterPoint.Value, 0.25f);
        }
    }
}
