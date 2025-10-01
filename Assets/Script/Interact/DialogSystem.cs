using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    [Header("dialogue references")]
    [Header("Dialog System")]
    public DialogManager dialogManager;
    public AudioManager audioManager;
    public TextMeshProUGUI dialogText;
    public GameObject dialogContainer;

    private int currentImageIndex = 0;

    [HideInInspector] public bool interacting;
    public bool isDogBed;
    [SerializeField] private ObjectPickHandler objectPickHandler;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private Transform player;
    private ObjectInteract objectInteract;
   
    private void Start()
    {
       
        objectInteract = GetComponent<ObjectInteract>();
        if (isDogBed)
        {
            enabled = false;
        }
    }
    private void Update()
    {
       
        if (isDogBed )
        {
            if (!interacting)
            {
                StartInteraction();
            }
            else if( interacting && Input.GetKeyDown(KeyCode.E))
            {
                NextDialogueImage();
            }
           
        }
        
    }
    public void StartInteraction()
    {
        playerInteract.isPointAndMovementEnabled = true;
        interacting = true;
        currentImageIndex = 0;
       

        if (dialogManager.dialogLines.Length > 0)
        {
            TypeLine(currentImageIndex);
        }
    }

    public void NextDialogueImage()
    {
        if (dialogManager.dialogLines.Length == 0)
            return;

        dialogContainer.SetActive(false);
        //dialogAudio[currentImageIndex].sorce.Stop();
        currentImageIndex++;

        if (currentImageIndex < dialogManager.dialogLines.Length)
        {
            TypeLine(currentImageIndex);
        }
        else
        {
            interacting = false;
            playerInteract.isPointAndMovementEnabled = false;

            if (isDogBed)
            {
                objectInteract.enabled = true;
                objectPickHandler.enabled = true;
                enabled = false;
              
            }
        }
    }
    void TypeLine(int currentImageIndex)
    {
        dialogContainer.SetActive(true);
        dialogText.fontSize = dialogManager.frontSize[currentImageIndex];
        dialogText.SetText(dialogManager.dialogLines[currentImageIndex]);

        if (dialogManager.dialogAudio.Length > currentImageIndex
                && dialogManager.dialogAudio[currentImageIndex] != null
                && dialogManager.dialogAudio[currentImageIndex].sorce != null)
        {
            audioManager.PlayDialogLine(dialogManager, currentImageIndex);
        }
    }
}
