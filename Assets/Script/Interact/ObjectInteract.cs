using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using static ObjectPickHandler;

public class ObjectInteract : MonoBehaviour
{
    public enum InteractType { Tablet, DogBed, Badge, Lighter, None }
    public InteractType type = InteractType.None;

    [Header("Dialogues Interaction reference's")]
    [SerializeField] private CanvasGroup inRange;
    [SerializeField] private CanvasGroup outRange;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private ObjectPickReferences pickReferences;

    [Header("Dialogue references")]
    [SerializeField] private GameObject[] dialogueImages;
    private int currentImageIndex = 0;

    public static bool isInteracted;
    [HideInInspector] public bool isRunning;
    private Vector2 turn;

    [SerializeField] private DialogAudio[] dialogAudio;

    public bool shouldWork = false;
    private bool InteractedWithDogBed = false;

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
        if (type == InteractType.DogBed && !InteractedWithDogBed)
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
            if (isInteracted || ObjectPickHandler.isCollected)
            {
                inRange.alpha = 0;
            }
        }

        if (playerInteract.GetObjectInteract() == this)
        {
            if (!shouldWork) return;

            outRange.alpha = 0;

            if (inRange != null)
            {
                if (!isInteracted && !ObjectPickHandler.isCollected)
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
        if (TabletManager.Instance != null && pickReferences != null)
        {
            if (TabletManager.isTabletOpen || pickReferences.interactionTutorial.isRunning) return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isInteracted && !ObjectPickHandler.isCollected)
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
            if (!isInteracted && currentImageIndex < dialogueImages.Length)
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

        if (dialogueImages.Length > 0)
        {
            if ((dialogueImages[currentImageIndex].CompareTag("Screen") || dialogueImages[currentImageIndex].CompareTag("Sound"))
                && dialogAudio.Length > currentImageIndex
                && dialogAudio[currentImageIndex] != null
                && dialogAudio[currentImageIndex].sorce != null)
            {
                dialogAudio[currentImageIndex].sorce.Play();
            }

            if (pickReferences != null)
            {
                if (dialogueImages[currentImageIndex].GetComponent<GettingClueCount>() == null)
                {
                    pickReferences.currentClue.SetActive(false);
                }
                else
                {
                    dialogueImages[currentImageIndex].GetComponent<GettingClueCount>().Checking();
                }
            }

            dialogueImages[currentImageIndex].SetActive(true);
        }
    }

    public void NextDialogueImage()
    {
        if (dialogueImages.Length == 0) return;

        if (dialogueImages[currentImageIndex].GetComponent<GettingClueCount>())
        {
            dialogueImages[currentImageIndex].GetComponent<GettingClueCount>().storingData();
        }

        dialogueImages[currentImageIndex].SetActive(false);

        if ((dialogueImages[currentImageIndex].CompareTag("Screen") || dialogueImages[currentImageIndex].CompareTag("Sound"))
            && dialogAudio.Length > currentImageIndex
            && dialogAudio[currentImageIndex] != null
            && dialogAudio[currentImageIndex].sorce != null)
        {
            dialogAudio[currentImageIndex].sorce.Stop();
        }

        currentImageIndex++;

        if (currentImageIndex < dialogueImages.Length)
        {
            if ((dialogueImages[currentImageIndex].CompareTag("Screen") || dialogueImages[currentImageIndex].CompareTag("Sound"))
                && dialogAudio.Length > currentImageIndex
                && dialogAudio[currentImageIndex] != null
                && dialogAudio[currentImageIndex].sorce != null)
            {
                dialogAudio[currentImageIndex].sorce.Play();
            }

            if (pickReferences != null)
            {
                if (dialogueImages[currentImageIndex].GetComponent<GettingClueCount>() == null)
                {
                    pickReferences.currentClue.SetActive(false);
                }
                else
                {
                    dialogueImages[currentImageIndex].GetComponent<GettingClueCount>().Checking();
                }
            }

            dialogueImages[currentImageIndex].SetActive(true);
        }
        else
        {
            isRunning = false;
            isInteracted = false;

            if (type == InteractType.Tablet)
            {
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

            if (type == InteractType.Lighter)
            {
                Destroy(gameObject, 0.1f);
            }
        }
    }

    private void Avoid()
    {
        if (currentImageIndex < dialogueImages.Length)
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
}
