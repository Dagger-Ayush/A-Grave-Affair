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

     public static bool isMouseLocked;
    public bool isMovable;
    [HideInInspector] public bool isPicked;
    [HideInInspector] public static bool isCollected;
    private bool isbusy = false;

    [SerializeField]private float inspectionCamFov = 40;

    [SerializeField]private bool isLighter = false;
    [SerializeField]private bool isLetter = false;

    [SerializeField]private bool checkClue = false;


    private ObjectInteract objectInteract;
    [SerializeField] private string[] clue;

   [HideInInspector] public bool shouldWork = false;
    private void Start()
    { 
        if (checkClue)
        {
            objectInteract = GetComponent<ObjectInteract>();
            objectInteract.enabled = false;
            if (isLighter)
            {
                enabled = false;
            }
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
        if (!shouldWork) return;
        if (objectCanvasGroup != null)
        {
            if (isCollected || ObjectInteract.isInteracted)
            {
                objectCanvasGroup.alpha = 0;
            }
        }
        if (isPicked && Input.GetKeyDown(KeyCode.E))
        {
            if (isbusy) return;
            StartCoroutine(ObjectDrop());
        }
        if (playerInteract.GetObjectPickHandler() == this)
        {
            if (objectCanvasGroup != null)
            {
                if (!isCollected && !ObjectInteract.isInteracted)
                {
                    objectCanvasGroup.alpha = 1;
                }
                else
                {
                    objectCanvasGroup.alpha = 0;
                }
            }
            if (isbusy) return;

            if (Input.GetKeyDown(KeyCode.E))
            {


                if (!isPicked)
                {
                    if (isCollected || ObjectInteract.isInteracted) return;
                    StartCoroutine(ObjectPickUp());
                }
                
            }
        }
        
        else if (playerInteract.GetObjectPickHandler() == null)
        {
            Avoid();
           
        }
       
        if (isPicked && Input.GetKeyDown(pickReferences.XrayToggle) && isLetter)
        {
            if (!isVision)
                XrayVisionEnable();
            else
                XrayVisionDisable();
        }
       
    }

    public IEnumerator ObjectPickUp()
    {
       

        isCollected = true;
        playerInteract.player.transform.LookAt(transform.position);

        isbusy = true;
        time = 0;
        isPicked = true;
       
        pickReferences.SwitchCam();

        pickReferences.inspectionBackroundimage.SetActive(true);

        objectTransform = transform.position;
        objectRotation = transform.rotation;
        transform.parent = pickReferences.objectContainer.transform;
        transform.rotation = PickUpRotation;

        while (time < 1f)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, time*2);
            yield return null;
        }

        transform.localPosition = Vector3.zero;
       
        pickReferences.FocusCam.Lens.FieldOfView = inspectionCamFov;
  

        
        yield return new WaitForSeconds(0.3f);
        isbusy = false;
    }

    public IEnumerator ObjectDrop()
    {
        XrayVisionDisable();
       
        transform.rotation = objectRotation;
        transform.parent = null;
        pickReferences.SwitchCam();
        isbusy = true;
        time = 0;

        pickReferences.inspectionBackroundimage.SetActive(false);


        while (time < 1f)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, objectTransform, time );
            yield return null;
        }
       
        transform.localPosition = objectTransform;

        isPicked = false;
        isCollected = false;

        if (checkClue)
        {
            int count = 0;

            foreach (string clues in clue)
            {
                if (ClueManager.Instance.ClueCheck(clues))
                {
                    count++;
                   
                }
            }
            if(count == clue.Length)
            {
                enabled = false;
                objectInteract.enabled = true;
                objectInteract.StartInteraction();
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
        
        objectCanvasGroup.alpha = 0;
    }
}
