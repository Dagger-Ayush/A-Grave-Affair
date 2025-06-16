using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactRange = 3f;
    [HideInInspector] public bool isPointAndMovementEnabled;
    private PointAndMovement pointAndMovement;
    private PlayerDialog playerDialog;
    public Transform player;

    private void Start()
    {
        pointAndMovement = GetComponent<PointAndMovement>();
        playerDialog = GetComponent<PlayerDialog>();
    }
    void Update()
    {
        GetObjectInteract();
        GetObjectPickHandler();

        if (isPointAndMovementEnabled)
        {
            pointAndMovement.agent.SetDestination(player.transform.position);
           
            pointAndMovement.enabled = false;
        }
        else if (!isPointAndMovementEnabled && !playerDialog.isInteraction)
        {
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
                if (objectInteract.interacting && objectInteract.enabled == true)
                {
                    isPointAndMovementEnabled = true;
                  
                }
                else if (!objectInteract.interacting && objectInteract.enabled == true)
                {
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
                if (objectPickHandler.isPicked && objectPickHandler.enabled == true)
                {
                    isPointAndMovementEnabled = true;
                   
                }
                else if (!objectPickHandler.isPicked && objectPickHandler.enabled ==true)
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
