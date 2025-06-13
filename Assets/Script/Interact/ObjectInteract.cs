
using System.Runtime.CompilerServices;
using UnityEngine;
public class ObjectInteract : MonoBehaviour
{  
  
    [Header("Dialogues Interaction reference's")]
    [SerializeField] private CanvasGroup objectCanvasGroup;
    [SerializeField] private PlayerInteract playerInteract;
  
    [SerializeField] private Transform player;

    [Header("dialogue references")]
    [SerializeField] private GameObject[] dialogueImages;
    private int currentImageIndex = 0;

    [HideInInspector]public bool interacting;

    private Vector2 turn;

   [SerializeField] private bool isTablet;// the player need not need to press E to enable Dialog
    private void Update()
    {
       
        if (playerInteract.GetObjectInteract() == this)
        {  
            if (objectCanvasGroup != null)
            {
                objectCanvasGroup.alpha = interacting ? 0 : 1;
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
    private void StartInteraction()
    {
        interacting = true;
        currentImageIndex = 0;

        if (dialogueImages.Length > 0)
        {
            dialogueImages[currentImageIndex].SetActive(true);
            
           
        }
    }

    private void NextDialogueImage()
    {
        if (dialogueImages.Length == 0 )
            return;
    
        dialogueImages[currentImageIndex].SetActive(false);
        currentImageIndex++;

        if (currentImageIndex < dialogueImages.Length)
        {
           
            dialogueImages[currentImageIndex].SetActive(true);
        }
        else
        {
            
            interacting = false;
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
