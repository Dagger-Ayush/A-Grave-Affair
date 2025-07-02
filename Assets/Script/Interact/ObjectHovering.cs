using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ObjectHovering : MonoBehaviour
{
    public static ObjectHovering instance;
    float radius = 0.2f;
   
    [SerializeField] private AudioSource cursorAudioClip;
    private bool isSoundPlayed = false;
    private InteractClueManager interactClueManager; // for internal Use
    private Vector2 cursorHotspot;

    [SerializeField] private float soundDelay = 3f;
    public static bool isRunning = false; //maintaining the balance between ObjectHowering and CursorHoverOverClue cursor changing

   private static bool isBusy = false;
    private void Awake()
    {

        instance = this;
    }
    void Update()
    {
        ObjectDectecting();

    }
    void ObjectDectecting()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit)  )
        {

            Collider[] colliderArray = Physics.OverlapSphere(hit.point, radius);

            bool objectInRange = false;
            bool mouseHovering = false;

            foreach (Collider collider in colliderArray)
            {

                if (collider.GetComponent<InteractClueManager>() && !collider.GetComponent<InteractClueManager>().isFinished)
                {
                    interactClueManager = collider.GetComponent<InteractClueManager>();

                    float minRange = collider.GetComponent<InteractClueManager>().HoveringMinRange;
                    float maxRange = collider.GetComponent<InteractClueManager>().HoveringMaxRange;


                    float distance = Vector3.Distance(hit.point, collider.transform.position);

                  
                    if ( distance <= maxRange && ObjectPickHandler.isCollected)
                    {
                        if (distance <= collider.GetComponent<InteractClueManager>().HoveringRange )
                        {

                            mouseHovering = true;
                           
                        }

                        objectInRange = true;
                        break;
                    }

                    


                }


            }

            //cursorHotspot = new Vector2(cursorTextureInRange.width / 2, cursorTextureInRange.height / 2);
            if ( !isRunning) 
            {
                if (objectInRange)
                {

                    if (!isSoundPlayed)
                    {
                        if(isBusy)return;
                        cursorAudioClip.Play();

                        StartCoroutine(Delay());
                    }
                }

                if (mouseHovering)
                {
                    CursorManager.Instance.SetClueCursor();
                    // Cursor.SetCursor(cursorTextureInRange, cursorHotspot, CursorMode.Auto);

                    if (Input.GetMouseButtonDown(0))
                    {
                       
                        StartCoroutine(WordPicking(interactClueManager));
                    }
                }
                else
                {

                    CursorManager.Instance.SetNormalCursor();
                    //Cursor.SetCursor(cursorTextureOutRange, cursorHotspot, CursorMode.Auto);
                    mouseHovering = false;

                }
            }
        }
    }
    IEnumerator WordPicking(InteractClueManager interactClueManager)
    {
        isBusy = true;
        Cursor.lockState = CursorLockMode.Locked;
        ObjectPickHandler.isMouseLocked = true;
        interactClueManager.ClueIndication();
        yield return new WaitForSeconds(1.8f);
        CursorManager.Instance.SetNormalCursor();
        ObjectPickHandler.isMouseLocked = false;
        Cursor.lockState = CursorLockMode.None;
        interactClueManager.isFinished = true;
        isBusy = false;
    }
    IEnumerator Delay()
    {
       
        isSoundPlayed = true;
       
        yield return new WaitForSeconds(soundDelay);
        isSoundPlayed = false;
    }
}
