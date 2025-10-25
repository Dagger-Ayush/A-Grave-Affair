using UnityEngine;
using UnityEngine.UI;

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 1f;

    [Header("References")]
    [SerializeField] private Animator animator;
    public Transform player;
    public Image tabletImage;

    [HideInInspector] public bool isPointAndMovementEnabled;
    private PointAndMovement pointAndMovement;
    private PlayerDialog playerDialog;
    public bool shouldTabletWork;

    private void Awake() { Instance = this; }

    private void Start()
    {
        pointAndMovement = GetComponent<PointAndMovement>();
        playerDialog = GetComponent<PlayerDialog>();
    }

    private void Update()
    {
        GetObjectInteract();
        GetObjectPickHandler();

        // Null-safe interaction checks
        bool pickHandlerInteract = ObjectPickHandler.isCollected;
        bool movingInteract = ObjectMoving.canInteract; // assuming static bool
        bool interactObject = ObjectInteract.isInteracting;
        bool canInteract = pickHandlerInteract || movingInteract || interactObject;

        bool isPlayerInteracting = playerDialog != null && playerDialog.isInteraction;

        if (canInteract)
        {
            isPointAndMovementEnabled = true;
            if (pointAndMovement != null && player != null)
                player.GetComponent<Rigidbody>().isKinematic = true;

            if (tabletImage != null)
                tabletImage.enabled = false;

            if (pointAndMovement != null)
                pointAndMovement.enabled = false;

            if (animator != null)
                animator.SetBool("IsWalking", false);
        }
        else if (!canInteract && !isPlayerInteracting)
        {
            if (player != null)
                player.GetComponent<Rigidbody>().isKinematic = false;

            isPointAndMovementEnabled = false;

            if (shouldTabletWork && tabletImage != null)
                tabletImage.enabled = true;

            if (pointAndMovement != null)
                pointAndMovement.enabled = true;
        }
    }

    public ObjectPickHandler GetObjectPickHandler()
    {
        if(TabletManager.isTabletOpen)return null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out ObjectPickHandler objectPickHandler))
                return objectPickHandler; // Highest priority
        }
        return null;
    }

    public ObjectMoving ObjectMovingHandler()
    {
        if (TabletManager.isTabletOpen) return null;
        /*
        if (GetObjectPickHandler() != null) // check PickHandler first
            return null;
        */
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out ObjectMoving objectMoving))
                return objectMoving;
        }
        return null;
    }

    public ObjectInteract GetObjectInteract()
    {
        if (TabletManager.isTabletOpen) return null;
        /*
        if (GetObjectPickHandler() != null || ObjectMovingHandler() != null) // check higher priority first
            return null;
        */
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out ObjectInteract objectInteract))
            {
                if (objectInteract.type == ObjectInteract.InteractType.Tablet)
                    shouldTabletWork = true;

                return objectInteract;
            }
        }
        return null;
    }


    public SceneChanger SceneChangerHandler()
    {
        if (TabletManager.isTabletOpen) return null;

        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out SceneChanger sceneChanger))
                return sceneChanger;
        }
        return null;
    }
}
