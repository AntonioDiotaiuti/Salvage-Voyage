using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundMusicManager : MonoBehaviour
{
    [Header("Lista delle tracce musicali")]
    [Tooltip("Trascina qui tutti gli AudioClip che vuoi vengano riprodotti in modo casuale")]
    public List<AudioClip> musicTracks = new List<AudioClip>();

    [Header("Impostazioni audio")]
    [Tooltip("AudioSource usato per la riproduzione della musica")]
    public AudioSource musicSource;

    [Range(0f, 1f)]
    [Tooltip("Volume della musica")]
    public float musicVolume = 0.8f;

    [Tooltip("Tempo di fade-in/out tra le tracce (in secondi)")]
    public float crossfadeDuration = 2f;

    private AudioClip currentTrack;
    private bool isFading = false;
    private bool keepPlaying = true;

    void Start()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }

        musicSource.loop = false;
        musicSource.volume = 0f;

        if (musicTracks.Count > 0)
        {
            StartCoroutine(PlayMusicLoop());
        }
        else
        {
            Debug.LogWarning("[BackgroundMusicManager] Nessuna traccia assegnata!");
        }
    }

    IEnumerator PlayMusicLoop()
    {
        yield return new WaitForSeconds(1f); // piccolo ritardo iniziale

        while (keepPlaying)
        {
            AudioClip nextTrack = GetRandomTrack();

            if (nextTrack != null)
            {
                currentTrack = nextTrack;
                yield return StartCoroutine(FadeInTrack(nextTrack));

                // Attendi la fine della traccia
                yield return new WaitForSeconds(currentTrack.length - crossfadeDuration);

                // Fade out prima della prossima
                yield return StartCoroutine(FadeOutCurrent());
            }
            else
            {
                yield return null;
            }
        }
    }

    AudioClip GetRandomTrack()
    {
        if (musicTracks.Count == 0) return null;

        AudioClip newClip;
        do
        {
            newClip = musicTracks[Random.Range(0, musicTracks.Count)];
        } while (newClip == currentTrack && musicTracks.Count > 1);

        return newClip;
    }

    IEnumerator FadeInTrack(AudioClip clip)
    {
        isFading = true;
        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();

        float t = 0f;
        while (t < crossfadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t / crossfadeDuration);
            yield return null;
        }

        musicSource.volume = musicVolume;
        isFading = false;
    }

    IEnumerator FadeOutCurrent()
    {
        isFading = true;

        float startVol = musicSource.volume;
        float t = 0f;

        while (t < crossfadeDuration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVol, 0f, t / crossfadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = 0f;
        isFading = false;
    }

    // Funzioni pubbliche opzionali
    public void StopMusic()
    {
        keepPlaying = false;
        StopAllCoroutines();
        if (musicSource.isPlaying)
            musicSource.Stop();
    }

    public void ResumeMusic()
    {
        if (!keepPlaying)
        {
            keepPlaying = true;
            StartCoroutine(PlayMusicLoop());
        }
    }
}

