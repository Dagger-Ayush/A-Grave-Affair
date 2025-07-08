using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 1f;
    [HideInInspector] public bool isPointAndMovementEnabled;
    private PointAndMovement pointAndMovement;
    private PlayerDialog playerDialog;
    public Transform player;
    [SerializeField] private DialogAudio BackGroundAudio;

    public Image tabletImage;
    private bool shouldTabletWork;
    private void Start()
    {
       
        pointAndMovement = GetComponent<PointAndMovement>();
        playerDialog = GetComponent<PlayerDialog>();



        BackGroundAudio.sorce.Play();
    }
    void Update()
    {
        GetObjectInteract();
        GetObjectPickHandler();

        if (ObjectInteract.isInteracted || ObjectPickHandler.isCollected || isPointAndMovementEnabled)
        {
            pointAndMovement.agent.SetDestination(player.transform.position);
            tabletImage.enabled= false;
            pointAndMovement.enabled = false;
        }
        else if (!ObjectInteract.isInteracted || !ObjectPickHandler.isCollected || !isPointAndMovementEnabled && !playerDialog.isInteraction)
        {
            if (shouldTabletWork)
            {
                tabletImage.enabled = true;
            }
           
            pointAndMovement.enabled = true;
            
        }


    }

    public ObjectInteract GetObjectInteract()
    {
        
        //checking if the object is in range

        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ObjectInteract objectInteract))
            {
                if (ObjectInteract.isInteracted && objectInteract.enabled == true)
                {
                    isPointAndMovementEnabled = true;
 
                }
                else if (!ObjectInteract.isInteracted && objectInteract.enabled == true)
                {
                  if( objectInteract.isTablet == true)
                    { 
                        shouldTabletWork = true; 
                    }
                    isPointAndMovementEnabled = false;
                }
                return objectInteract;
            }

        }
        return null;
    }
    public ObjectPickHandler GetObjectPickHandler()
    {
        //checking if the object is in range

        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ObjectPickHandler objectPickHandler))
            {
                if (ObjectPickHandler.isCollected && objectPickHandler.enabled == true)
                {
                    isPointAndMovementEnabled = true;
                   
                }
                else if (!ObjectPickHandler.isCollected && objectPickHandler.enabled ==true)
                {
                    
                    isPointAndMovementEnabled = false;
                }
                return objectPickHandler;
            }

        }
        return null;
    }
    public ObjectMoving ObjectMoving()
    {
        //checking if the object is in range

        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ObjectMoving objectMoving))
            {
                if (objectMoving.canInteract)
                {
                    isPointAndMovementEnabled = true;

                }
                else
                {
                    isPointAndMovementEnabled = false;
                }
                return objectMoving;
            }

        }
        return null;
    }
}
