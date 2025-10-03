using UnityEngine;
using System.Collections;

public class BoatAndPlayerStartup : MonoBehaviour
{
    [Header("Boat")]
    public GameObject boatModel;        // il modello 3D con BoatFollower
    public GameObject boatRoot;         // l’empty con AlignToWater
    public float boatActivationDelay = 2f;

    [Header("Player")]
    public GameObject player;
    public Transform spawnPoint;
    public float playerActivationDelay = 1f; // dopo che la barca è apparsa

    [Header("Fade UI")]
    public GameObject fadeScreen;
    public float fadeDuration = 1f;

    private void Start()
    {
        if (boatModel != null) boatModel.SetActive(false);
        if (player != null) player.SetActive(false);

        if (fadeScreen != null) fadeScreen.SetActive(true);

        StartCoroutine(StartupSequence());
    }

    private IEnumerator StartupSequence()
    {
        // 1) aspetta che il BoatRoot si stabilizzi
        yield return new WaitForSeconds(boatActivationDelay);

        // 2) attiva la barca
        if (boatModel != null) boatModel.SetActive(true);

        // 3) aspetta un po’ prima di spawnare il player
        yield return new WaitForSeconds(playerActivationDelay);

        if (player != null)
        {
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
            player.SetActive(true);
        }

        // 4) fade-out
        if (fadeScreen != null)
        {
            CanvasGroup cg = fadeScreen.GetComponent<CanvasGroup>();
            if (cg == null) cg = fadeScreen.AddComponent<CanvasGroup>();

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }
            fadeScreen.SetActive(false);
        }
    }
}
