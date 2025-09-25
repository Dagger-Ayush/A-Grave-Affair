//using UnityEngine;
//using UnityEngine.UI;

//public class CharacterButton : MonoBehaviour
//{
//    public CharacterData characterData;
//    private CharacterInfoUI infoUI;

//    void Start()
//    {
//        infoUI = FindAnyObjectByType<CharacterInfoUI>();
//        GetComponent<Button>().onClick.AddListener(OnClick);
//    }

//    void OnClick()
//    {
//        if(infoUI != null && characterData != null)
//            infoUI.DisplayCharacter(characterData);
//    }
//}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CharacterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Character Data")]
    public CharacterData characterData;

    [Header("UI Effects")]
    public Image rippleImage;
    public GameObject roundBackground;

    private CharacterInfoUI infoUI;
    private Coroutine rippleRoutine;

    // Static to ensure only one button is selected at a time
    private static CharacterButton currentlySelected;

    void Start()
    {
        infoUI = FindAnyObjectByType<CharacterInfoUI>();
        GetComponent<Button>().onClick.AddListener(OnClick);

        if (roundBackground != null)
            roundBackground.SetActive(false); // ensure hidden by default

        if(rippleImage != null)
            rippleImage.gameObject.SetActive(false);
    }

    private void OnClick()
    {
        // Update character info panel
        if (infoUI != null && characterData != null)
            infoUI.DisplayCharacter(characterData);

        // Handle round background selection
        if (currentlySelected != null && currentlySelected != this)
        {
            currentlySelected.roundBackground?.SetActive(false);
        }

        currentlySelected = this;
        if (roundBackground != null)
        {
            roundBackground.SetActive(true);
            roundBackground.transform.SetAsFirstSibling();
        }
        if(rippleRoutine != null)
        {
            StopCoroutine(rippleRoutine);
            rippleImage.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (rippleImage != null && rippleRoutine == null)
        {
            rippleRoutine = StartCoroutine(RippleLoop());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (rippleRoutine != null)
        {
            StopCoroutine(rippleRoutine);
            rippleRoutine = null;
        }
        if (rippleImage != null)
            rippleImage.gameObject.SetActive(false);
    }

    private IEnumerator RippleLoop()
    {
        rippleImage.gameObject.SetActive(true);

        while (true)
        {
            // Reset
            rippleImage.transform.localScale = Vector3.zero;
            rippleImage.color = new Color(1, 1, 1, 1);

            float duration = 1f; // ripple speed
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                rippleImage.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.2f, t);
                rippleImage.color = new Color(1, 1, 1, 1 - t);

                yield return null;
            }

            yield return null;
        }
    }
}
