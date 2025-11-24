using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorHoverOverClue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static CursorHoverOverClue instance;

    [Header("Hover Settings")]
    public Color outlineColor = Color.red;
    public float ovalPadding = 9f;
    public int outlineThickness = 3;
    public float animationDuration = 0.5f;
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

    // For permanent outlines
    private List<GameObject> permanentOutlines = new List<GameObject>();

    // Store last hover’s position & size for exact permanent matching
    private Vector2 lastHoverSize;
    private Vector3 lastHoverPosition;
    private string lastHoverClueId;

    void Awake()
    {
        instance = this;
        textComponent = GetComponent<TextMeshProUGUI>();
        uiCamera = GetCanvasCamera();

        Image img = GetComponent<Image>();
        if (img != null) img.raycastTarget = false;
        RawImage rawImg = GetComponent<RawImage>();
        if (rawImg != null) rawImg.raycastTarget = false;
    }

    void OnEnable()
    {
        if (textComponent != null)
            textComponent.ForceMeshUpdate();

        StartCoroutine(DelayedDrawPermanentOutlines());
    }

    void OnDisable()
    {
        ClearPermanentOutlines();
        StopHover();
    }

    private IEnumerator DelayedDrawPermanentOutlines()
    {
        yield return null;
        RefreshPermanentOutlines();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!ObjectInteract.isInteracting ||
            (InteractionTutorial.Instance != null && !InteractionTutorial.Instance.canHover))
            return;

        textComponent.ForceMeshUpdate();

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
        int previousLinkIndex = -1;

        while (isHovering)
        {
            int newLinkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, Input.mousePosition, uiCamera);

            if (newLinkIndex != previousLinkIndex)
            {
                RemoveOutline();

                if (newLinkIndex != -1)
                {
                    var linkInfo = textComponent.textInfo.linkInfo[newLinkIndex];
                    currentClueId = linkInfo.GetLinkID();

                    if (!ClueManager.Instance.ClueCheck(currentClueId))
                    {
                        CreateOutline(newLinkIndex);
                        SetCursorState(CursorState.Clue);
                    }
                    else
                    {
                        SetCursorState(CursorState.Normal);
                    }
                }
                else
                {
                    SetCursorState(CursorState.Normal);
                }

                previousLinkIndex = newLinkIndex;
                currentLinkIndex = newLinkIndex;
            }

            if (newLinkIndex != -1 && Input.GetMouseButtonDown(0))
            {
                var linkInfo = textComponent.textInfo.linkInfo[newLinkIndex];
                currentClueId = linkInfo.GetLinkID();

                if (!ClueManager.Instance.ClueCheck(currentClueId))
                {
                    ClueManager.Instance.AddClue(currentClueId);
                    FloatingTextSpawner.Instance.SpawnFloatingText(currentClueId, Input.mousePosition);

                    // Record this position & size for exact permanent matching
                    lastHoverSize = targetSize;
                    lastHoverPosition = targetPosition;
                    lastHoverClueId = currentClueId;

                    RefreshPermanentOutlines();
                }
            }

            yield return null;
        }
    }

    public void StopHover()
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

    public void ResetHover()
    {
        if (hoverRoutine != null)
        {
            StopCoroutine(hoverRoutine);
            hoverRoutine = null;
        }

        isHovering = false;
        currentLinkIndex = -1;
        currentClueId = "";
        SetCursorState(CursorState.Normal);
    }

    private void CreateOutline(int linkIndex)
    {
        if (linkIndex < 0 || linkIndex >= textComponent.textInfo.linkCount) return;
        if (ovalOutline != null) return;

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
        ovalRect.sizeDelta = targetSize;
        ovalRect.localPosition = targetPosition;
        ovalRect.localRotation = textComponent.rectTransform.localRotation;

        var ovalImage = ovalOutline.AddComponent<RawImage>();
        ovalImage.texture = new Texture2D(1, 1);
        ovalImage.color = outlineColor;
        ovalOutline.transform.SetAsFirstSibling();

        if (animationRoutine != null) StopCoroutine(animationRoutine);
        animationRoutine = StartCoroutine(AnimateDraw(ovalImage));
    }

    private IEnumerator AnimateDraw(RawImage image)
    {
        int texW = Mathf.RoundToInt(targetSize.x * 2);
        int texH = Mathf.RoundToInt(targetSize.y * 2);
        Texture2D texture = new Texture2D(texW, texH, TextureFormat.RGBA32, false);
        image.texture = texture;

        Vector2 center = new Vector2(texW / 2f, texH / 2f);
        float radiusX = texW / 2f - outlineThickness;
        float radiusY = texH / 2f - outlineThickness;

        Color[] clear = new Color[texW * texH];
        for (int i = 0; i < clear.Length; i++) clear[i] = Color.clear;
        texture.SetPixels(clear);

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            float t = animationCurve.Evaluate(elapsed / animationDuration);
            float angle = Mathf.Lerp(0, 360, t);
            DrawArc(texture, center, radiusX, radiusY, angle, outlineColor);
            texture.Apply();
            elapsed += Time.deltaTime;
            yield return null;
        }

        DrawArc(texture, center, radiusX, radiusY, 360, outlineColor);
        texture.Apply();
    }

    private void DrawArc(Texture2D tex, Vector2 center, float rx, float ry, float angleMax, Color col)
    {
        int steps = 360 * 4;
        for (int i = 0; i < steps; i++)
        {
            float angle = (i / (float)steps) * 360f;
            if (angle > angleMax) break;

            float rad = angle * Mathf.Deg2Rad;
            int baseX = Mathf.RoundToInt(center.x + Mathf.Cos(rad) * rx);
            int baseY = Mathf.RoundToInt(center.y + Mathf.Sin(rad) * ry);

            for (int dx = -2; dx <= 2; dx++)
            {
                for (int dy = -2; dy <= 2; dy++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) <= 3)
                    {
                        int x = baseX + dx;
                        int y = baseY + dy;
                        if (x >= 0 && y >= 0 && x < tex.width && y < tex.height)
                            tex.SetPixel(x, y, col);
                    }
                }
            }
        }
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

    // --- Permanent Outline Logic ---

    public void RefreshPermanentOutlines()
    {
        ClearPermanentOutlines();
        textComponent.ForceMeshUpdate();

        for (int i = 0; i < textComponent.textInfo.linkCount; i++)
        {
            var linkInfo = textComponent.textInfo.linkInfo[i];
            string clueId = linkInfo.GetLinkID();

            if (ClueManager.Instance.ClueCheck(clueId))
            {
                CreatePermanentOutline(i, clueId);
            }
        }
    }

    private void CreatePermanentOutline(int linkIndex, string clueId)
    {
        if (linkIndex < 0 || linkIndex >= textComponent.textInfo.linkCount) return;

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

        Vector3 position;
        Vector2 size;

        // ✅ Use last hover’s size/pos if it’s the same clue
        if (clueId == lastHoverClueId)
        {
            size = lastHoverSize;
            position = lastHoverPosition;
        }
        else
        {
            position = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0);
            size = new Vector2((maxX - minX) + ovalPadding, (maxY - minY) + ovalPadding);
        }

        GameObject outlineGO = new GameObject("PermanentOvalOutline");
        outlineGO.transform.SetParent(transform, false);
        RectTransform rect = outlineGO.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.localPosition = position;
        rect.localRotation = textComponent.rectTransform.localRotation;

        RawImage image = outlineGO.AddComponent<RawImage>();

        // 👇 thinner permanent outline — 40% of hover thickness
        float thinOutline = Mathf.Max(1f, outlineThickness * 0.1f);

        image.texture = CreateOvalOutlineTexture(
            (int)(size.x * 1),
            (int)(size.y * 1),
             Mathf.RoundToInt(thinOutline),
            outlineColor
        );

        image.color = Color.white;
        outlineGO.transform.SetAsFirstSibling();

        permanentOutlines.Add(outlineGO);
    }

    private Texture2D CreateOvalOutlineTexture(int width, int height, int thickness, Color outlineColor)
    {
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.clear;
        tex.SetPixels(pixels);

        Vector2 center = new Vector2(width / 2f, height / 2f);
        float radiusX = width / 2f - thickness;
        float radiusY = height / 2f - thickness;
        int steps = 360 * 4;

        for (int i = 0; i < steps; i++)
        {
            float angle = (i / (float)steps) * 360f;
            float rad = angle * Mathf.Deg2Rad;
            int baseX = Mathf.RoundToInt(center.x + Mathf.Cos(rad) * radiusX);
            int baseY = Mathf.RoundToInt(center.y + Mathf.Sin(rad) * radiusY);

            for (int dx = -thickness; dx <= thickness; dx++)
            {
                for (int dy = -thickness; dy <= thickness; dy++)
                {
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) <= thickness * 1.5f)
                    {
                        int x = baseX + dx;
                        int y = baseY + dy;
                        if (x >= 0 && y >= 0 && x < tex.width && y < tex.height)
                            tex.SetPixel(x, y, outlineColor);
                    }
                }
            }
        }

        tex.Apply();
        return tex;
    }

    public void ClearPermanentOutlines()
    {
        if (permanentOutlines == null) return;

        foreach (var outline in permanentOutlines)
        {
            if (outline != null)
                Destroy(outline);
        }
        permanentOutlines.Clear();
    }

    public bool isHovered() => currentLinkIndex != -1;
}
