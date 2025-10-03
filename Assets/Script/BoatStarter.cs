using UnityEngine;
using System.Collections;
using StylizedWater3; // serve per accedere a AlignToWater

public class BoatInitializer : MonoBehaviour
{
    [Header("Riferimenti")]
    public AlignToWater alignScript;   // script che gestisce il galleggiamento
    public Transform spawnPoint;       // punto dove spawna il player sopra la barca
    public GameObject player;          // prefab o istanza del player

    [Header("Tempi")]
    public float alignDelay = 0.2f;    // quanto aspettare prima di attivare AlignToWater
    public float playerSpawnDelay = 1f;// quanto aspettare prima di spawnare il player

    private bool initialized = false;

    private void Awake()
    {
        // disattiva AlignToWater all’inizio per evitare che muova la barca
        if (alignScript != null)
            alignScript.enabled = false;

        // disattiva temporaneamente il player
        if (player != null)
            player.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(InitializeBoat());
    }

    private IEnumerator InitializeBoat()
    {
        // ?? Allinea manualmente la barca all’acqua una volta subito
        if (alignScript != null)
        {
            // calcola altezza immediata
            alignScript.FixedUpdate(); // forza un aggiornamento
        }

        // ?? Aspetta un attimo per far stabilizzare tutto
        yield return new WaitForSeconds(alignDelay);

        // ?? Attiva il galleggiamento
        if (alignScript != null)
            alignScript.enabled = true;

        // ?? Aspetta ancora un po' prima di spawnare il player
        yield return new WaitForSeconds(playerSpawnDelay);

        // ?? Spawna il player nel punto corretto sopra la barca
        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
            player.SetActive(true);

            // assicurati che la gravità sia attiva
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
                rb.useGravity = true;
        }

        initialized = true;
    }
}

