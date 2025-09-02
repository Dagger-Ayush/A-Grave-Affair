using UnityEngine;
using UnityEngine.EventSystems;

public class XRaySync : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform uiImage; // the circle image
    public Camera xrayCamera;     // the camera rendering to RenderTexture
    public Canvas canvas;         // your UI canvas

    [HideInInspector] public bool isDragging;
    private Vector2 dragOffset;   // stores offset between mouse and image

    void Update()
    {
        // 1. Convert UI image position to screen point
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, uiImage.position);

        // 2. Convert to world and lock Z
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));

        // 3. Move x-ray camera there
        xrayCamera.transform.position = worldPos;

        // Keep camera facing forward
        xrayCamera.transform.rotation = Camera.main.transform.rotation;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        Vector2 localMouse;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localMouse
        );

        // store offset between mouse and image position
        dragOffset = uiImage.anchoredPosition - localMouse;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 localMouse;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out localMouse
        );

        // apply stored offset
        uiImage.anchoredPosition = localMouse + dragOffset;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}
