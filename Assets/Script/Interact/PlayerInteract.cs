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
    [SerializeField] private Animator animator;

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

        if (ObjectInteract.isInteracted || ObjectPickHandler.isCollected || ObjectMoving.canInteract)
        {
            isPointAndMovementEnabled = true;
            pointAndMovement.agent.SetDestination(player.transform.position);
            tabletImage.enabled= false;
            pointAndMovement.enabled = false;
            animator.SetBool("IsWalking", false);
        }
        else if (!ObjectInteract.isInteracted || !ObjectPickHandler.isCollected || !ObjectMoving.canInteract  && !playerDialog.isInteraction)
        {
            isPointAndMovementEnabled = false;
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
                if (objectInteract.isTablet == true)
                {
                    shouldTabletWork = true;
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
               
                return objectPickHandler;
            }

        }
        return null;
    }
    public ObjectMoving ObjectMovingHandler()
    {
        //checking if the object is in range

        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ObjectMoving objectMoving))
            {
               
                return objectMoving;
            }

        }
        return null;
    }
}
