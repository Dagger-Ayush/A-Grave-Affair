using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ObjectHovering : MonoBehaviour
{
    public static ObjectHovering instance;
    float radius = 0.2f;
    [SerializeField] private PlayerInteract playerInteract;
    //[SerializeField] private Texture2D cursorTextureInRange;
    // [SerializeField] private Texture2D cursorTextureOutRange;

    [SerializeField] private AudioSource cursorAudioClip;
    private bool isSoundPlayed = false;
    private InteractClueManager interactClueManager; // for internal Use
    private Vector2 cursorHotspot;


    [HideInInspector] public bool isRunning = false; //maintaining the balance between ObjectHowering and CursorHoverOverClue cursor changing
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
                    if (playerInteract.GetObjectPickHandler() == null) return;


                    if ( distance <= maxRange && playerInteract.GetObjectPickHandler().isPicked )
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
        Cursor.lockState = CursorLockMode.Locked;
        playerInteract.GetObjectPickHandler().isMouseLocked = true;
        interactClueManager.ClueIndication();
        yield return new WaitForSeconds(1.8f);
        CursorManager.Instance.SetNormalCursor();
        playerInteract.GetObjectPickHandler().isMouseLocked = false;
        Cursor.lockState = CursorLockMode.None;
        interactClueManager.isFinished = true;

    }
    IEnumerator Delay()
    {
       
        isSoundPlayed = true;
       
        yield return new WaitForSeconds(3f);
        isSoundPlayed = false;
    }
}
