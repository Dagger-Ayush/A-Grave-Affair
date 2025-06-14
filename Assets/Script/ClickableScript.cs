using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ClickableScript : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        var text = GetComponent<TextMeshProUGUI>();
        if (eventData.button == PointerEventData.InputButton.Left)
        {
           
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);

            if (linkIndex != -1)
            {
                var linkInfo = text.textInfo.linkInfo[linkIndex];
                var clueId = linkInfo.GetLinkID();
                ClueManager.Instance.AddClue(clueId);
            }
        }
    }
}
