using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreenManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Immagine nera per il fade iniziale")]
    public Image fadeImage;

    [Tooltip("Logo del titolo (Salvage Voyage)")]
    public Image titleLogo;

    [Tooltip("Testo 'Press Any Button'")]
    public TMP_Text pressAnyButtonText;

    [Header("Audio")]
    [Tooltip("Musica di sottofondo per la title screen")]
    public AudioSource musicSource;

    [Tooltip("Clip audio dell'OST (es. musica marina, tema principale)")]
    public AudioClip titleMusic;

    [Tooltip("Volume iniziale della musica")]
    [Range(0f, 1f)] public float musicVolume = 0.6f;

    [Tooltip("Tempo di fade-in della musica")]
    public float musicFadeInDuration = 2f;

    [Header("Timings")]
    [Tooltip("Tempo iniziale prima che inizi il fade (barca sott'acqua)")]
    public float startDelay = 1f;

    [Tooltip("Durata del fade in dal nero")]
    public float fadeDuration = 2f;

    [Tooltip("Ritardo prima di mostrare il logo")]
    public float logoDelay = 0.5f;

    [Tooltip("Ritardo prima di mostrare 'Press Any Button' dopo il logo")]
    public float pressButtonDelay = 1.5f;

    [Header("Scena di gioco")]
    [Tooltip("Nome della scena di gameplay da caricare")]
    public string gameplaySceneName = "Gameplay";

    private bool canProceed = false;
    private bool flashing = false;

    void Start()
    {
        // Inizializza alpha elementi UI
        SetAlpha(fadeImage, 1f);
        SetAlpha(titleLogo, 0f);
        SetAlpha(pressAnyButtonText, 0f);

        // Prepara audio
        if (musicSource != null)
        {
            musicSource.clip = titleMusic;
            musicSource.volume = 0f;
            musicSource.loop = true;
        }

        StartCoroutine(TitleSequence());
    }

    IEnumerator TitleSequence()
    {
        // Aspetta mentre la barca “riemerge”
        yield return new WaitForSeconds(startDelay);

        // Fade del nero
        yield return StartCoroutine(FadeImage(fadeImage, 1f, 0f, fadeDuration));

        // Avvia musica dopo il fade
        if (musicSource != null && titleMusic != null)
            StartCoroutine(FadeInMusic());

        // Mostra logo
        yield return new WaitForSeconds(logoDelay);
        yield return StartCoroutine(FadeImage(titleLogo, 0f, 1f, 1f));

        // Mostra “Press Any Button”
        yield return new WaitForSeconds(pressButtonDelay);
        yield return StartCoroutine(FadeText(pressAnyButtonText, 0f, 1f, 1f));

        // Attiva input
        canProceed = true;
        flashing = true;
        StartCoroutine(FlashText(pressAnyButtonText));
    }

    void Update()
    {
        if (canProceed && Input.anyKeyDown)
        {
            canProceed = false;
            flashing = false;
            StartCoroutine(LoadGameplay());
        }
    }

    IEnumerator LoadGameplay()
    {
        // Fade musica e schermo prima del cambio scena
        StartCoroutine(FadeOutMusic(1.5f)); // fade-out morbido della musica
        yield return StartCoroutine(FadeImage(fadeImage, 0f, 1f, 1f));
        SceneManager.LoadScene(gameplaySceneName);
    }

    // AUDIO FADE
    IEnumerator FadeInMusic()
    {
        musicSource.Play();
        float t = 0f;
        while (t < musicFadeInDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t / musicFadeInDuration);
            yield return null;
        }
        musicSource.volume = musicVolume;
    }

    IEnumerator FadeOutMusic(float duration)
    {
        if (musicSource == null) yield break;
        float startVol = musicSource.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / duration);
            yield return null;
        }
        musicSource.Stop();
        musicSource.volume = 0f;
    }

    // UI FADE HELPERS
    void SetAlpha(Graphic g, float alpha)
    {
        if (g == null) return;
        Color c = g.color;
        c.a = alpha;
        g.color = c;
    }

    IEnumerator FadeImage(Image img, float from, float to, float duration)
    {
        if (img == null) yield break;
        float t = 0f;
        Color c = img.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            img.color = c;
            yield return null;
        }
        c.a = to;
        img.color = c;
    }

    IEnumerator FadeText(TMP_Text txt, float from, float to, float duration)
    {
        if (txt == null) yield break;
        float t = 0f;
        Color c = txt.color;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            txt.color = c;
            yield return null;
        }
        c.a = to;
        txt.color = c;
    }

    IEnumerator FlashText(TMP_Text txt)
    {
        if (txt == null) yield break;
        while (flashing)
        {
            yield return StartCoroutine(FadeText(txt, 1f, 0.3f, 0.8f));
            yield return StartCoroutine(FadeText(txt, 0.3f, 1f, 0.8f));
        }
    }
}

