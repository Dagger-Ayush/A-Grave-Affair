
using System.Runtime.CompilerServices;
using UnityEngine;
public class ObjectInteract : MonoBehaviour
{  
  
    [Header("Dialogues Interaction reference's")]
    [SerializeField] private CanvasGroup inRange;
    [SerializeField] private CanvasGroup outRange;
    [SerializeField] private PlayerInteract playerInteract;


    [Header("dialogue references")]
    [SerializeField] private GameObject[] dialogueImages;
    private int currentImageIndex = 0;

    [HideInInspector]public  bool interacting;
     public static bool isInteracted;
    private Vector2 turn;

   [SerializeField] private bool isTablet;// the player need not need to press E to enable Dialog

    private ObjectPickHandler pickHandler;
    public bool isCigarette;


    [SerializeField] private DialogAudio[] dialogAudio;

    [HideInInspector] public bool shouldWork = false;
    private void Start()
    {
        
        
        if (isCigarette)
        {
            pickHandler = GetComponent<ObjectPickHandler>();
            pickHandler.enabled = false;

        }
        if (isTablet)
        {
            shouldWork = true;
        }
    }
    private void Update()
    {
        
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

        else if (playerInteract.GetObjectInteract() == null)
        {
            Avoid();
        }
       
        if (interacting == true)
        {
            if (dialogueImages[currentImageIndex].tag == "Screen")
            {
                dialogueImages[currentImageIndex].transform.rotation = Quaternion.Euler(0, 44, 0);
                dialogueImages[currentImageIndex].transform.position = new Vector3(playerInteract.player.transform.position.x - 1.75f,
                                                                                  playerInteract.player.transform.position.y + 4.4f,
                                                                                  playerInteract.player.transform.position.z - 2);
            }
         
        }
    }
    private void ObjectHandler()
    {
        if (TabletManager.isTabletOpen) return;

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
       
        playerInteract.player.transform.LookAt(transform.position);
        
       
        if (dialogueImages.Length > 0)
        {
            if(dialogueImages[currentImageIndex].tag == "Screen")
            {
                dialogAudio[currentImageIndex].sorce.Play();
            }
           
            dialogueImages[currentImageIndex].SetActive(true);
        }
       
    }

    public void NextDialogueImage()
    {
        if (dialogueImages.Length == 0 )
            return;
    
        dialogueImages[currentImageIndex].SetActive(false);
        if (dialogueImages[currentImageIndex].tag == "Screen" || dialogueImages[currentImageIndex].tag == "Sound")
        {
            dialogAudio[currentImageIndex].sorce.Stop();
        }
        currentImageIndex++;

        if (currentImageIndex < dialogueImages.Length )
        {
            if (dialogueImages[currentImageIndex].tag == "Screen" || dialogueImages[currentImageIndex].tag == "Sound")
            {
                dialogAudio[currentImageIndex].sorce.Play();
            }
            dialogueImages[currentImageIndex].SetActive(true);
        }
        else
        {
           
            interacting = false;
            isInteracted = false;
           
            if (isCigarette)
            {
                FindAnyObjectByType<ObjectInteract>().shouldWork = true;
                FindAnyObjectByType<ObjectPickHandler>().shouldWork = true;
                FindAnyObjectByType<ObjectMoving>().shouldWork = true;
                Collider[] colliderArray = Physics.OverlapSphere(transform.position, 45);

                foreach (Collider collider in colliderArray)
                {
                    if (collider.TryGetComponent(out ObjectInteract objectInteract))
                    {

                        objectInteract.shouldWork = true;
                    }
                    if (collider.TryGetComponent(out ObjectPickHandler objectPickHandler))
                    {

                        objectPickHandler.shouldWork = true;
                    }
                    if (collider.TryGetComponent(out ObjectMoving objectMoving))
                    {
                        objectMoving.shouldWork = true;

                    }

                }
                pickHandler.enabled = true;

                enabled = false;
                }

            if (isTablet)
            {
                if (FindFirstObjectByType<ObjectInteract>().isCigarette)
                {
                    FindFirstObjectByType<ObjectInteract>().shouldWork = true;
                }
                Destroy(gameObject, 0.1f);// using delay for Movment and point and click to enable
            }
           
        }
    }

    private void Avoid()
    {   

        if (currentImageIndex < dialogueImages.Length)
        {
            dialogueImages[currentImageIndex].SetActive(false);
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
