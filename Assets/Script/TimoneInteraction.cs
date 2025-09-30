using UnityEngine;

public class TimoneController : MonoBehaviour
{
    [Header("Riferimenti")]
    public InputManager playerInput; // script sul player
    public GameObject playerModel;   // il modello 3D del player
    public Movement boatMovement;    // script Movement della barca
    public GameObject firstPersonCamera;
    public GameObject thirdPersonCamera;

    [Header("Impostazioni Interazione")]
    public KeyCode interactKey = KeyCode.E;

    private bool isPlayerNearby = false;
    private bool isControllingBoat = false;

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(interactKey))
        {
            if (!isControllingBoat)
                EnterBoatMode();
            else
                ExitBoatMode();
        }
    }

    private void EnterBoatMode()
    {
        isControllingBoat = true;

        // disattiva player
        playerInput.SetActiveControls(false);
        playerModel.SetActive(false);

        // attiva nave
        boatMovement.enabled = true;

        // cambia camera
        firstPersonCamera.SetActive(false);
        thirdPersonCamera.SetActive(true);
    }

    private void ExitBoatMode()
    {
        isControllingBoat = false;

        // riattiva player
        playerInput.SetActiveControls(true);
        playerModel.SetActive(true);

        // disattiva nave
        boatMovement.enabled = false;

        // torna camera player
        thirdPersonCamera.SetActive(false);
        firstPersonCamera.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }
}
