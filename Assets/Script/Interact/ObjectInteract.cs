using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static ObjectPickHandler;

public class ObjectInteract : MonoBehaviour
{
    public static ObjectInteract Instance;
    public enum InteractType { Tablet, DogBed, Badge, Lighter,InteractiveAutomatic, NonInteractiveAutomatic, None }
    public InteractType type = InteractType.None;

    [Header("Dialog System")]
    public DialogManager dialogManager;
    public AudioManager audioManager;
    public TextMeshProUGUI dialogText;
    public GameObject dialogContainer;
    

    public static bool isInteracted;

    [Header("Dialogues Interaction reference's")]
    [SerializeField] private CanvasGroup inRange;
    [SerializeField] private CanvasGroup outRange;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private ObjectPickReferences pickReferences;

    [Header("Dialogue references")]
    //[SerializeField] private GameObject[] dialogueImages;
    private int currentImageIndex = 0;

   
    [HideInInspector] public bool isRunning;
    private Vector2 turn;

    //[SerializeField] private DialogAudio[] dialogAudio;

    public bool shouldWork = false;
    private bool InteractedWithDogBed = false;
    [HideInInspector] public bool isAutoComplete = false;
    [HideInInspector] public bool isAutoCompleteNearObject = false;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        if (type == InteractType.Tablet)
        {
            outRange.alpha = 1;
            shouldWork = true;
        }

        if (type == InteractType.DogBed)
        {
            enabled = false;
        }
    }

    private void Update()
    {
       
        if ((type == InteractType.DogBed && !InteractedWithDogBed) || (type == InteractType.NonInteractiveAutomatic && !isAutoComplete))
        {
            if (!isInteracted)
            {
                StartInteraction();
            }
            else if (isInteracted && Input.GetKeyDown(KeyCode.E))
            {
                NextDialogueImage();
            }
            return;
        }

        if (inRange != null)
        {
            if (isInteracted || ObjectPickHandler.Instance.InteractionCheck())
            {
                inRange.alpha = 0;
            }
        }

        if (playerInteract.GetObjectInteract() == this)
        {
            if (!shouldWork) return;

            if (type == InteractType.InteractiveAutomatic && !isAutoCompleteNearObject)
            {
                if (!isInteracted)
                {
                    StartInteraction();
                }
                else if (isInteracted && Input.GetKeyDown(KeyCode.E))
                {
                    NextDialogueImage();
                }
                return;
            }
            if (outRange != null)
            {
                outRange.alpha = 0;
            }
            if (inRange != null)
            {
                if (!isInteracted && !ObjectPickHandler.Instance.InteractionCheck())
                {
                    inRange.alpha = 1;
                }
                else
                {
                    inRange.alpha = 0;
                }
            }
           
                ObjectHandler();
        }
        else if (playerInteract.GetObjectInteract() == null)
        {
            Avoid();
        }
    }

    private void ObjectHandler()
    {
        if (TabletManager.Instance != null && pickReferences != null && pickReferences.interactionTutorial!= null)
        {
            if (TabletManager.isTabletOpen || pickReferences.interactionTutorial.isRunning) return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isInteracted && !ObjectPickHandler.Instance.InteractionCheck())
            {
                StartInteraction();
            }
            else
            {
                NextDialogueImage();
            }
        }

        if (type == InteractType.Tablet)
        {
            if (!isInteracted && currentImageIndex < dialogManager.dialogLines.Length)
            {
                StartInteraction();
                gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }

    public void StartInteraction()
    {
        isRunning = true;
        isInteracted = true;
        currentImageIndex = 0;

        if (dialogManager.dialogLines.Length > 0)
        {
            TypeLine();
            
            /*
            if (pickReferences != null)
            {
                if (dialogManager.dialogueImages[currentImageIndex].GetComponent<GettingClueCount>() == null)
                {
                    pickReferences.currentClue.SetActive(false);
                }
                else
                {
                    dialogManager.dialogLines[currentImageIndex].GetComponent<GettingClueCount>().Checking();
                }
            }
            */
        }
    }

    public void NextDialogueImage()
    {
        if (dialogManager.dialogLines.Length == 0) return;
        /*
        if (dialogManager.dialogueImages[currentImageIndex].GetComponent<GettingClueCount>())
        {
            dialogueImages[currentImageIndex].GetComponent<GettingClueCount>().storingData();
        }
        */
        dialogContainer.SetActive(false);

        if (//(dialogManager.dialogueImages[currentImageIndex].CompareTag("Screen") || dialogueImages[currentImageIndex].CompareTag("Sound"))
             dialogManager.dialogAudio.Length > currentImageIndex
            && dialogManager.dialogAudio[currentImageIndex] != null
            && dialogManager.dialogAudio[currentImageIndex].sorce != null)
        {
            audioManager.Stop();
        }

        currentImageIndex++;

        if (currentImageIndex < dialogManager.dialogLines.Length)
        {
            
            TypeLine();
            /*
            if (pickReferences != null)
            {
                if (dialogManager.dialogueImages[currentImageIndex].GetComponent<GettingClueCount>() == null)
                {
                    pickReferences.currentClue.SetActive(false);
                }
                else
                {
                    dialogManager.dialogueImages[currentImageIndex].GetComponent<GettingClueCount>().Checking();
                }
            }
            */

            
        }
        else
        {
            isRunning = false;
            isInteracted = false;

            if (type == InteractType.Tablet)
            {
                TabletUnlocker.instance.UnlockTablet();
                if (pickReferences.ObjectInteractBadge.type == InteractType.Badge)
                {
                    pickReferences.ObjectInteractBadge.shouldWork = true;
                }

                if (pickReferences.ObjectPickHandlerCigarette.type == InspectType.Cigarette)
                {
                    pickReferences.ObjectPickHandlerCigarette.shouldWork = true;
                }

                Destroy(gameObject, 0.1f); // small delay for movement and point-and-click to enable
            }

            if (type == InteractType.DogBed)
            {
                if (pickReferences.lighterObjectPickHandler != null)
                {
                    pickReferences.lighterObjectPickHandler.enabled = true;
                }

                InteractedWithDogBed = true;
            }
            if (type == InteractType.InteractiveAutomatic)
            {
                isAutoCompleteNearObject = true;
                playerInteract.player.GetComponent<PointAndMovement>().enabled = true;
                
            } 
            if ( type == InteractType.NonInteractiveAutomatic)
            {
                isAutoComplete = true;
                playerInteract.player.GetComponent<PointAndMovement>().enabled = true;

            }

            if (type == InteractType.Lighter)
            {
                Destroy(gameObject, 0.1f);
            }
        }
    }

    private void Avoid()
    {
        if (currentImageIndex < dialogManager.dialogLines.Length)
        {
            currentImageIndex = 0;
        }

        isRunning = false;
        isInteracted = false;

        if (inRange != null && shouldWork && !isInteracted)
        {
            inRange.alpha = 0;
            outRange.alpha = 1;
        }
    }
    public bool InteractionCheck()
    {
        if (!isInteracted) return false;
        else return true;
    }
    void TypeLine()
    {
        dialogContainer.SetActive(true);
        if (dialogManager.changeFontSize)
        {
            dialogText.fontSize = dialogManager.frontSize[currentImageIndex];
        }
        else
        {
            dialogText.fontSize = 45;
        }
        dialogText.SetText(dialogManager.dialogLines[currentImageIndex]);

        if (dialogManager.dialogAudio.Length > currentImageIndex
                && dialogManager.dialogAudio[currentImageIndex] != null
                && dialogManager.dialogAudio[currentImageIndex].sorce != null)
        {
            audioManager.PlayDialogLine(dialogManager, currentImageIndex);
        }
    }
   

}
