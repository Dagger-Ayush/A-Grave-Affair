using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorHoverOverClue : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    public Color outlineColor = Color.red;
    public float ovalPadding = 15f;
    public int outlineThickness = 20;
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
        ovalRect.sizeDelta = targetSize;
        ovalRect.localPosition = targetPosition;

        var ovalImage = ovalOutline.AddComponent<RawImage>();
        ovalImage.texture = new Texture2D(1, 1); // will be replaced in animation
        ovalImage.color = Color.white;

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

        // Fill transparent
        Color[] clear = new Color[texW * texH];
        for (int i = 0; i < clear.Length; i++) clear[i] = Color.clear;
        texture.SetPixels(clear);

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            float t = animationCurve.Evaluate(elapsed / animationDuration);
            float angle = Mathf.Lerp(0, 360, t);

            // redraw arc each frame
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
        int steps = 360 * 4; // resolution
        for (int i = 0; i < steps; i++)
        {
            float angle = (i / (float)steps) * 360f;
            if (angle > angleMax) break;

            float rad = angle * Mathf.Deg2Rad;
            int x = Mathf.RoundToInt(center.x + Mathf.Cos(rad) * rx);
            int y = Mathf.RoundToInt(center.y + Mathf.Sin(rad) * ry);

            if (x >= 0 && y >= 0 && x < tex.width && y < tex.height)
            {
                tex.SetPixel(x, y, col);
                // thickness
                for (int j = 1; j < outlineThickness; j++)
                {
                    if (x + j < tex.width) tex.SetPixel(x + j, y, col);
                    if (x - j >= 0) tex.SetPixel(x - j, y, col);
                    if (y + j < tex.height) tex.SetPixel(x, y + j, col);
                    if (y - j >= 0) tex.SetPixel(x, y - j, col);
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

    void OnDisable()
    {
        StopHover();
    }

    public bool isHovered()
    {
        return currentLinkIndex != -1;
    }
}