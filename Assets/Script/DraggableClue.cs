using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class DraggableClue : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string clueText;
    [HideInInspector] public DropZone sourceDropZone;
    private Transform originalParent;
    private CanvasGroup canvasGroup;
    private TMP_Text label;
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        
        label = GetComponentInChildren<TMP_Text>();
        if(label != null )
        {
            clueText = label.text;
            Debug.Log($"Clue text assigned from label: {clueText}");
        }
        else
        {
            Debug.LogWarning("No TMP_Text found on clue!");
        }
    }

    public void SetClueText(string text)
    {
        clueText = text;
        if(label != null )
            label.text = text;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (TabletManager.isTabletOpen)
        {
            CursorManager.Instance.SetDraggingClue(true);
            CursorManager.Instance.SetCursor(CursorState.TabletDrag);
        }

        sourceDropZone = GetComponentInParent<DropZone>();
        originalParent = transform.parent;

        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        //if(transform.parent == transform.root)
        //    ReturnToSource();
        
        // Check where the clue was dropped
        GameObject dropTarget = eventData.pointerEnter;

        // If dropped into the clue container
        if (dropTarget != null && dropTarget.transform.IsChildOf(TabletManager.Instance.clueContainer))
        {
            // Clear its old drop zone reference
            if (sourceDropZone != null)
            {
                sourceDropZone.ClearZone();
                sourceDropZone = null;
            }

            // Place it back in clue container
            transform.SetParent(TabletManager.Instance.clueContainer);
            transform.localPosition = Vector3.zero;
        }
        // If dropped nowhere valid, return to source
        else if (transform.parent == transform.root)
        {
            ReturnToSource();
        }
        canvasGroup.blocksRaycasts = true;
        if (TabletManager.isTabletOpen)
        {
            CursorManager.Instance.SetDraggingClue(false);
            CursorManager.Instance.SetCursor(CursorState.Tablet);
        }
    }
    private void ReturnToSource()
    {
        transform.SetParent(sourceDropZone != null ? sourceDropZone.transform : originalParent);
        transform.localPosition = Vector3.zero;
    }
}
