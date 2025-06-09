using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class ObjectPickHandler : MonoBehaviour
{
    public bool isMovable;
    private Vector3 offset;
    private Plane dragPlane;

    [Header("Object Handling references")]
    [HideInInspector] public bool isPicked;

    private Vector3 objectTransform;
    private Quaternion objectRotation;
    private Vector2 turn;

    [SerializeField] private Quaternion PickUpRotation;

    public GameObject objectContainer;
    public GameObject inspectionBackroundimage;
    public Camera inspectionCamara;
    public CinemachineCamera MainCam, FocusCam;
    public PointAndMovement pointAndMovement;
    public float rotationSensitivity;

    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private CanvasGroup objectCanvasGroup;
    [SerializeField] private CanvasGroup fadeImage;

    private bool isbusy = false;
    private float time;

    [Header("X-ray references")]
    private bool isVision = false;
    [SerializeField] private GameObject XrayCamara;
    [SerializeField] private GameObject XrayObject;
    [SerializeField] private KeyCode XrayToggle;

    private void Update()
    {
        ObjectHandler();
    }

    private void ObjectHandler()
    { 
        if (playerInteract.GetObjectPickHandler() == this)
        {  
          
            objectCanvasGroup.alpha = isPicked ? 0 : 1;
            if (isbusy) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isPicked)
                    StartCoroutine(ObjectPickUp());
                else
                    StartCoroutine(ObjectDrop());
            }
        }
        else if (playerInteract.GetObjectPickHandler() == null)
        {
            Avoid();
        }

        if (isPicked && Input.GetKeyDown(XrayToggle))
        {
            if (!isVision)
                XrayVisionEnable();
            else
                XrayVisionDisable();
        }
    }

    public IEnumerator ObjectPickUp()
    {
        pointAndMovement.agent.SetDestination(pointAndMovement.player.transform.position);
        pointAndMovement.player.transform.LookAt(transform.position);
        SwitchCam();
        isbusy = true;
        time = 0;
       
        objectTransform = transform.position;
        objectRotation = transform.rotation;
        transform.parent = objectContainer.transform;

        while (time < 1f)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, time);
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        transform.rotation = PickUpRotation;

        isPicked = true;

        yield return new WaitForSeconds(0.45f);
        inspectionBackroundimage.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        isbusy = false;
    }

    public IEnumerator ObjectDrop()
    {
        XrayVisionDisable();
        inspectionBackroundimage.SetActive(false);

        transform.rotation = objectRotation;
        SwitchCam();
        isbusy = true;
        time = 1;

        transform.parent = null;

        while (time > 0f)
        {
            time -= Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, objectTransform, time);
            yield return null;
        }

        transform.localPosition = objectTransform;

        isPicked = false;
        yield return new WaitForSeconds(1);
        isbusy = false;
    }

    private void SwitchCam()
    {
        if (FocusCam.Priority == 0)
        {
            MainCam.Priority = 0;
            FocusCam.Priority = 1;
            MainCam.Lens.ModeOverride = LensSettings.OverrideModes.Perspective;
            FocusCam.Lens.ModeOverride = LensSettings.OverrideModes.Perspective;
        }
        else
        {
            MainCam.Priority = 1;
            FocusCam.Priority = 0;
            MainCam.Lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
            FocusCam.Lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
        }
    }

    private void XrayVisionEnable()
    {
        isVision = true;
        if (XrayObject != null) XrayObject.SetActive(true);
        XrayCamara.SetActive(true);
    }

    private void XrayVisionDisable()
    {
        isVision = false;
        if (XrayObject != null) XrayObject.SetActive(false);
        XrayCamara.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!isPicked) return;

        // Create drag plane perpendicular to camera
        dragPlane = new Plane(-inspectionCamara.transform.forward, transform.position);

        Ray ray = inspectionCamara.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            offset = transform.position - ray.GetPoint(enter);
        }
    }

    private void OnMouseDrag()
    {
        if (!isPicked) return;

        if (isMovable)
        {
            Ray ray = inspectionCamara.ScreenPointToRay(Input.mousePosition);
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                transform.position = hitPoint + offset;
            }
            Vector3 screenPos = inspectionCamara.WorldToScreenPoint(transform.position);
            screenPos.x = Mathf.Clamp(screenPos.x, 0, Screen.width);
            screenPos.y = Mathf.Clamp(screenPos.y, 0, Screen.height);
            transform.position = inspectionCamara.ScreenToWorldPoint(screenPos);

        }
        else
        {
            turn.x = Input.GetAxis("Mouse X") * rotationSensitivity;
            turn.y = Input.GetAxis("Mouse Y") * rotationSensitivity;

            Vector3 right = Vector3.Cross(inspectionCamara.transform.up, transform.position - inspectionCamara.transform.position);
            Vector3 up = Vector3.Cross(transform.position - inspectionCamara.transform.position, right);

            transform.rotation = Quaternion.AngleAxis(-turn.x, up) * transform.rotation;
            transform.rotation = Quaternion.AngleAxis(turn.y, right) * transform.rotation;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = inspectionCamara.WorldToScreenPoint(transform.position).z;
        return inspectionCamara.ScreenToWorldPoint(mousePoint);
    }

    private void Avoid()
    {
        isPicked = false;
        objectCanvasGroup.alpha = 0;
    }
}
