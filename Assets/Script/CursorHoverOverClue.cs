using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CursorHoverOverClue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Color Settings")]
    public Color hoverColor = Color.green;
    public Color defaultLinkColor = Color.black; // Default color is black

    private TextMeshProUGUI textComponent;
    private Camera uiCamera;
    private Coroutine hoverRoutine;
    private bool isHovering = false;
    private int currentLinkIndex = -1;
    private string currentClueId = "";

    void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        uiCamera = GetCanvasCamera();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isHovering)
        {
            isHovering = true;
            hoverRoutine = StartCoroutine(HoverCheck());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopHover();
    }

    private System.Collections.IEnumerator HoverCheck()
    {
        while (isHovering)
        {
            int newLinkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, Input.mousePosition, uiCamera);

            // Reset previous link color if hover changed and clue not collected
            if (currentLinkIndex != -1 && currentLinkIndex != newLinkIndex && !ClueManager.Instance.ClueCheck(currentClueId))
            {
                ResetLinkColor(currentLinkIndex);
            }

            currentLinkIndex = newLinkIndex;

            if (currentLinkIndex != -1)
            {
                var linkInfo = textComponent.textInfo.linkInfo[currentLinkIndex];
                currentClueId = linkInfo.GetLinkID();

                if (!ClueManager.Instance.ClueCheck(currentClueId))
                {
                    SetLinkColor(currentLinkIndex, hoverColor);
                    SetCursorState(CursorState.Clue);

                    if (Input.GetMouseButtonDown(0))
                    {
                        ClueManager.Instance.AddClue(currentClueId);
                        SetLinkColor(currentLinkIndex, hoverColor); // Permanently green after clicking
                        FloatingTextSpawner.Instance.SpawnFloatingText(currentClueId, Input.mousePosition);
                    }
                }
                else
                {
                    SetLinkColor(currentLinkIndex, hoverColor); // Already collected, stay green
                    SetCursorState(CursorState.Normal);
                }
            }
            else
            {
                // If mouse leaves all links, reset color for non-collected
                if (currentLinkIndex != -1 && !ClueManager.Instance.ClueCheck(currentClueId))
                {
                    ResetLinkColor(currentLinkIndex);
                }
                SetCursorState(CursorState.Normal);
            }

            yield return null;
        }
    }

    private void StopHover()
    {
        if (hoverRoutine != null)
        {
            StopCoroutine(hoverRoutine);
            hoverRoutine = null;
        }

        if (currentLinkIndex != -1 && !ClueManager.Instance.ClueCheck(currentClueId))
        {
            ResetLinkColor(currentLinkIndex);
        }

        isHovering = false;
        currentLinkIndex = -1;
        currentClueId = "";
        SetCursorState(CursorState.Normal);
    }

    private void SetLinkColor(int linkIndex, Color color)
    {
        if (linkIndex < 0 || linkIndex >= textComponent.textInfo.linkCount) return;

        var linkInfo = textComponent.textInfo.linkInfo[linkIndex];
        for (int i = 0; i < linkInfo.linkTextLength; i++)
        {
            int charIndex = linkInfo.linkTextfirstCharacterIndex + i;
            if (charIndex >= textComponent.textInfo.characterCount) continue;

            var charInfo = textComponent.textInfo.characterInfo[charIndex];
            if (!charInfo.isVisible) continue;

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Color32[] vertexColors = textComponent.textInfo.meshInfo[meshIndex].colors32;
            vertexColors[vertexIndex + 0] = color;
            vertexColors[vertexIndex + 1] = color;
            vertexColors[vertexIndex + 2] = color;
            vertexColors[vertexIndex + 3] = color;
        }
        textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void ResetLinkColor(int linkIndex)
    {
        SetLinkColor(linkIndex, Color.black);
    }

    private void SetCursorState(CursorState state)
    {
        if (TabletManager.isTabletOpen)
            CursorManager.Instance.SetCursor(CursorState.Tablet);
        else
            CursorManager.Instance.SetCursor(state);

        ObjectHovering.isRunning = (state == CursorState.Clue);
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