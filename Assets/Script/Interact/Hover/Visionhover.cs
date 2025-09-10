using UnityEngine;
using UnityEngine.UI;

public class Visionhover : MonoBehaviour
{
    [Header("Hover")]
    public float radius = 0.2f;
    public Color hoverColor = Color.white;
    public Color unHoverColor = Color.blue;
    [Header("Vision")]
    public Color visionClue = Color.white;
    public Color visionUnClue = Color.blue;

    public GameObject vision;
    public Image visionBorder;

    public Camera visionCamera;
    public float scale = 0.75f;
    public float orthoSize = 3f;

    private Vector3 scaleStore;
    private float orthoSizeStore;

    private Coroutine currentRoutine;
    public float lerpSpeed = 5f;
    private void Start()
    {
        scaleStore = vision.transform.localScale;

        orthoSizeStore = visionCamera.orthographicSize;
    }
    void Update()
    {
        bool found = false;

        
        Ray ray = Camera.main.ScreenPointToRay(vision.transform.position);
       
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!vision.activeSelf) return;

            Collider[] colliderArray = Physics.OverlapSphere(hit.point, radius);

           
            foreach (Collider collider in colliderArray)
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
    void SetColor(GameObject obj, Color objectColor,Color visionColor)
    {
        if (obj != null)
        {
            obj.GetComponent<MeshRenderer>().material.color = objectColor;
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

    private System.Collections.IEnumerator SmoothLerp(Vector3 targetScale, float targetOrthoSize)
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
