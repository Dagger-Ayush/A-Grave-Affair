using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VisionHover : MonoBehaviour
{
    [Header("Hover Settings")]
    [SerializeField] private float radius = 0.2f;
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private Color unHoverColor = Color.blue;

    [Header("Vision Colors")]
    [SerializeField] private Color visionClue = Color.white;
    [SerializeField] private Color visionUnClue = Color.blue;

    [Header("References")]
    [SerializeField] private GameObject vision;
    [SerializeField] private Image visionBorder;
    [SerializeField] private Camera visionCamera;

    [Header("Animation")]
    [SerializeField] private float scale = 0.75f;
    [SerializeField] private float orthoSize = 3f;
    [SerializeField] private float lerpSpeed = 5f;

    private Vector3 scaleStore;
    private float orthoSizeStore;
    private Coroutine currentRoutine;

    private void Start()
    {
        if (vision != null)
            scaleStore = vision.transform.localScale;

        if (visionCamera != null)
            orthoSizeStore = visionCamera.orthographicSize;
    }

    private void Update()
    {
        if (vision == null || !vision.activeSelf) return;

        bool found = false;

        // Raycast from vision object position
        Ray ray = Camera.main.ScreenPointToRay(vision.transform.position);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Collider[] colliders = Physics.OverlapSphere(hit.point, radius);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Object"))
                {
                    HoverEffect();
                    SetColor(collider.gameObject, hoverColor, visionClue);
                    found = true;
                    break;
                }
            }
        }

        if (!found)
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Object"))
            {
                UnHoverEffect();
                SetColor(obj, unHoverColor, visionUnClue);
            }
        }
    }

    private void SetColor(GameObject obj, Color objectColor, Color visionColor)
    {
        if (obj.TryGetComponent(out MeshRenderer renderer))
        {
            renderer.material.color = objectColor;
        }

        if (visionBorder != null)
        {
            visionBorder.color = visionColor;
        }
    }

    public void HoverEffect()
    {
        StartSmoothLerp(new Vector3(scale, scale, scale), orthoSize);
    }

    public void UnHoverEffect()
    {
        StartSmoothLerp(scaleStore, orthoSizeStore);
    }

    private void StartSmoothLerp(Vector3 targetScale, float targetOrthoSize)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(SmoothLerp(targetScale, targetOrthoSize));
    }

    private IEnumerator SmoothLerp(Vector3 targetScale, float targetOrthoSize)
    {
        Vector3 startScale = vision.transform.localScale;
        float startOrthoSize = visionCamera.orthographicSize;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * lerpSpeed;

            vision.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            visionCamera.orthographicSize = Mathf.Lerp(startOrthoSize, targetOrthoSize, t);

            yield return null;
        }
    }
}
