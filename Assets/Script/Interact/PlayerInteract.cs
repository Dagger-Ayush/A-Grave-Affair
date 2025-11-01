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
    public bool doPointAndMovementWork = false;
    private PointAndMovement pointAndMovement;
    private PlayerDialog playerDialog;
    public bool shouldTabletWork;

    // 🔹 Pause control
    private bool wasInteractingBeforePause = false;
    private bool isPaused = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        doPointAndMovementWork = false;
        pointAndMovement = GetComponent<PointAndMovement>();
        playerDialog = GetComponent<PlayerDialog>();
    }

    private void Update()
    {
        // If paused, don't process interactions
        if (isPaused)
            return;

        GetObjectInteract();
        GetObjectPickHandler();

        // Null-safe interaction checks
        bool pickHandlerInteract = ObjectPickHandler.isCollected;
        bool movingInteract = ObjectMoving.canInteract; // assuming static bool
        bool interactObject = ObjectInteract.isInteracting;
        bool canInteract = pickHandlerInteract || movingInteract || interactObject;

        bool isPlayerInteracting = playerDialog != null && playerDialog.isInteraction;

        if (canInteract || doPointAndMovementWork)
        {
            isPointAndMovementEnabled = true;

            if (player != null)
                player.GetComponent<Rigidbody>().isKinematic = true;

            if (tabletImage != null)
                tabletImage.enabled = false;

            if (pointAndMovement != null)
                pointAndMovement.enabled = false;

            if (animator != null)
                animator.SetBool("IsWalking", false);
        }
        else if (!canInteract && !isPlayerInteracting && !doPointAndMovementWork)
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

    // 🔹 Called by MainMenuUI when paused/unpaused
    public void OnPauseStateChanged(bool paused)
    {
        isPaused = paused;

        if (paused)
        {
            // Remember if player was in an interaction when paused
            wasInteractingBeforePause = ObjectPickHandler.isCollected ||
                                        ObjectMoving.canInteract ||
                                        ObjectInteract.isInteracting ||
                                        (playerDialog != null && playerDialog.isInteraction);

            // Stop player movement safely
            if (pointAndMovement != null)
                pointAndMovement.enabled = false;

            if (player != null)
                player.GetComponent<Rigidbody>().isKinematic = true;

            if (tabletImage != null)
                tabletImage.enabled = false;
        }
        else
        {
            // Resume based on previous state
            if (!wasInteractingBeforePause)
            {
                if (player != null)
                    player.GetComponent<Rigidbody>().isKinematic = false;

                if (pointAndMovement != null)
                    pointAndMovement.enabled = true;

                if (shouldTabletWork && tabletImage != null)
                    tabletImage.enabled = true;
            }

            wasInteractingBeforePause = false;
        }
    }

    public ObjectPickHandler GetObjectPickHandler()
    {
        if (TabletManager.isTabletOpen) return null;

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
