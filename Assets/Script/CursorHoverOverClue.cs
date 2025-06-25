using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CursorHoverOverClue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;
    private Camera uiCamera;
    private Coroutine hoverRoutine;
    private bool isClueCursorActive = false;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        uiCamera = GetCanvasCamera();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverRoutine == null)
            hoverRoutine = StartCoroutine(CheckHover());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopHover();
    }

    private void StopHover()
    {
        if (hoverRoutine != null)
        {
            StopCoroutine(hoverRoutine);
            hoverRoutine = null;
        }

        if (isClueCursorActive)
        {
            CursorManager.Instance.SetNormalCursor();
            isClueCursorActive = false;
        }

        ObjectHovering.instance.isRunning = false;
    }

    public void ResetClueCursor()
    {
        StopHover();
    }

    private System.Collections.IEnumerator CheckHover()
    {
        while (true)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, uiCamera);

            if (linkIndex != -1)
            {
                if (!isClueCursorActive)
                {
                    isClueCursorActive = true;
                    ObjectHovering.instance.isRunning = true;
                    CursorManager.Instance.SetClueCursor();
                }
            }
            else
            {
                if (isClueCursorActive)
                {
                    isClueCursorActive = false;
                    ObjectHovering.instance.isRunning = false;
                    CursorManager.Instance.SetNormalCursor();
                }
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
