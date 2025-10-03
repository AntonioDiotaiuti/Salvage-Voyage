using UnityEngine;
using TMPro;

public class LadderTeleport : MonoBehaviour
{
    [Header("Destinazioni scale")]
    public Transform bottomPoint;
    public Transform topPoint;

    [Header("Interazione")]
    public KeyCode interactKey = KeyCode.E;
    public string playerTag = "Player";

    [Header("UI Prompt")]
    public TMP_Text interactionText;

    private bool isPlayerNearby = false;
    private Transform currentPlayer;

    void Start()
    {
        if (interactionText == null)
        {
            Debug.LogError($"[LadderTeleport] Nessun TMP_Text assegnato su {name}! Assegna il LadderPrompt nel Canvas.");
            return;
        }

        interactionText.text = "";
        interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isPlayerNearby || currentPlayer == null || interactionText == null) return;

        if (Input.GetKeyDown(interactKey))
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        if (currentPlayer == null) return;

        float dTop = Vector3.Distance(currentPlayer.position, topPoint.position);
        float dBottom = Vector3.Distance(currentPlayer.position, bottomPoint.position);

        Transform target = (dTop < dBottom) ? bottomPoint : topPoint;

        var rb = currentPlayer.GetComponent<Rigidbody>();
        var cc = currentPlayer.GetComponent<CharacterController>();

        if (cc != null) cc.enabled = false;
        if (rb != null) rb.isKinematic = true;

        currentPlayer.position = target.position;
        currentPlayer.rotation = target.rotation;

        if (cc != null) cc.enabled = true;
        if (rb != null) rb.isKinematic = false;

        // 🔹 Nascondi il testo dopo il teletrasporto
        if (interactionText != null)
        {
            interactionText.text = "";
            interactionText.gameObject.SetActive(false);
        }

        isPlayerNearby = false;
        currentPlayer = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        isPlayerNearby = true;
        currentPlayer = other.transform;

        UpdateInteractionText();

        if (interactionText != null)
            interactionText.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        isPlayerNearby = false;
        currentPlayer = null;

        if (interactionText != null)
        {
            interactionText.text = "";
            interactionText.gameObject.SetActive(false);
        }

        Debug.Log($"[LadderTeleport] Uscito dal trigger: {other.name}");
    }

    private void UpdateInteractionText()
    {
        if (interactionText == null || currentPlayer == null) return;

        float dTop = Vector3.Distance(currentPlayer.position, topPoint.position);
        float dBottom = Vector3.Distance(currentPlayer.position, bottomPoint.position);

        interactionText.text = (dTop < dBottom)
            ? $"Premi [{interactKey}] per salire"
            : $"Premi [{interactKey}] per scendere";
    }
}

