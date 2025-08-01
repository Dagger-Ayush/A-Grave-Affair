using UnityEngine;

public enum CursorState
{
    Normal,
    Clue,
    Tablet,
    TabletDrag
}
public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    public Texture2D normalCursor;
    public Texture2D clueCursor;
    public Texture2D tabletCursor;
    public Texture2D tabletDragCursor;
    public Vector2 hotSpot = Vector2.zero;

    private CursorState currentState;
    private bool isDraggingClue = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SetCursor(CursorState.Normal);
    }

    public void SetCursor(CursorState state)
    {
        if (currentState == state) return;

        if(isDraggingClue && state != CursorState.TabletDrag)
            return;
        
        currentState = state;

        switch (state)
        {
            case CursorState.Normal:
                Cursor.SetCursor(normalCursor, hotSpot, CursorMode.Auto);
                break;
            case CursorState.Clue:
                Cursor.SetCursor(clueCursor, hotSpot, CursorMode.Auto);
                break;
            case CursorState.Tablet:
                Cursor.SetCursor(tabletCursor, hotSpot, CursorMode.Auto);
                break;
            case CursorState.TabletDrag:
                Cursor.SetCursor(tabletDragCursor, hotSpot, CursorMode.Auto);
                break;
        }
    }

    public void SetDraggingClue(bool value)
    {
        isDraggingClue = value;
        if(value)
            SetCursor(CursorState.TabletDrag);
        else
            SetCursor(CursorState.Tablet);
    }
    public bool IsDraggingClue() => isDraggingClue;

}