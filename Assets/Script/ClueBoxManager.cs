using UnityEngine;
using UnityEngine.UI;

public class ClueBoxManager : MonoBehaviour
{
    public static ClueBoxManager Instance;
    public GameObject clueBox; 
    private GameObject currentButton;
    private int clueBoxOriginalIndex;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        clueBoxOriginalIndex = clueBox.transform.GetSiblingIndex();
    }

    public void ToggleClueBoxBelow(Transform buttonTransform)
    {
        if (clueBox.activeSelf && currentButton == buttonTransform.gameObject)
        {
            clueBox.SetActive(false);
            clueBox.transform.SetSiblingIndex(clueBoxOriginalIndex);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)clueBox.transform.parent);
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

    public void RestoreClueBoxPosition()
    {
        clueBox.transform.SetSiblingIndex(clueBoxOriginalIndex);
    }

}
