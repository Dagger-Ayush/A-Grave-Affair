using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

public class ObjectPickHandler : MonoBehaviour
{
  
    private float time;

    private Vector3 offset;
    private Plane dragPlane;

    private Vector3 objectTransform;
    private Quaternion objectRotation;
    private Vector2 turn;

    [SerializeField] private Quaternion PickUpRotation ;

    [SerializeField] private ObjectPickReferences pickReferences;
    private Camera inspectionCamara;
    private KeyCode XrayToggle;
   
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private CanvasGroup objectCanvasGroup;

    public float rotationSensitivity = 4f;

    private bool isVision = false;
    [SerializeField] private GameObject XrayObject;

    [HideInInspector] public bool isMouseLocked;
    public bool isMovable;
    [HideInInspector] public bool isPicked;

    private bool isbusy = false;

    [SerializeField]private float inspectionCamFov = 40;

    [SerializeField]private bool isLighter = false;
    private ObjectInteract objectInteract;
    [SerializeField] private string clue;
    private bool isClue;
    private void Start()
    {
        if (isLighter)
        {
            objectInteract = GetComponent<ObjectInteract>();
            objectInteract.enabled = false;
        }
        inspectionCamara = pickReferences.inspectionCamara;
        XrayToggle = pickReferences.XrayToggle;
    }
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

        if (isPicked && Input.GetKeyDown(pickReferences.XrayToggle))
        {
            if (!isVision)
                XrayVisionEnable();
            else
                XrayVisionDisable();
        }
       
    }

    public IEnumerator ObjectPickUp()
    {
        
        playerInteract.player.transform.LookAt(transform.position);

        isbusy = true;
        time = 0;
        isPicked = true;
        pickReferences.SwitchCam();

        objectTransform = transform.position;
        objectRotation = transform.rotation;
        transform.parent = pickReferences.objectContainer.transform;

        while (time < 1f)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, time);
            yield return null;
        }

        transform.localPosition = Vector3.zero;
        transform.rotation = PickUpRotation;
        pickReferences.FocusCam.Lens.FieldOfView = inspectionCamFov;
  

        yield return new WaitForSeconds(0.45f);
        pickReferences. inspectionBackroundimage.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        isbusy = false;
    }

    public IEnumerator ObjectDrop()
    {
        XrayVisionDisable();
        pickReferences.inspectionBackroundimage.SetActive(false);

        transform.rotation = objectRotation;
        pickReferences.SwitchCam();
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

        if (isLighter)
        {
            if (ClueManager.Instance.ClueCheck(clue))
            {
                enabled = false;
                objectInteract.enabled = true;
            }
        }
        yield return new WaitForSeconds(1);
        isbusy = false;
    }

   

    private void XrayVisionEnable()
    {
        isVision = true;
        if (XrayObject != null) XrayObject.SetActive(true);
        pickReferences.XrayCamara.SetActive(true);
    }

    private void XrayVisionDisable()
    {
        isVision = false;
        if (XrayObject != null) XrayObject.SetActive(false);
        pickReferences. XrayCamara.SetActive(false);
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
        if (isMouseLocked) return;

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
