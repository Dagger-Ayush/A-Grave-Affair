using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CursorHoverOverClue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;
    private Camera uiCamera;
    
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        uiCamera = GetCanvasCamera();
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
           
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, uiCamera);
           
            if (linkIndex != -1)
            {
                ObjectHovering.instance.isRunning = true;
                CursorManager.Instance.SetClueCursor();
              
            }
            else
            {
                ObjectHovering.instance.isRunning = false;
                CursorManager.Instance.SetNormalCursor();
            }
            yield return null;
        }
        
    }

    private Camera GetCanvasCamera()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;
        return canvas != null && canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
    }

   
}