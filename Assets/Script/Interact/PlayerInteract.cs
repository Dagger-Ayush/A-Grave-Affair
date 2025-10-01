using UnityEngine;
using UnityEngine.UI;
using static ObjectInteract;

public class PlayerInteract : MonoBehaviour
{
    public static PlayerInteract Instance;

    [Header("Interaction Settings")]
    [SerializeField] private float interactRange = 1f;

    [Header("References")]
    [SerializeField] private DialogAudio backGroundAudio;
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

        if (backGroundAudio != null)
        {
           // backGroundAudio.sorce.Play();
        }
    }

    private void Update()
    {
        GetObjectInteract();
        GetObjectPickHandler();

        // Interaction / Pickup / Movement checks
        bool canInteract =  (ObjectPickHandler.Instance.InteractionCheck())
                           || (ObjectMoving.canInteract)
                           || (ObjectInteract.Instance.InteractionCheck());

        bool isPlayerInteracting = playerDialog != null && playerDialog.isInteraction;

        if (canInteract)
        {
            isPointAndMovementEnabled = true;

            if (pointAndMovement != null && pointAndMovement.agent != null && player != null)
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
            player.GetComponent<Rigidbody>().isKinematic = false;
            isPointAndMovementEnabled = false;

            if (shouldTabletWork && tabletImage != null)
                tabletImage.enabled = true;

            if (pointAndMovement != null)
                pointAndMovement.enabled = true;
        }

    }

    // -------------------
    // Interaction Handlers
    // -------------------
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
