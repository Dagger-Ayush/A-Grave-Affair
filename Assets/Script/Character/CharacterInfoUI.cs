using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text nameText;
    public TMP_Text traitsText;
    public Image portraitImage;

    [Header("Animation Settings")]
    public float fadeDuration = 0.8f;
    public float slideOffset = -100f;

    private Vector3 nameStartPos;
    private Vector3 traitsStartPos;
    private Vector3 portraitStartPos;

    void Awake()
    {
        if(nameText != null) nameStartPos = nameText.rectTransform.anchoredPosition;
        if(traitsText != null) traitsStartPos = traitsText.rectTransform.anchoredPosition;
        if(portraitImage != null) portraitStartPos = portraitImage.rectTransform.anchoredPosition;

        Clear();    
    }
    public void DisplayCharacter(CharacterData data)
    {
        if(data == null) return;

        nameText.text = data.characterName;
        traitsText.text = data.traits;
        
        if(data.portrait != null)
        {
            portraitImage.sprite = data.portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        SetAlpha(0f);
        OffsetPositions(slideOffset);

        StopAllCoroutines();
        StartCoroutine(FadeInRoutine());
    }

    public void Clear()
    {
        nameText.text = "";
        traitsText.text = "";
        portraitImage.sprite = null;
        portraitImage.gameObject.SetActive(false);
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        if(nameText != null) nameText.alpha = alpha;
        if(traitsText != null) traitsText.alpha = alpha;
        if(portraitImage != null)
        {
            Color c = portraitImage.color;
            c.a = alpha;
            portraitImage.color = c;
        }
    }

    private void OffsetPositions(float offset)
    {
        if (nameText != null) nameText.rectTransform.anchoredPosition = nameStartPos + new Vector3(offset, 0, 0);
        if (traitsText != null) traitsText.rectTransform.anchoredPosition = traitsStartPos + new Vector3(offset, 0, 0);
        if(portraitImage != null) portraitImage.rectTransform.anchoredPosition = portraitStartPos + new Vector3(offset, 0, 0);
    }

    private IEnumerator FadeInRoutine()
    {
        float t = 0f;
        while(t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t/fadeDuration);
            SetAlpha(progress);

            if (nameText != null) nameText.rectTransform.anchoredPosition = Vector3.Lerp(nameStartPos + new Vector3(slideOffset, 0, 0), nameStartPos, progress);
            if (traitsText != null) traitsText.rectTransform.anchoredPosition = Vector3.Lerp(traitsStartPos + new Vector3(slideOffset, 0, 0), traitsStartPos, progress);
            if (portraitImage != null) portraitImage.rectTransform.anchoredPosition = Vector3.Lerp(portraitStartPos + new Vector3(slideOffset, 0, 0), portraitStartPos, progress);
            
            yield return null;
        }
        SetAlpha(1f);
        if(nameText != null) nameText.rectTransform.anchoredPosition = nameStartPos;
        if(traitsText != null) traitsText.rectTransform.anchoredPosition = traitsStartPos;
        if(portraitImage != null) portraitImage.rectTransform.anchoredPosition = portraitStartPos;
    }
}
