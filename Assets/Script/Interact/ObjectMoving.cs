using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectMoving : MonoBehaviour
{  
    public enum ObjectType{ DogBed, DogBowl};
   [SerializeField] private ObjectType objectType;

    [SerializeField] private CanvasGroup objectCanvasGroup;
    [SerializeField] private PlayerInteract playerInteract;

    
    [SerializeField] private GameObject foodBowlEmpty, foodBowlFilled;
    [SerializeField] private GameObject dialogueImages;

    private bool isCompleted = false;
    [HideInInspector]public bool canInteract = false;
    private static bool canInteractWithBed = false;

 
    [SerializeField] private DialogAudio dialogAudio;

    public bool shouldWork = false;
  
    void Update()
    {
        if (playerInteract.ObjectMoving() == this && !isCompleted)
        {
            if(!shouldWork)return;
            if (objectCanvasGroup != null )
            {
               objectCanvasGroup.alpha = 1; 
            }
            if (Input.GetKeyDown(KeyCode.E) )
            {
                if (isCompleted) return;
                switch (objectType)
                {
                    case ObjectType.DogBed:
                        if (canInteractWithBed)
                        {
                            DogBedMoving();
                        }
                        break;

                     case ObjectType.DogBowl:
                        if (!canInteract)
                        {
                            dialogAudio.sorce.Play();
                            dialogueImages.SetActive(true);
                            dialogueImages.transform.rotation = Quaternion.Euler(0, 44, 0);
                            dialogueImages.transform.position = new Vector3(playerInteract.player.transform.position.x - 1.75f,
                                                                            playerInteract.player.transform.position.y + 4.4f,
                                                                            playerInteract.player.transform.position.z - 2);
                            canInteract = true;
                        }
                        else
                        {
                            dialogueImages.SetActive(false);
                            DogBowlFilling();
                        }
                       
                        break;
                }
                
            }
            
        }
        else
        {
            if (objectCanvasGroup != null)
            {
                objectCanvasGroup.alpha = 0;
            }

        }
        if (objectCanvasGroup != null && isCompleted)
        {
            objectCanvasGroup.alpha = 0;
        }
    }
    void DogBedMoving()
    {

        DialogSystem dialogSystem = GetComponent<DialogSystem>();

        dialogSystem.enabled = true;

        isCompleted = true;

        float objectMove = 1;

        float z = transform.position.z ;
        
        transform.position = new Vector3(transform.position.x, transform.position.y, z -= objectMove);

      
    }
    void DogBowlFilling()
    {
        isCompleted = true;

        foodBowlEmpty.SetActive(false);
        foodBowlFilled.SetActive(true);
        canInteractWithBed = true;
        canInteract = false;

    }
}
