using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    public Texture2D normalCursor;
    public Texture2D clueCursor;
    public Vector2 hotSpot = Vector2.zero;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        SetNormalCursor();
    }

  
    public void SetNormalCursor()
    {
            Cursor.SetCursor(normalCursor, hotSpot, CursorMode.Auto);
       
    }

    public void SetClueCursor()
    {
       
            Cursor.SetCursor(clueCursor, hotSpot, CursorMode.Auto);
        
    }
}