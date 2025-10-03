using UnityEngine;
using TMPro; // <-- importante per TextMeshProUGUI

public class FishingActivator : MonoBehaviour
{
    [Header("Riferimenti oggetti")]
    public GameObject fishingRodWorld;   // l'oggetto visivo della canna nel mondo (da attivare/disattivare)
    public GameObject fishingRodPlayer;  // la canna che il player "indossa" (disattivata all'inizio)

    [Header("Impostazioni interazione")]
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Prompt (TextMeshPro)")]
    public GameObject uiPrompt;              // pannello UI (GameObject) contenente il testo
    public TextMeshProUGUI promptText;       // il componente TMP da trascinare nell'inspector

    private bool isPlayerNearby = false;
    private bool hasRod = false;

    private void Start()
    {
        // assicurati che la canna del player sia nello stato iniziale voluto
        if (fishingRodPlayer != null)
            fishingRodPlayer.SetActive(hasRod);

        if (uiPrompt != null)
            uiPrompt.SetActive(false); // nasconde il prompt all'inizio
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(interactKey))
        {
            ToggleRod();
            UpdatePromptText();
        }
    }

    private void ToggleRod()
    {
        hasRod = !hasRod;

        if (fishingRodPlayer != null)
            fishingRodPlayer.SetActive(hasRod);

        if (fishingRodWorld != null)
            fishingRodWorld.SetActive(!hasRod);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            UpdatePromptText();
            if (uiPrompt != null) uiPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (uiPrompt != null) uiPrompt.SetActive(false);
        }
    }

    private void UpdatePromptText()
    {
        if (promptText == null) return;
        promptText.text = hasRod ? "Premi [E] per riporre la canna" : "Premi [E] per prendere la canna";
    }
}
