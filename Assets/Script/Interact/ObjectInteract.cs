
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
public class ObjectInteract : MonoBehaviour
{  
  
    [Header("Dialogues Interaction reference's")]
    [SerializeField] private CanvasGroup inRange;
    [SerializeField] private CanvasGroup outRange;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private ObjectPickReferences pickReferences;

    [Header("dialogue references")]
    [SerializeField] private GameObject[] dialogueImages;
    private int currentImageIndex = 0;

    public  bool interacting;
     public static bool isInteracted;
    private Vector2 turn;

   public bool isTablet;// the player need not need to press E to enable Dialog
    public bool isDogBed;
    public bool isBadge;
  
    [SerializeField] private bool isLighter = false;

    [SerializeField] private DialogAudio[] dialogAudio;

   public bool shouldWork = false;

    private bool InteractedWithDogBed = false;

    private void Start()
    {

        
        if (isTablet)
        {
            outRange.alpha = 1;
            shouldWork = true;
        }
        if (isDogBed)
        {
            enabled = false;
        }
    }
    private void Update()
    {
        if (isDogBed && !InteractedWithDogBed )
        {
            if (!interacting)
            {
                StartInteraction();
            }
            else if (interacting && Input.GetKeyDown(KeyCode.E))
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
            if (inRange != null )
            {
               if(!isInteracted && !ObjectPickHandler.isCollected)
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

        else if (playerInteract.GetObjectInteract() == null  )
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
                    if (!interacting)
                    {
                     if (isInteracted || ObjectPickHandler.isCollected) return;
                      StartInteraction();
                    }
                    else
                    {
                        NextDialogueImage();
                    }
                  
            
        }
        if (isTablet)
        {
            if (!interacting && currentImageIndex < dialogueImages.Length)
            {
                StartInteraction();
                gameObject.GetComponent<Renderer>().enabled = false;
            }
           
        }
       
    }
    public void StartInteraction()
    {
       
        interacting = true;
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
        if (dialogueImages.Length == 0 )
            return;
        
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

        if (currentImageIndex < dialogueImages.Length )
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
           

            interacting = false;
            isInteracted = false;
     
            if (isTablet)
            {
                if (pickReferences.ObjectInteractBadge.isBadge)
                {
                    pickReferences.ObjectInteractBadge.shouldWork = true;
                }
                if (pickReferences.ObjectPickHandlerCigarette.isCigarette)
                {
                   
                    pickReferences.ObjectPickHandlerCigarette.shouldWork = true;

                }
                
                Destroy(gameObject, 0.1f);// using delay for Movment and point and click to enable
            }
            if (isDogBed)
            {
                if (pickReferences.lighterObjectPickHandler!= null)
                {
                    pickReferences.lighterObjectPickHandler.enabled = true;
                }

                InteractedWithDogBed = true;
            }
            if (isLighter)
            {
                Destroy(gameObject, 0.1f);
            }
        }
    }

    private void Avoid()
    {   
       
        if (currentImageIndex < dialogueImages.Length)
        {
            //dialogueImages[currentImageIndex].SetActive(false);
            currentImageIndex = 0;

        }
       
        interacting = false;
        if (inRange != null && shouldWork && !isInteracted)
        {
            inRange.alpha = 0;
            outRange.alpha = 1;
        }
       
    }
    
    
    
}
