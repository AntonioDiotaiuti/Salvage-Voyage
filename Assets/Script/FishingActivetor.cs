using UnityEngine;
using TMPro; 

public class FishingActivator : MonoBehaviour
{
    [Header("Riferimenti oggetti")]
    public GameObject fishingRodWorld;   
    public GameObject fishingRodPlayer; 

    [Header("Impostazioni interazione")]
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Prompt (TextMeshPro)")]             
    public TextMeshProUGUI promptText; 

    private bool isPlayerNearby = false;
    private bool hasRod = false;

    private void Start()
    {
        // assicurati che la canna del player sia nello stato iniziale voluto
        if (fishingRodPlayer != null)
            fishingRodPlayer.SetActive(hasRod);

        if (promptText != null)
            promptText.enabled = false; 
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
            if (promptText != null) promptText.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (promptText != null) promptText.enabled = false;
        }
    }

    private void UpdatePromptText()
    {
        if (promptText == null) return;
        promptText.text = hasRod ? "Premi [E] per riporre la canna" : "Premi [E] per prendere la canna";
    }
}
