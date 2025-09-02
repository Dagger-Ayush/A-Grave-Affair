using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System;

public class ObjectPickHandler : MonoBehaviour
{
    public static ObjectPickHandler Instance;

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
    [SerializeField] private CanvasGroup inRange;
    [SerializeField] private CanvasGroup outRange;

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
    public bool isCigarette = false;
    public bool isLetter_1 = false;
    public bool isLetter_2 = false;

    [SerializeField]private bool checkClue = false;


    private ObjectInteract objectInteract;
    [SerializeField] private string[] clue;

    public bool shouldWork = false;


    public static int clueCount;
    public int totalClues;
    public int clueCountStoring = 0;
    public static int clueCountMain;
    public bool WillClueCountStop = false;

    private ObjectInteract objectInteractCigarette;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (isCigarette)
        {
            objectInteractCigarette = GetComponent<ObjectInteract>();
            objectInteractCigarette.enabled = false;

        }
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
        clueCount = 0;

    }
    private void Update()
    {
        ObjectHandler();

        if (clueCount < clueCountMain)
        {
            pickReferences.currentClueCount.text = "Clues Found (" + clueCount.ToString() + "/" + clueCountMain.ToString() + ")";
        }
        else
        {
            pickReferences.currentClueCount.text = "Clue's Picked(" + clueCountMain.ToString() + "/" + clueCountMain.ToString() + ")";
        }
    }

    private void ObjectHandler()
    {

        if (!shouldWork) return;
        outRange.alpha = 0;
        if (inRange != null)
        {
            if (isCollected || ObjectInteract.isInteracted)
            {
                inRange.alpha = 0;
            }
        }
        if (isPicked && Input.GetKeyDown(KeyCode.E))
        {
            if (isbusy || InteractClueManager.isClueShowing || pickReferences.inspectionTutorial.isRunning) return;
            StartCoroutine(ObjectDrop());
        }
        if (playerInteract.GetObjectPickHandler() == this)
        {
            if (inRange != null)
            {
                if (!isCollected && !ObjectInteract.isInteracted)
                {
                    inRange.alpha = 1;
                }
                else
                {
                    inRange.alpha = 0;
                }
            }
            if (isbusy) return;

            if (Input.GetKeyDown(KeyCode.E))
            {


                if (!isPicked)
                {
                    if (isCollected || ObjectInteract.isInteracted ) return;
                    StartCoroutine(ObjectPickUp());
                }
                
            }
        }
        
        else if (playerInteract.GetObjectPickHandler() == null)
        {
            Avoid();
           
        }
       if(isCollected )//&& isLetter_1 || isLetter_2)
        {
            
            if (Input.GetKeyDown(pickReferences.XrayToggle))
            {
                if (!isVision)
                    XrayVisionEnable();
                else
                    XrayVisionDisable();
            }
            if (!isVision)
            {
                pickReferences.XrayOnImage.SetActive(false);
                pickReferences.XrayOfImage.SetActive(true);
            }
            else
            {
                pickReferences.XrayOfImage.SetActive(false);
                pickReferences.XrayOnImage.SetActive(true);
            }
        }
        else if(!isCollected)
        {
            XrayVisionDisable();
            pickReferences.XrayOfImage.SetActive(false);
            pickReferences.XrayOnImage.SetActive(false);
        }
     
       
    }

    public IEnumerator ObjectPickUp()
    {

        clueCount = clueCountStoring;
        clueCountMain = totalClues;

        isCollected = true;
        pickReferences.currentClue.SetActive(true);

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
        if (isLetter_2 && isPicked && isCollected && isVision)
        {
            pickReferences.gameOverTrigger.SetActive(true);
        }


    }

    public IEnumerator ObjectDrop()
    {
        clueCountStoring = clueCount;
        pickReferences.currentClue.SetActive(false);

        XrayVisionDisable();
       
        transform.rotation = objectRotation;
        transform.parent = null;
        pickReferences.SwitchCam();
        isbusy = true;
        time = 0;

        pickReferences.inspectionBackroundimage.SetActive(false);

        if (isCigarette)
        {
            objectInteractCigarette.enabled = true;

            objectInteractCigarette.shouldWork = true;
            enabled = false;
        }
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

        if (isLetter_2 && isCollected )
        {
            pickReferences.gameOverTrigger.SetActive(true);
        }
    }

    private void XrayVisionDisable()
    {
        isVision = false;
        if (XrayObject != null) XrayObject.SetActive(false);
        pickReferences. XrayCamara.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!isPicked|| isVision) return;

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
        if (!isPicked || isMouseLocked|| isVision) return;
       
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
       if (shouldWork && !isCollected)
        {
            inRange.alpha = 0;
            outRange.alpha = 1;
        }
       
    }
}
