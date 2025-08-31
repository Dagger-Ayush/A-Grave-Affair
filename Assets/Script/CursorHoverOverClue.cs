using System.Collections;
using Mono.Cecil.Cil;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorHoverOverClue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public Color outlineColor = Color.red;
    public float ovalPadding = 15f;
    public float outlineThickness = 3f;
    public float animationDuration = 0.3f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private TextMeshProUGUI textComponent;
    private Camera uiCamera;
    private Coroutine hoverRoutine;
    private Coroutine animationRoutine;
    private bool isHovering = false;
    private int currentLinkIndex = -1;
    private string currentClueId = "";
    private GameObject ovalOutline;
    private Vector2 targetSize;
    private Vector3 targetPosition;

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
        if (ovalOutline != null) return; // Already exists

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

        targetPosition = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0);
        targetSize = new Vector2((maxX - minX) + ovalPadding, (maxY - minY) + ovalPadding);

        ovalOutline = new GameObject("OvalOutline");
        ovalOutline.transform.SetParent(transform, false);
        RectTransform ovalRect = ovalOutline.AddComponent<RectTransform>();

        // Start with zero size for animation
        ovalRect.sizeDelta = Vector2.zero;
        ovalRect.localPosition = targetPosition;

        var ovalImage = ovalOutline.AddComponent<UnityEngine.UI.Image>();
        ovalImage.sprite = CreateOvalSprite((int)targetSize.x, (int)targetSize.y);
        ovalImage.color = outlineColor;
        ovalImage.type = UnityEngine.UI.Image.Type.Simple;
        ovalImage.preserveAspect = true;

        ovalOutline.transform.SetAsFirstSibling();

        // Start the animation
        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(AnimateOutline(ovalRect));
    }

    private IEnumerator AnimateOutline(RectTransform rectTransform)
    {
        float elapsed = 0f;
        Vector2 initialSize = Vector2.zero;
        Color initialColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0);
        Color targetColor = outlineColor;

        Image image = rectTransform.GetComponent<Image>();
        if (image) image.color = initialColor;

        while (elapsed < animationDuration)
        {
            float t = animationCurve.Evaluate(elapsed / animationDuration);

            // Animate size
            rectTransform.sizeDelta = Vector2.Lerp(initialSize, targetSize, t);

            // Animate alpha
            if (image) image.color = Color.Lerp(initialColor, targetColor, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final values
        rectTransform.sizeDelta = targetSize;
        if (image) image.color = targetColor;
    }

    private Sprite CreateOvalSprite(int width, int height)
    {
        int textureWidth = Mathf.Max(64, width);
        int textureHeight = Mathf.Max(64, height);
        Texture2D texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);

        // Fill with transparent
        Color[] transparentPixels = new Color[textureWidth * textureHeight];
        for (int i = 0; i < transparentPixels.Length; i++)
        {
            transparentPixels[i] = Color.clear;
        }
        texture.SetPixels(transparentPixels);

        // Draw oval outline
        Vector2 center = new Vector2(textureWidth / 2f, textureHeight / 2f);
        float radiusX = textureWidth / 2f - outlineThickness;
        float radiusY = textureHeight / 2f - outlineThickness;

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                // Calculate normalized position (-1 to 1)
                float nx = (x - center.x) / radiusX;
                float ny = (y - center.y) / radiusY;

                // Calculate distance from edge (1 = at edge, 0 = at center)
                float distance = Mathf.Sqrt(nx * nx + ny * ny);

                // Draw outline if we're near the edge
                if (distance >= 0.9f && distance <= 1.1f)
                {
                    // Smooth alpha based on distance from perfect edge
                    float alpha = 1f - Mathf.Abs(distance - 1f) * 10f;
                    texture.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(alpha)));
                }
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;

        return Sprite.Create(texture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f));
    }

    private void RemoveOutline()
    {
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }

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
