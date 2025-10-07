using UnityEngine;
using UnityEngine.UI;
using static ObjectInteract;

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

    private bool shouldTabletWork;

    private void Awake()
    {
        Instance = this;
    }

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
        bool pickHandlerInteract = ObjectPickHandler.Instance != null && ObjectPickHandler.Instance.InteractionCheck();
        bool movingInteract = ObjectMoving.canInteract; // assuming static bool, already safe
        bool interactObject = ObjectInteract.Instance != null && ObjectInteract.Instance.InteractionCheck();

        bool canInteract = pickHandlerInteract || movingInteract || interactObject;
        bool isPlayerInteracting = playerDialog != null && playerDialog.isInteraction;

        if (canInteract)
        {
            isPointAndMovementEnabled = true;

            if (pointAndMovement != null  && player != null)
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


    public ObjectInteract GetObjectInteract()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out ObjectInteract objectInteract))
            {
                if (objectInteract.type == InteractType.Tablet)
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out ObjectPickHandler objectPickHandler))
                return objectPickHandler;
        }
        return null;
    }

    public ObjectMoving ObjectMovingHandler()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out ObjectMoving objectMoving))
                return objectMoving;
        }
        return null;
    }

    public SceneChanger SceneChangerHandler()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out SceneChanger sceneChanger))
                return sceneChanger;
        }
        return null;
    }
}
