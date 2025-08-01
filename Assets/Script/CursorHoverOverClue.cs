using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CursorHoverOverClue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;
    private Camera uiCamera;
    private Coroutine hoverRoutine;
    private bool isHovering = false;
    private string currentHoveredClueId = "";

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
            ResetCursorState();
            isHovering = false;
            currentHoveredClueId = "";
        }
    }

    private void ResetCursorState()
    {
        if (TabletManager.isTabletOpen)
            CursorManager.Instance.SetCursor(CursorState.Tablet);
        else
            CursorManager.Instance.SetCursor(CursorState.Normal);
        ObjectHovering.isRunning = false;
    }

    private System.Collections.IEnumerator CheckHover()
    {
        while (isHovering)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, uiCamera);

            // Reset if not hovering over any link
            if (linkIndex == -1)
            {
                if (!string.IsNullOrEmpty(currentHoveredClueId))
                {
                    currentHoveredClueId = "";
                    ResetCursorState();
                }
                yield return null;
                continue;
            }

            // Get current hovered clue info
            var linkInfo = text.textInfo.linkInfo[linkIndex];
            currentHoveredClueId = linkInfo.GetLinkID();
            bool isClueCollected = ClueManager.Instance.ClueCheck(currentHoveredClueId);

            // Handle click only on uncollected clues
            if (Input.GetMouseButtonDown(0))
            {
                if (!isClueCollected)
                {
                    ClueManager.Instance.AddClue(currentHoveredClueId);
                    ResetCursorState();
                    yield return new WaitForEndOfFrame(); //  immediate cursor reset
                    continue;
                }
            }

            // Only show clue cursor for uncollected clues
            if (!isClueCollected)
            {
                if (!ObjectHovering.isRunning)
                {
                    if (TabletManager.isTabletOpen)
                        CursorManager.Instance.SetCursor(CursorState.Tablet); 
                    else
                        CursorManager.Instance.SetCursor(CursorState.Clue);
                    ObjectHovering.isRunning = true;
                }
            }
            else if (ObjectHovering.isRunning)
            {
                ResetCursorState();
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