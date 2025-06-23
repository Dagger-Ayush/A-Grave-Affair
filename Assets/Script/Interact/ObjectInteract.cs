
using System.Runtime.CompilerServices;
using UnityEngine;
public class ObjectInteract : MonoBehaviour
{  
  
    [Header("Dialogues Interaction reference's")]
    [SerializeField] private CanvasGroup objectCanvasGroup;
    [SerializeField] private PlayerInteract playerInteract;


    [Header("dialogue references")]
    [SerializeField] private GameObject[] dialogueImages;
    private int currentImageIndex = 0;

    [HideInInspector]public bool interacting;

    private Vector2 turn;

   [SerializeField] private bool isTablet;// the player need not need to press E to enable Dialog

    private ObjectPickHandler pickHandler;
    public bool isCigarette;
   

    private void Start()
    {
        if (isCigarette)
        {
            pickHandler = GetComponent<ObjectPickHandler>();
            pickHandler.enabled = false;
        }
    }
    private void Update()
    {
       
        if (playerInteract.GetObjectInteract() == this)
        {
           
            if (objectCanvasGroup != null )
            {
                objectCanvasGroup.alpha = interacting ? 0 : 1;
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
        if (Input.GetKeyDown(KeyCode.E))
        {
                    if (!interacting)
                    {
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
        currentImageIndex = 0;
       
            playerInteract.player.transform.LookAt(transform.position);
        
       
        if (dialogueImages.Length > 0)
        {
           
            dialogueImages[currentImageIndex].SetActive(true);
        }
    }

    public void NextDialogueImage()
    {
        if (dialogueImages.Length == 0 )
            return;
    
        dialogueImages[currentImageIndex].SetActive(false);
        currentImageIndex++;

        if (currentImageIndex < dialogueImages.Length )
        {
            dialogueImages[currentImageIndex].SetActive(true);
        }
        else
        {
            interacting = false;

           
            if (isCigarette)
                {
                    pickHandler.enabled = true;
                    enabled = false;
                }

            if (isTablet)
            {
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
        if (objectCanvasGroup != null)
        {
            objectCanvasGroup.alpha = 0;
        }
       
    }
    
    
    
}
