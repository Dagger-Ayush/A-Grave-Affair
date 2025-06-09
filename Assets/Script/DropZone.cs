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

        if (fromZone != null)
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

            incoming.sourceDropZone = this; // Update references
        }

        else if (currentClue == null)
        {
            incoming.transform.SetParent(this.transform);
            incoming.transform.localPosition = Vector3.zero;
            currentClue = incoming;
            incoming.sourceDropZone = this;
        }

        else // coming from clue container or outside a drop zone
        {
            if (currentClue != null)
            {
                // Move existing clue back to clue container
                currentClue.transform.SetParent(validator.tabletManager.clueContainer);
                currentClue.transform.localPosition = Vector3.zero;
                currentClue.sourceDropZone = null;
            }

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
 }
