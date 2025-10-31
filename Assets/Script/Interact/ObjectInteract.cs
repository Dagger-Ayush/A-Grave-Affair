using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteract : MonoBehaviour
{
    public static ObjectInteract Instance;

    public enum InteractType { Tablet, DogBed, Badge, Lighter, InteractiveAutomatic, NonInteractiveAutomatic, Cigarette, None }
    public InteractType type = InteractType.None;

    [Header("Dialog System")]
    public DialogManager dialogManager;
    public TextMeshProUGUI dialogText;
    public GameObject dialogContainer;

    public static bool isInteracting;
    public static ObjectInteract activeInteraction; // ✅ new: track the currently active interaction

    [HideInInspector] public bool isInteracted;
    public bool DoAutoRun = false;

    [Header("Dialogue Interaction References")]
    [SerializeField] private CanvasGroup inRange;
    [SerializeField] private CanvasGroup outRange;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private ObjectPickReferences pickReferences;

    [Header("Dialogue State")]
    private int currentImageIndex = 0;
    [HideInInspector] public bool isSecondDialog = false;

    public bool shouldWork = false;
    private bool InteractedWithDogBed = false;
    [HideInInspector] public bool isAutoComplete = false;
    [HideInInspector] public bool isAutoCompleteNearObject = false;
    [HideInInspector] public bool isInteractionComplete = false;
    public event System.Action OnInteractionStarted;

    [Header("Clues")]
    public GettingClueCount gettingClueCount;
    public static int clueCount;
    private int totalClues;
    public static int clueCountMain;

    public Sprite jhonSpriteStore;
    private Image jhon;

    private void OnDisable()
    {
        if (activeInteraction == this)
            activeInteraction = null; // ✅ release active interaction if this is the one being disabled

        if (outRange != null) outRange.alpha = 0;
        if (inRange != null) inRange.alpha = 0;
    }

    private void Awake()
    {
        Instance = this;
        jhon = dialogContainer.GetComponent<Image>();
    }

    private void Start()
    {
        if (dialogManager != null)
        {
            if (dialogManager.currentClueCount == null || dialogManager.currentClueCount.Length != dialogManager.dialogLines.Length)
                dialogManager.currentClueCount = new int[dialogManager.dialogLines.Length];

            dialogManager.ResetClues();
        }

        if (type == InteractType.Tablet)
        {
            outRange.alpha = 1;
            shouldWork = true;
        }

        if (type == InteractType.DogBed)
            enabled = false;


    }

    private void Update()
    {

        if ((ObjectPickHandler.Instance != null && ObjectPickHandler.Instance.InteractionCheck())
           && outRange != null && inRange != null)
        {
            outRange.alpha = 0;
            inRange.alpha = 0;
            return;
        }
        if (!shouldWork) return;

        if (type == InteractType.NonInteractiveAutomatic && !isAutoComplete)
        {
            if (activeInteraction != null && activeInteraction != this)
                return;

            if (!isInteracted)
            {
                StartInteraction();
                playerInteract.doPointAndMovementWork = true;
            }
            else if (isInteracted && Input.GetKeyDown(KeyCode.E)) NextDialogueImage();
            return;
        }
        if (type == InteractType.DogBed && !InteractedWithDogBed)
        {
            if (!isInteracted) StartInteraction();
            else if (isInteracted && Input.GetKeyDown(KeyCode.E)) NextDialogueImage();
            return;
        }


        if (ObjectPickHandler.Instance != null)
        {
            if (inRange != null && (isInteracted || ObjectPickHandler.Instance.InteractionCheck()))
                inRange.alpha = 0;
        }

        if (playerInteract.GetObjectInteract() == this)
        {

            if (outRange != null) outRange.alpha = 0;
            if (ObjectPickHandler.Instance != null && inRange != null)
                inRange.alpha = (!isInteracted && !ObjectPickHandler.Instance.InteractionCheck()) ? 1 : 0;

            if (DoAutoRun && type == InteractType.InteractiveAutomatic && !isAutoCompleteNearObject)
            {
                if (!isInteracted) StartInteraction();
                else if (isInteracted && Input.GetKeyDown(KeyCode.E)) NextDialogueImage();
                return;
            }

            ObjectHandler();
        }
        else if (playerInteract.GetObjectInteract() == null)
        {
            Avoid();
        }

        if (gettingClueCount != null)
            gettingClueCount.UpdateTick(clueCount);
    }

    private void ObjectHandler()
    {
        if (TabletManager.Instance != null && pickReferences?.interactionTutorial != null)
            if (TabletManager.isTabletOpen || pickReferences.interactionTutorial.isRunning) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isInteracted && !ObjectPickHandler.Instance.InteractionCheck())
                StartInteraction();
            else
                NextDialogueImage();
        }

        if (type == InteractType.Tablet && !isInteracted && currentImageIndex < dialogManager.dialogLines.Length)
        {
            StartInteraction();
            gameObject.GetComponent<Renderer>().enabled = false;
        }
    }

    public void StartInteraction()
    {
        // ✅ Prevent multiple objects interacting at once
        if (activeInteraction != null && activeInteraction != this)
            return;
        if (ObjectPickHandler.activePickup != null) return;

        activeInteraction = this; // mark this as the current active one

        if (ObjectPickHandler.Instance != null && ObjectPickHandler.Instance.InteractionCheck()) return;
        OnInteractionStarted?.Invoke();
        isInteracting = true;
        isInteracted = true;

        if (type == InteractType.NonInteractiveAutomatic && !isAutoComplete)
        { playerInteract.doPointAndMovementWork = true; }

        if (currentImageIndex >= dialogManager.dialogLines.Length)
            currentImageIndex = 0;

        clueCount = dialogManager.currentClueCount[currentImageIndex];
        totalClues = dialogManager.totalCount[currentImageIndex];
        clueCountMain = clueCount;

        gettingClueCount?.AddTick(clueCount, totalClues);

        TypeLine();

        if (ObjectPickReferences.instance != null)
        {
            ObjectPickReferences.instance.XrayOnImage.SetActive(false);
            ObjectPickReferences.instance.XrayOfImage.SetActive(false);
        }
    }

    public void NextDialogueImage()
    {
        isSecondDialog = true;

        if (pickReferences.nextPageSound != null)
            pickReferences.nextPageSound.Play();

        if (dialogManager.currentClueCount != null && currentImageIndex < dialogManager.currentClueCount.Length)
            dialogManager.currentClueCount[currentImageIndex] = clueCount;

        dialogContainer.SetActive(false);

        if (dialogManager.dialogAudio.Length > currentImageIndex &&
            dialogManager.dialogAudio[currentImageIndex]?.sorce != null)
            AudioManager.Instance.Stop();

        currentImageIndex++;

        if (currentImageIndex < dialogManager.dialogLines.Length)
        {
            clueCount = dialogManager.currentClueCount[currentImageIndex];
            totalClues = dialogManager.totalCount[currentImageIndex];
            clueCountMain = totalClues;

            dialogText.SetText(dialogManager.dialogLines[currentImageIndex]);

            if (CursorHoverOverClue.instance != null)
                CursorHoverOverClue.instance.RefreshPermanentOutlines();

            TypeLine();
        }
        else
        {
            gettingClueCount?.DisableAll();
            HandlePostDialogueActions();

            // ✅ Keep interaction active for NonInteractiveAutomatic until it’s marked complete
            if (type == InteractType.NonInteractiveAutomatic)
            {
                playerInteract.doPointAndMovementWork = false;
            }
            // Default end of interaction
            isInteracting = false;
            isInteracted = false;
            activeInteraction = null;
        }

    }

    private void HandlePostDialogueActions()
    {
        isInteractionComplete = true;

        if (type == InteractType.Tablet)
        {
            TabletUnlocker.instance.UnlockTablet();

            if (pickReferences.ObjectInteractBadge.type == InteractType.Badge)
                pickReferences.ObjectInteractBadge.shouldWork = true;

            if (pickReferences.ObjectPickHandlerCigarette.type == ObjectPickHandler.InspectType.Cigarette)
                pickReferences.ObjectPickHandlerCigarette.shouldWork = true;

            Destroy(gameObject, 0.1f);
        }

        if (type == InteractType.DogBed && pickReferences.lighterObjectPickHandler != null)
        {
            pickReferences.lighterObjectPickHandler.enabled = true;
            InteractedWithDogBed = true;
        }

        if (type == InteractType.InteractiveAutomatic)
        {
            isAutoCompleteNearObject = true;
        }

        if (type == InteractType.NonInteractiveAutomatic)
        {
            isAutoComplete = true;
        }

        if (type == InteractType.Lighter)
            Destroy(gameObject, 0.1f);
    }

    private void Avoid()
    {
        if (activeInteraction == this)
            activeInteraction = null;

        isInteracting = false;
        isInteracted = false;

        if (inRange != null && shouldWork && !isInteracted)
        {
            inRange.alpha = 0;
            outRange.alpha = 1;
        }
    }

    private void TypeLine()
    {
        if (dialogManager.backgroundImages != null &&
            currentImageIndex < dialogManager.backgroundImages.Length && dialogManager.doBackgroundChange)
        {
            jhon.sprite = dialogManager.backgroundImages[currentImageIndex];
        }
        else
        {
            jhon.sprite = jhonSpriteStore;
        }

        dialogContainer.SetActive(true);

        dialogText.fontSize = (dialogManager.changeFontSize && dialogManager.frontSize != null && currentImageIndex < dialogManager.frontSize.Length)
            ? dialogManager.frontSize[currentImageIndex] : 45;

        dialogText.SetText((dialogManager.dialogLines != null && currentImageIndex < dialogManager.dialogLines.Length)
            ? dialogManager.dialogLines[currentImageIndex] : "");

        if (dialogManager.dialogAudio != null &&
            currentImageIndex < dialogManager.dialogAudio.Length &&
            dialogManager.dialogAudio[currentImageIndex]?.sorce != null)
            AudioManager.Instance.PlayDialogLine(dialogManager, currentImageIndex);

        if (gettingClueCount != null)
            gettingClueCount.AddTick(clueCount, totalClues);
    }

    public int GetCurrentClueCount() => clueCount;
    public int GetPickedCount() => dialogManager.currentClueCount[currentImageIndex];

    public void UpdateClueCount()
    {
        if (dialogManager.currentClueCount != null && currentImageIndex < dialogManager.currentClueCount.Length)
        {
            clueCount = dialogManager.currentClueCount[currentImageIndex];
            gettingClueCount?.AddTick(clueCount, totalClues);
        }
    }

    public static bool InteractionCheck() => isInteracting;

    void SwitchPage()
    {
        CursorHoverOverClue.instance?.StopHover();
        CursorHoverOverClue.instance?.ClearPermanentOutlines();
        CursorHoverOverClue.instance?.RefreshPermanentOutlines();
    }
}
