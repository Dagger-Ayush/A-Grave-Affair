using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    [Header("dialogue references")]
    [SerializeField] private GameObject[] dialogueImages;
    private int currentImageIndex = 0;

    [HideInInspector] public bool interacting;
    public bool isDogBed;
    [SerializeField] private ObjectPickHandler objectPickHandler;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private Transform player;
    [SerializeField] private DialogAudio[] dialogAudio;
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
       

        if (dialogueImages.Length > 0)
        {
            
            dialogueImages[currentImageIndex].SetActive(true);
            dialogAudio[currentImageIndex].sorce.Play();
        }
    }

    public void NextDialogueImage()
    {
        if (dialogueImages.Length == 0)
            return;

        dialogueImages[currentImageIndex].SetActive(false);
        dialogAudio[currentImageIndex].sorce.Stop();
        currentImageIndex++;

        if (currentImageIndex < dialogueImages.Length)
        {
            dialogAudio[currentImageIndex].sorce.Play();
            dialogueImages[currentImageIndex].SetActive(true);
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
}
