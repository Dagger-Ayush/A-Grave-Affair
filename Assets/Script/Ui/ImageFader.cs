using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFade : MonoBehaviour
{
    public static ImageFade instance;

    [Header("Assign the UI Image to fade")]
    public Image img;

    [Header("Seconds for each fade phase")]
    public float fadeTime = 1f;

    void Awake()
    {
        instance = this;
    }

    public void FadeInOut()
    {
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        // Enable the image before fading
        img.gameObject.SetActive(true);

        // ---------- Fade IN ----------
        yield return Fade(0f, 1f);

        // ---------- Fade OUT ----------
        yield return Fade(1f, 0f);

        // Disable after fading out
        img.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color c = img.color;

        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeTime);
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            img.color = c;
            yield return null;
        }

        // Ensure final alpha is set exactly
        c.a = endAlpha;
        img.color = c;
    }
}
