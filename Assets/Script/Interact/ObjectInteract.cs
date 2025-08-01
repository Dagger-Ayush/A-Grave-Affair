﻿
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
    private ObjectPickHandler pickHandler;
    public bool isCigarette;
    [SerializeField] private bool isLighter = false;

    [SerializeField] private DialogAudio[] dialogAudio;

    [HideInInspector] public bool shouldWork = false;

    private bool InteractedWithDogBed = false;

    private void Start()
    {
        
        
        if (isCigarette)
        {
            pickHandler = GetComponent<ObjectPickHandler>();
            pickHandler.enabled = false;

        }
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

        
            if (currentImageIndex<dialogueImages.Length)
            {
            if (dialogueImages[currentImageIndex].tag == "Screen")
            {
                dialogueImages[currentImageIndex].transform.rotation = Quaternion.Euler(0, 44, 0);
                dialogueImages[currentImageIndex].transform.position = new Vector3(playerInteract.player.transform.position.x - 1.75f,
                                                                                  playerInteract.player.transform.position.y + 4.4f,
                                                                                  playerInteract.player.transform.position.z - 2);
            }
           
            }

        
        if (isDogBed && !InteractedWithDogBed)
        {
            if (!interacting)
            {
                StartInteraction();
            }
            else if (interacting && Input.GetKeyDown(KeyCode.E))
            {
                NextDialogueImage();
            }
        }

        if (isDogBed && !InteractedWithDogBed) return;

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
       
       
        if (dialogueImages.Length > 0)
        {
            if(dialogueImages[currentImageIndex].tag == "Screen")
            {
                dialogAudio[currentImageIndex].sorce.Play();
            }

            if (dialogueImages[currentImageIndex].GetComponent<GettingClueCount>() == null)
            {
                pickReferences.currentClueCount.gameObject.SetActive(false);
            }
            else
            {
                dialogueImages[currentImageIndex].GetComponent<GettingClueCount>().Checking();
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
            if (dialogueImages[currentImageIndex].GetComponent<GettingClueCount>() == null)
            {
                pickReferences.currentClueCount.gameObject.SetActive(false);
            }
            else
            {
                dialogueImages[currentImageIndex].GetComponent<GettingClueCount>().Checking();
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

                Collider[] colliderArray = Physics.OverlapSphere(transform.position, 100);

                foreach (Collider collider in colliderArray)
                {
                    if (collider.TryGetComponent(out ObjectInteract objectInteract))
                    {

                        objectInteract.shouldWork = true;
                    }
                    if (collider.TryGetComponent(out ObjectPickHandler objectPickHandler))
                    {
                       
                            objectPickHandler.shouldWork = true;



                        if (objectPickHandler.isLetter_1 || objectPickHandler.isLetter_2)
                        {
                            objectPickHandler.shouldWork = false;

                        }
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
                    if(playerInteract. tabletImage != null)
                    {
                        playerInteract.tabletImage.enabled = true;
                    }
                    
                    FindFirstObjectByType<ObjectInteract>().shouldWork = true;
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
