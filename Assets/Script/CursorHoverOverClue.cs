using System.Collections;
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

    void Awake()
    {
        instance = this;
        textComponent = GetComponent<TextMeshProUGUI>();
        uiCamera = GetCanvasCamera();

        // Disable raycast blocking from background image if present
        Image img = GetComponent<Image>();
        if (img != null) img.raycastTarget = false;
        RawImage rawImg = GetComponent<RawImage>();
        if (rawImg != null) rawImg.raycastTarget = false;
    }

    void OnEnable()
    {
        // force mesh update to avoid link geometry delay
        if (textComponent != null)
            textComponent.ForceMeshUpdate();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!ObjectInteract.isInteracting ||
            (InteractionTutorial.Instance != null && !InteractionTutorial.Instance.canHover))
            return;

        textComponent.ForceMeshUpdate(); // ✅ ensures link bounds are updated right away

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
            int newLinkIndex = FindRotatedLinkUnderMouse(textComponent, Input.mousePosition, uiCamera);

            if (currentLinkIndex != -1 && currentLinkIndex != newLinkIndex)
                RemoveOutline();

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

    void OnDisable()
    {
        // 🔧 Don’t clear hover if panel is just being toggled — only if destroyed
        if (!gameObject.activeInHierarchy)
        {
            StopHover();
        }
    }

    public bool isHovered() => currentLinkIndex != -1;

    private int FindRotatedLinkUnderMouse(TextMeshProUGUI tmp, Vector2 mousePos, Camera cam)
    {
        tmp.ForceMeshUpdate();
        RectTransform rect = tmp.rectTransform;
        for (int i = 0; i < tmp.textInfo.linkCount; i++)
        {
            var linkInfo = tmp.textInfo.linkInfo[i];
            bool inside = false;

            for (int c = 0; c < linkInfo.linkTextLength; c++)
            {
                int charIndex = linkInfo.linkTextfirstCharacterIndex + c;
                if (charIndex >= tmp.textInfo.characterCount) continue;
                var charInfo = tmp.textInfo.characterInfo[charIndex];
                if (!charInfo.isVisible) continue;

                Vector3 bl = rect.TransformPoint(charInfo.bottomLeft);
                Vector3 tl = rect.TransformPoint(new Vector3(charInfo.bottomLeft.x, charInfo.topRight.y, 0));
                Vector3 tr = rect.TransformPoint(charInfo.topRight);
                Vector3 br = rect.TransformPoint(new Vector3(charInfo.topRight.x, charInfo.bottomLeft.y, 0));

                RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, mousePos, cam, out Vector3 worldMouse);

                if (PointInQuad(worldMouse, bl, tl, tr, br))
                {
                    inside = true;
                    break;
                }
            }

            if (inside) return i;
        }
        return -1;
    }

    private bool PointInQuad(Vector3 p, Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br)
    {
        return PointInTriangle(p, bl, tl, tr) || PointInTriangle(p, bl, tr, br);
    }

    private bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 v0 = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = p - a;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        return (u >= 0) && (v >= 0) && (u + v < 1);
    }
}
