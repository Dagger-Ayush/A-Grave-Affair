using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ObjectPickHandler;

public class ObjectInteract : MonoBehaviour
{
    public static ObjectInteract Instance;

    public enum InteractType { Tablet, DogBed, Badge, Lighter, InteractiveAutomatic, NonInteractiveAutomatic, None }
    public InteractType type = InteractType.None;

    [Header("Dialog System")]
    public DialogManager dialogManager;
    public AudioManager audioManager;
    public TextMeshProUGUI dialogText;
    public GameObject dialogContainer;

    public static bool isInteracting;
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
     public bool isAutoComplete = false;
    [HideInInspector] public bool isAutoCompleteNearObject = false;

    [Header("Clues")]
    public GettingClueCount gettingClueCount;
    public static int clueCount;
    private int totalClues;              // max clues this line  
    public static int clueCountMain;    // global count for UI


    //private Sprite jhonSprite;
    public Sprite jhonSpriteStore;
    private Image jhon;

    private void OnDisable()
    {
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
        // Ensure currentClueCount array is initialized
        if (dialogManager != null)
        {
            if (dialogManager.currentClueCount == null || dialogManager.currentClueCount.Length != dialogManager.dialogLines.Length)
                dialogManager.currentClueCount = new int[dialogManager.dialogLines.Length];

            dialogManager.ResetClues(); // optional: resets UI
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
        //if (PlayerDialog.instance != null && PlayerDialog.instance.isInteraction) return;

        if ((type == InteractType.DogBed && !InteractedWithDogBed) ||
            (type == InteractType.NonInteractiveAutomatic && !isAutoComplete))
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
            if (!shouldWork) return;

            if (outRange != null) outRange.alpha = 0;
            if (ObjectPickHandler.Instance != null)
            {
                if (inRange != null)
                    inRange.alpha = (!isInteracted && !ObjectPickHandler.Instance.InteractionCheck()) ? 1 : 0;
            }
            if (DoAutoRun && type == InteractType.InteractiveAutomatic && !isAutoCompleteNearObject)
            {
                if (!isInteracted) StartInteraction();
                else if (isInteracted && Input.GetKeyDown(KeyCode.E)) NextDialogueImage();
                return;
            }

            ObjectHandler();
        }
        else if (playerInteract.GetObjectInteract() == null)
            Avoid();

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
        isInteracting = true;
        isInteracted = true;

        if (currentImageIndex >= dialogManager.dialogLines.Length)
            currentImageIndex = 0;

        // Load saved progress
        clueCount = dialogManager.currentClueCount[currentImageIndex];
        totalClues = dialogManager.totalCount[currentImageIndex];

        // ✅ clueCountMain should match progress, not total
        clueCountMain = clueCount;

        gettingClueCount?.AddTick(clueCount, totalClues);

        TypeLine();
    }


    public void NextDialogueImage()
    {
        isSecondDialog = true;
        // Save the clue count before hiding
        if (dialogManager.currentClueCount != null && currentImageIndex < dialogManager.currentClueCount.Length)
            dialogManager.currentClueCount[currentImageIndex] = clueCount;


            dialogContainer.SetActive(false);


        if (dialogManager.dialogAudio.Length > currentImageIndex &&
            dialogManager.dialogAudio[currentImageIndex]?.sorce != null)
            audioManager.Stop();

        currentImageIndex++;

        if (currentImageIndex < dialogManager.dialogLines.Length)
        {
           
            clueCount = dialogManager.currentClueCount[currentImageIndex];

            totalClues = dialogManager.totalCount[currentImageIndex];
            clueCountMain = totalClues;
            TypeLine();
        }
        else
        {
            isInteracting = false;
            isInteracted = false;
            gettingClueCount?.DisableAll();
            HandlePostDialogueActions();
        }
    }

   
    private void HandlePostDialogueActions()
    {
        if (type == InteractType.Tablet)
        {
            TabletUnlocker.instance.UnlockTablet();

            if (pickReferences.ObjectInteractBadge.type == InteractType.Badge)
                pickReferences.ObjectInteractBadge.shouldWork = true;

            if (pickReferences.ObjectPickHandlerCigarette.type == InspectType.Cigarette)
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
            playerInteract.player.GetComponent<PointAndMovement>().enabled = true;
        }

        if (type == InteractType.NonInteractiveAutomatic)
        {
            isAutoComplete = true;
            playerInteract.player.GetComponent<PointAndMovement>().enabled = true;
        }

        if (type == InteractType.Lighter)
            Destroy(gameObject, 0.1f);
    }

    private void Avoid()
    {
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
        // Font size
        dialogText.fontSize = (dialogManager.changeFontSize && dialogManager.frontSize != null && currentImageIndex < dialogManager.frontSize.Length)
            ? dialogManager.frontSize[currentImageIndex] : 45;

        // Text
        dialogText.SetText((dialogManager.dialogLines != null && currentImageIndex < dialogManager.dialogLines.Length)
            ? dialogManager.dialogLines[currentImageIndex] : "");

        // Audio
        if (dialogManager.dialogAudio != null &&
            currentImageIndex < dialogManager.dialogAudio.Length &&
            dialogManager.dialogAudio[currentImageIndex]?.sorce != null)
            audioManager.PlayDialogLine(dialogManager, currentImageIndex);

        // Clues
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

}
