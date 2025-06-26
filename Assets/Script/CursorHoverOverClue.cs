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

        ObjectHovering.isRunning = false;
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
            if (Input.GetMouseButtonDown(0))
            {

                if (linkIndex != -1)
                {
                    var linkInfo = text.textInfo.linkInfo[linkIndex];
                    var clueId = linkInfo.GetLinkID();
                    ClueManager.Instance.AddClue(clueId);

                }
            }
            if (linkIndex != -1)
            {
                ObjectHovering.isRunning = true;
                CursorManager.Instance.SetClueCursor();

                if (!isClueCursorActive)
                {
                    isClueCursorActive = true;
                    ObjectHovering.isRunning = true;
                    CursorManager.Instance.SetClueCursor();
                }
            }
            else
            {
                ObjectHovering.isRunning = false;
                CursorManager.Instance.SetNormalCursor();
                if (isClueCursorActive)
                {
                    isClueCursorActive = false;
                    ObjectHovering.isRunning = false;
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
