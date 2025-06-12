
using UnityEngine;
public class ObjectInteract : MonoBehaviour
{  
  
    [Header("Dialogues Interaction reference's")]
    [SerializeField] private CanvasGroup objectCanvasGroup;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private PointAndMovement pointAndMovement;
    [SerializeField] private Transform player;

    [Header("dialogue references")]
    [SerializeField] private GameObject[] dialogueImages;
    private int currentImageIndex = 0;

    private bool interacting;

    private Vector2 turn;

    private void Update()
    {
        if (playerInteract.GetObjectInteract() == this)
        {  
                objectCanvasGroup.alpha = 1;
                ObjectHandler();
        }

        else if (playerInteract.GetObjectInteract() == null  )
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
        if (interacting)
        {
            pointAndMovement.enabled = false;
            dialogueImages[currentImageIndex].transform.rotation = Quaternion.Euler(0, 44, 0);
            dialogueImages[currentImageIndex].transform.position = new Vector3(player.position.x,player.position.y + 3,player.position.z);
        }
        else
        {
            pointAndMovement.enabled = true;
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
        objectCanvasGroup.alpha = 0;
    }

    
    
}
