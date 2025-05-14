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
    public GameObject inspectionCamara;
    public CinemachineCamera MainCam, FocusCam;
    public ClickToMove clickToMove;
    public float rotationSensitivity;

    [SerializeField]private PlayerInteract playerInteract;
    [SerializeField] private CanvasGroup objectCanvasGroup;

    [SerializeField] private CanvasGroup fadeImage;
 
    private bool isFade = true;

    private bool isbusy = false;

    private float time;


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
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, time);
            time += Time.deltaTime ;
            yield return null;
        }

        transform.localPosition = Vector3.zero;

       
        isPicked = true;
     
       clickToMove.enabled = false;
        yield return new WaitForSeconds(1);
        isbusy = false;
    }
    public IEnumerator ObjectDrop()
    {

        SwitchCam();
        isbusy = true;
        time = 1;
  
        transform.parent =null;

        while (time > 0f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, objectTransform, time);
            time -= Time.deltaTime;
            yield return null;
        }

        transform.localPosition = objectTransform; // Snap to exact zero just in case

        isPicked = false;

        //transform.parent = null;
        //transform.position = objectTransform;
     
        transform.rotation = Quaternion.Euler(0, 0, 0);
        clickToMove.enabled = true;
        yield return new WaitForSeconds(1); // changed to 1 second
        isbusy = false;

    }
    void SwitchCam()
    {
        if (FocusCam.Priority == 0)
        {
            MainCam.Priority = 0;
            FocusCam.Priority = 1;
           
        }
        else if (FocusCam.Priority == 1)
        {
            MainCam.Priority = 1;
            FocusCam.Priority = 0;
         
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
