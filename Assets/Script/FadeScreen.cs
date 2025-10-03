using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FadeScreen : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Mostra subito la schermata nera (alpha 1)
    /// </summary>
    public void ShowInstant()
    {
        canvasGroup.alpha = 1f;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Nasconde gradualmente la schermata nera in 'duration' secondi
    /// </summary>
    public IEnumerator FadeOut(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
