using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class CursorHoverOverClue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public Color outlineColor = Color.red;
    public float ovalPadding = 15f; 
    public float cornerRadius = 20f; 

    private TextMeshProUGUI textComponent;
    private Camera uiCamera;
    private Coroutine hoverRoutine;
    private bool isHovering = false;
    private int currentLinkIndex = -1;
    private string currentClueId = "";
    private GameObject ovalOutline;

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

    private IEnumerator HoverCheck()
    {
        while (isHovering)
        {
            int newLinkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, Input.mousePosition, uiCamera);

            if (currentLinkIndex != -1 && currentLinkIndex != newLinkIndex)
            {
                RemoveOutline();
            }

            currentLinkIndex = newLinkIndex;

            if (currentLinkIndex != -1)
            {
                var linkInfo = textComponent.textInfo.linkInfo[currentLinkIndex];
                currentClueId = linkInfo.GetLinkID();

                if (!ClueManager.Instance.ClueCheck(currentClueId))
                {
                    CreateOutline(currentLinkIndex);
                    SetCursorState(CursorState.Clue);

                    if (Input.GetMouseButtonDown(0))
                    {
                        ClueManager.Instance.AddClue(currentClueId);
                        FloatingTextSpawner.Instance.SpawnFloatingText(currentClueId, Input.mousePosition);
                    }
                }
                else
                {
                    RemoveOutline();
                    SetCursorState(CursorState.Normal);
                }
            }
            else
            {
                RemoveOutline();
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

        RemoveOutline();
        isHovering = false;
        currentLinkIndex = -1;
        currentClueId = "";
        SetCursorState(CursorState.Normal);
    }

    private void CreateOutline(int linkIndex)
    {
        if (linkIndex < 0 || linkIndex >= textComponent.textInfo.linkCount) return;

        
        RemoveOutline();

        var linkInfo = textComponent.textInfo.linkInfo[linkIndex];
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        for (int i = 0; i < linkInfo.linkTextLength; i++)
        {
            int charIndex = linkInfo.linkTextfirstCharacterIndex + i;
            if (charIndex >= textComponent.textInfo.characterCount) continue;

            var charInfo = textComponent.textInfo.characterInfo[charIndex];
            if (!charInfo.isVisible) continue;

            minX = Mathf.Min(minX, charInfo.bottomLeft.x);
            maxX = Mathf.Max(maxX, charInfo.topRight.x);
            minY = Mathf.Min(minY, charInfo.descender);
            maxY = Mathf.Max(maxY, charInfo.ascender);
        }

        
        Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0);
        float width = (maxX - minX) + ovalPadding;
        float height = (maxY - minY) + ovalPadding;
 
        ovalOutline = new GameObject("OvalOutline");
        ovalOutline.transform.SetParent(transform, false);
        RectTransform ovalRect = ovalOutline.AddComponent<RectTransform>();
        ovalRect.sizeDelta = new Vector2(width, height);
        ovalRect.localPosition = center;
        
        var ovalImage = ovalOutline.AddComponent<UnityEngine.UI.Image>();
        ovalImage.sprite = CreateOvalSprite((int)width, (int)height);
        ovalImage.color = outlineColor;
        ovalImage.type = UnityEngine.UI.Image.Type.Sliced;

       
        ovalOutline.transform.SetAsFirstSibling();
    }

    private Sprite CreateOvalSprite(int width, int height)
    {
       
        int textureWidth = Mathf.Max(64, width);
        int textureHeight = Mathf.Max(64, height);
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        Vector2 center = new Vector2(textureWidth / 2, textureHeight / 2);
        float radiusX = textureWidth / 2 - 2; 
        float radiusY = textureHeight / 2 - 2;

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                // Oval equation: (x-center.x)^2/radiusX^2 + (y-center.y)^2/radiusY^2 <= 1
                float normalizedX = (x - center.x) / radiusX;
                float normalizedY = (y - center.y) / radiusY;
                float distance = normalizedX * normalizedX + normalizedY * normalizedY;

                if (distance > 0.9f && distance <= 1.1f) 
                {
                    texture.SetPixel(x, y, Color.white);
                }
                else
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
    }

    private void RemoveOutline()
    {
        if (ovalOutline != null)
        {
            Destroy(ovalOutline);
            ovalOutline = null;
        }
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

    public bool isHovered()
    {
        return currentLinkIndex != -1;
    }
}