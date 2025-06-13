using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
   [SerializeField]private float interactRange = 3f;
   [HideInInspector] public bool isPointAndMovementEnabled;
    private PointAndMovement pointAndMovement;
     private PlayerDialog playerDialog;
    

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
                if (objectInteract.interacting)
                {
                    isPointAndMovementEnabled = true;
                   
                }
                else
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
                if (objectPickHandler.isPicked)
                {
                    isPointAndMovementEnabled = true; 
                   
                }
                else
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
                return objectMoving;
            }

        }
        return null;
    }
}
