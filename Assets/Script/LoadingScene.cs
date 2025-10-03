using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerSpawnDelay : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Il punto sul ponte dove il player apparirà dopo il delay")]
    public Transform spawnPoint;

    [Tooltip("Tempo di attesa prima di far apparire il player")]
    public float delaySeconds = 2f;

    [Header("Fade UI")]
    [Tooltip("Oggetto UI full-screen (es. Panel nero, Image o TMP con alpha pieno)")]
    public GameObject fadeScreen;

    [Tooltip("Durata del fade-out (in secondi)")]
    public float fadeDuration = 1f;

    [Header("Player Visual")]
    [Tooltip("Root del modello grafico del player (opzionale)")]
    public GameObject modelRoot;

    // Riferimenti
    private CharacterController controller;
    private PlayerMotor motor;
    private PlayerLook look;
    private InputManager inputManager;

    // Supporto per fade
    private CanvasGroup fadeCanvasGroup;
    private Graphic[] fadeGraphics;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        inputManager = GetComponent<InputManager>();

        // Setup fade
        if (fadeScreen != null)
        {
            fadeCanvasGroup = fadeScreen.GetComponent<CanvasGroup>();

            if (fadeCanvasGroup == null)
            {
                // Se non esiste CanvasGroup, prendiamo tutti i Graphic
                fadeGraphics = fadeScreen.GetComponentsInChildren<Graphic>(true);
            }
        }
    }

    void Start()
    {
        // Mostra subito lo schermo nero
        ShowFadeInstant();

        // Disattiva i controlli del player
        if (controller) controller.enabled = false;
        if (motor) motor.enabled = false;
        if (look) look.enabled = false;
        if (inputManager) inputManager.enabled = false;

        // Nascondi modello
        if (modelRoot != null) modelRoot.SetActive(false);

        // Avvia la coroutine di spawn
        StartCoroutine(SpawnAfterDelay());
    }

    IEnumerator SpawnAfterDelay()
    {
        // Attendi il tempo di stabilizzazione della barca
        yield return new WaitForSeconds(delaySeconds);

        // Sposta il player sul punto di spawn
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }
        else
        {
            Debug.LogWarning("[PlayerSpawnDelay] Nessun spawnPoint assegnato!");
        }

        // Riattiva controlli
        if (controller) controller.enabled = true;
        if (motor) motor.enabled = true;
        if (look) look.enabled = true;
        if (inputManager) inputManager.enabled = true;

        // Mostra modello
        if (modelRoot != null) modelRoot.SetActive(true);

        // Effettua fade-out
        yield return StartCoroutine(FadeOut());
    }

    // ?? Mostra subito schermo nero
    void ShowFadeInstant()
    {
        if (fadeScreen == null) return;

        fadeScreen.SetActive(true);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 1f;
        }
        else if (fadeGraphics != null)
        {
            foreach (var g in fadeGraphics)
            {
                if (g != null)
                {
                    Color c = g.color;
                    c.a = 1f;
                    g.color = c;
                }
            }
        }
    }

    // ?? Fade-out graduale
    IEnumerator FadeOut()
    {
        if (fadeScreen == null) yield break;

        float t = 0f;

        if (fadeCanvasGroup != null)
        {
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 0f;
        }
        else if (fadeGraphics != null && fadeGraphics.Length > 0)
        {
            // Usa i Graphic (Image, TMP, ecc.)
            Color[] startColors = new Color[fadeGraphics.Length];
            for (int i = 0; i < fadeGraphics.Length; i++)
                startColors[i] = fadeGraphics[i].color;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);

                for (int i = 0; i < fadeGraphics.Length; i++)
                {
                    if (fadeGraphics[i] == null) continue;
                    Color c = startColors[i];
                    c.a = alpha;
                    fadeGraphics[i].color = c;
                }
                yield return null;
            }
        }

        // Disattiva del tutto il pannello
        fadeScreen.SetActive(false);
    }
}


