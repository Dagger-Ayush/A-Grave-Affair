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
            backGroundAudio.sorce.Play();
        }
    }

    private void Update()
    {
        GetObjectInteract();
        GetObjectPickHandler();

        // Interaction / Pickup / Movement checks
        if (ObjectInteract.isInteracted || ObjectPickHandler.isCollected || ObjectMoving.canInteract)
        {
            isPointAndMovementEnabled = true;
            pointAndMovement.agent.SetDestination(player.transform.position);
            tabletImage.enabled = false;
            pointAndMovement.enabled = false;
            animator.SetBool("IsWalking", false);
        }
        else if (!ObjectInteract.isInteracted &&
                 !ObjectPickHandler.isCollected &&
                 !ObjectMoving.canInteract &&
                 !playerDialog.isInteraction)
        {
            isPointAndMovementEnabled = false;

            if (shouldTabletWork)
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
