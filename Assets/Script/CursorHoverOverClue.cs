using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CursorHoverOverClue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(CheckHover());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance.SetNormalCursor();
        StopAllCoroutines();
    }

    private System.Collections.IEnumerator CheckHover()
    {
        while (true)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, Camera.main);
            if (linkIndex != -1)
                CursorManager.Instance.SetClueCursor();
            else
                CursorManager.Instance.SetNormalCursor();

            yield return null;
        }
    }
}