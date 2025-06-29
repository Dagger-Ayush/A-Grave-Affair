using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CursorHoverOverClue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;
    private Camera uiCamera;
    private Coroutine hoverRoutine;
    private bool isHovering = false;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        uiCamera = GetCanvasCamera();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovering && hoverRoutine == null)
        {
            isHovering = true;
            hoverRoutine = StartCoroutine(CheckHover());
        }
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

        if (isHovering)
        {
            CursorManager.Instance.SetNormalCursor();
            isHovering = false;
            ObjectHovering.isRunning = false;
        }
    }

    private System.Collections.IEnumerator CheckHover()
    {
        while (isHovering)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, uiCamera);


            if (Input.GetMouseButtonDown(0) && linkIndex != -1)
            {
                var linkInfo = text.textInfo.linkInfo[linkIndex];
                ClueManager.Instance.AddClue(linkInfo.GetLinkID());
            }


            bool shouldShowClueCursor = linkIndex != -1;

            if (shouldShowClueCursor)
            {
                if (!ObjectHovering.isRunning)
                {
                    CursorManager.Instance.SetClueCursor();
                    ObjectHovering.isRunning = true;
                }
            }
            else
            {
                if (ObjectHovering.isRunning)
                {
                    CursorManager.Instance.SetNormalCursor();
                    ObjectHovering.isRunning = false;
                }
            }

            yield return null;
        }


        StopHover();
    }

    private Camera GetCanvasCamera()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return null;
        return canvas?.worldCamera ?? Camera.main;
    }

    void OnDisable()
    {
        StopHover();
    }
}


