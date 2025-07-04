using UnityEngine;
using UnityEngine.UI;

public class ClueBoxManager : MonoBehaviour
{
    public GameObject clueBox; 
    private GameObject currentButton;

    public void ToggleClueBoxBelow(Transform buttonTransform)
    {
        if (clueBox.activeSelf && currentButton == buttonTransform.gameObject)
        {
            clueBox.SetActive(false);
            currentButton = null;
            return;
        }

        currentButton = buttonTransform.gameObject;

        RectTransform buttonRect = buttonTransform.GetComponent<RectTransform>();
        RectTransform clueRect = clueBox.GetComponent<RectTransform>();

        clueBox.transform.SetParent(buttonRect.parent, false);
        int buttonIndex = buttonRect.GetSiblingIndex();
        clueBox.transform.SetSiblingIndex(buttonIndex + 1);
        clueBox.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)clueBox.transform.parent);
    }
}
