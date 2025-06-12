using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class ObjectPickHandler : MonoBehaviour
{
    [Header("Object Handling reference's")]
   [HideInInspector] public bool isPicked;
    //Before Picking transform of object
    private Vector3 objectTransform;
    //Mouse 
    private Vector2 turn;

    public GameObject objectContainer;
    public GameObject inspectionBackroundimage;
    public GameObject inspectionCamara;
    public CinemachineCamera MainCam, FocusCam;
    public PointAndMovement pointAndMovement;
    public float rotationSensitivity;

    [SerializeField]private PlayerInteract playerInteract;
    [SerializeField] private CanvasGroup objectCanvasGroup;

    [SerializeField] private CanvasGroup fadeImage;
 
    private bool isFade = true;

    private bool isbusy = false;

    private float time;

    [Header("X ray refreence's")]
    private bool isVision = false;

    [SerializeField] private Material DefaultMaterial;
    [SerializeField]private Material visionMaterial;
    [SerializeField]private GameObject XrayCamara;
    [SerializeField] private KeyCode XrayToggle;

    private void Update()
    {
        if (playerInteract.GetObjectPickHandler() == this)
        {
            if (!isPicked) { objectCanvasGroup.alpha = 1; }
            else { objectCanvasGroup.alpha = 0; }

            if (isbusy) return;

           

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!isPicked)
                {
                    StartCoroutine(ObjectPickUp());
  
                }
                else
                {
                   
                    StartCoroutine(ObjectDrop());
                }
            }
        }
        else if (playerInteract.GetObjectPickHandler() == null)
        {
            Avoid();
        }
        if(isPicked && Input.GetKeyDown(XrayToggle)) { 
             if (!isVision)
                {
                     XrayVisionEnable();
                }
              else if (isVision)
                {

                     XrayVisionDisable();
                }
        }

    }
    public IEnumerator ObjectPickUp()
    {
        SwitchCam();
        isbusy = true; 
        time = 0;
        objectTransform = transform.position;
        transform.parent = objectContainer.transform;

        while (time < 1f)
        {
            time += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, time);
            
            yield return null;
        }

        transform.localPosition = Vector3.zero;

       
        isPicked = true;
     

       pointAndMovement.enabled = false;
        yield return new WaitForSeconds(0.45f);
        inspectionBackroundimage.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        isbusy = false;

        
    }
    public IEnumerator ObjectDrop()
    {
        XrayVisionDisable();
        inspectionBackroundimage.SetActive(false);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        SwitchCam();
        isbusy = true;
        time = 1;
  
        transform.parent =null;

        while (time > 0f)
        {
            time -= Time.deltaTime/2;
            transform.localPosition = Vector3.Lerp(transform.localPosition, objectTransform, time);
           
            yield return null;
        }

        transform.localPosition = objectTransform; // Snap to exact zero just in case

        isPicked = false;

        //transform.parent = null;
        //transform.position = objectTransform;
        pointAndMovement.enabled = true;
        yield return new WaitForSeconds(1); // changed to 1 second
        isbusy = false;

    }
    void SwitchCam()
    {
        if (FocusCam.Priority == 0)
        {
            MainCam.Priority = 0;
            FocusCam.Priority = 1;
            MainCam.Lens.ModeOverride = LensSettings.OverrideModes.Perspective;
            FocusCam.Lens.ModeOverride = LensSettings.OverrideModes.Perspective;
        }
        else if (FocusCam.Priority == 1)
        {
            MainCam.Priority = 1;
            FocusCam.Priority = 0;
            MainCam.Lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
            FocusCam.Lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
        }
    }
    void XrayVisionEnable()
    {
        isVision = true;
        XrayCamara.SetActive (true);
        if (visionMaterial != null)
        {
            //this.gameObject.GetComponent<MeshRenderer>().material = visionMaterial;
        }
      
    }
    void XrayVisionDisable()
    {
        isVision = false;
        XrayCamara.SetActive(false);
        if (DefaultMaterial != null)
        {
            //this.gameObject.GetComponent<MeshRenderer>().material = DefaultMaterial;
        }
    }
    private void OnMouseDrag()
    {
        if (!isPicked) return;

        turn.x = Input.GetAxis("Mouse X") * rotationSensitivity;
        turn.y = Input.GetAxis("Mouse Y") * rotationSensitivity;

        //transform.localRotation = Quaternion.Euler(0, -turn.x, turn.y);
        Vector3 right = Vector3.Cross(inspectionCamara.transform.up, transform.position - inspectionCamara.transform.position);
        Vector3 up = Vector3.Cross(transform.position - inspectionCamara.transform.position, right);

        transform.rotation = Quaternion.AngleAxis(-turn.x, up) * transform.rotation;
        transform.rotation = Quaternion.AngleAxis(turn.y, right) * transform.rotation;
    }
    private void Avoid()
    {
        isPicked = false;
        objectCanvasGroup.alpha = 0;
    }
    

}
