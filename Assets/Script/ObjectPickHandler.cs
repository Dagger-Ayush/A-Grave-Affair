using System.Collections;
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
    public Camera mainCamara, inspectionCamara;
    public ClickToMove clickToMove;
    public float rotationSensitivity;

    [SerializeField]private PlayerInteract playerInteract;
    [SerializeField] private CanvasGroup objectCanvasGroup;

    [SerializeField] private CanvasGroup fadeImage;
 
    private bool isFade = true;

    private bool isbusy = false;

  
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
           
            if (isPicked)
            {
                if (Input.GetMouseButton(0))
                {
                    ObjectRotation();
                }
            }
        }
        else if (playerInteract.GetObjectPickHandler() == null)
        {
            Avoid();
        }

    }
    public IEnumerator ObjectPickUp()
    {   isbusy = true;
        StartCoroutine(FadeScreen());
        isPicked = true;
        yield return new WaitForSeconds(1);
       
       
        objectTransform = transform.position;

        transform.parent = objectContainer.transform;
        transform.localPosition = Vector3.zero;

       mainCamara.gameObject.SetActive(false);
       inspectionCamara.gameObject.SetActive(true);

       clickToMove.enabled = false;
        yield return new WaitForSeconds(1);
        isbusy = false;
    }
    public IEnumerator ObjectDrop()
    {
        isbusy = true;
        StartCoroutine(FadeScreen());
       
        yield return new WaitForSeconds(1);
        isPicked = false;

        transform.parent = null;
        transform.position = objectTransform;
        mainCamara.gameObject.SetActive(true);
        inspectionCamara.gameObject.SetActive(false);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        clickToMove.enabled = true;
        yield return new WaitForSeconds(2);
        isbusy = false;
    }
    /*
    private void ObjectRotation()
    {
            turn.x += Input.GetAxis("Mouse X") * rotationSensitivity;
            turn.y += Input.GetAxis("Mouse Y") * rotationSensitivity;
            transform.localRotation = Quaternion.Euler(turn.y, -turn.x, 0);


    }*/
    private void ObjectRotation()
    {
        Vector3 mousePos = Input.mousePosition;

        // Check if mouse is within screen boundss
        if (mousePos.x >= 0 && mousePos.x <= Screen.width &&
            mousePos.y >= 0 && mousePos.y <= Screen.height)
        {
            turn.x += Input.GetAxis("Mouse X") * rotationSensitivity;
            turn.y += Input.GetAxis("Mouse Y") * rotationSensitivity;
        }

        transform.localRotation = Quaternion.Euler(turn.y, -turn.x, 0);
    }
    private void Avoid()
    {
        isPicked = false;
        objectCanvasGroup.alpha = 0;
    }
    private IEnumerator FadeScreen()
    {
        if (isFade)
        {
            // Fade out
            while (fadeImage.alpha < 1f)
            {
                fadeImage.alpha += Time.deltaTime;
                yield return null;
            }



            // Fade in
            while (fadeImage.alpha > 0f)
            {
                fadeImage.alpha -= Time.deltaTime;
                yield return null;
            }
            isFade = false;
        }
        if (!isFade)
        {   isFade = true;
            fadeImage.alpha = 0;
        }
    }

}
