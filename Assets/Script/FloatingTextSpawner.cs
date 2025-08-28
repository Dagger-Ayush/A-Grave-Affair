using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance;
    public GameObject floatingTextPrefab;
    public Canvas uiCanvas;
    public RectTransform bottomLeftTarget;
    public Image tabletImage;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        else Destroy(gameObject);
    }

    public void SpawnFloatingText(string message, Vector2 screenPosition)
    {
        if (tabletImage != null)
            tabletImage.gameObject.SetActive(true);

        GameObject ft = Instantiate(floatingTextPrefab, uiCanvas.transform);
        ft.transform.position = screenPosition;

        TMP_Text text = ft.GetComponentInChildren<TMP_Text>();
        if (text != null) text.text = message;

        MMF_Player feedback = ft.GetComponent<MMF_Player>();
        //if (feedback != null) feedback.PlayFeedbacks();

        if (feedback != null)
        {
            foreach (var fb in feedback.FeedbacksList)
            {
                if (fb is MMF_Position moveToTarget)
                {
                    Vector2 targetAnchoredPos = bottomLeftTarget.anchoredPosition;
                    moveToTarget.DestinationPosition = targetAnchoredPos;
                }

            }
            feedback.PlayFeedbacks();
        }
        Destroy(ft, 1.2f);
        StartCoroutine(DeactivateIconAfterDelay(1.5f));
    }

    private System.Collections.IEnumerator DeactivateIconAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (tabletImage != null)
            tabletImage.gameObject.SetActive(false);
    }
}
