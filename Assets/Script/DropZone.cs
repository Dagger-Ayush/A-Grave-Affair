using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler
{
    public string correctAnswer;
    public DraggableClue currentClue;
    public PuzzleValidator validator;

    public void OnDrop(PointerEventData eventData)
    {
        DraggableClue incoming = eventData.pointerDrag?.GetComponent<DraggableClue>();
        if (incoming == null || incoming == currentClue)
            return;

        DropZone fromZone = incoming.sourceDropZone;

        if (fromZone != null && fromZone != this)
        {
            DraggableClue temp = currentClue; // Swap current clue and incoming clue

            incoming.transform.SetParent(this.transform); // Move incoming to this zone
            incoming.transform.localPosition = Vector3.zero;
            currentClue = incoming;
            incoming.sourceDropZone = this;

            if (temp != null) // Move existing clue to source zone
            {
                temp.transform.SetParent(fromZone.transform);
                temp.transform.localPosition = Vector3.zero;
                fromZone.currentClue = temp;
                temp.sourceDropZone = fromZone;
            }
            else
            {
                fromZone.currentClue = null;
            }
                //incoming.sourceDropZone = this; // Update references
        }

        else if (fromZone == null && currentClue == null)
        {
            incoming.transform.SetParent(this.transform);
            incoming.transform.localPosition = Vector3.zero;
            currentClue = incoming;
            incoming.sourceDropZone = this;
        }

        else if (fromZone == null && currentClue != null)// coming from clue container or outside a drop zone
        {
            // Prevent accidental null-fromZone caused by drag timing
            if (incoming.transform.parent.GetComponent<DropZone>() != null)
            {
                // Treat this as a swap instead
                fromZone = incoming.transform.parent.GetComponent<DropZone>();
                OnDrop(eventData); // Retry cleanly as a swap
                return;
            }
            
            currentClue.transform.SetParent(validator.tabletManager.clueContainer);
            currentClue.transform.localPosition = Vector3.zero;
            currentClue.sourceDropZone = null;

            incoming.transform.SetParent(this.transform);
            incoming.transform.localPosition = Vector3.zero;
            currentClue = incoming;
            incoming.sourceDropZone = this;
        }

        validator.CheckAllDropZonesFilled();
    }
    public bool IsCorrect()
    {
        return currentClue != null && currentClue.clueText == correctAnswer;
    }
    public void ClearZone()
    {
        currentClue = null;
    }
}
