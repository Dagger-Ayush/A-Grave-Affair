using UnityEngine;
using UnityEngine.EventSystems;

public class ClueContainerDropZone : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        DraggableClue clue = eventData.pointerDrag?.GetComponent<DraggableClue>();
        if(clue != null )
        {
            clue.transform.SetParent(transform);
            clue.transform.localPosition = Vector3.zero;
            clue.transform.localScale = Vector3.one;
        }
    }
}
