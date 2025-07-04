using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

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

    [HideInInspector] public bool shouldWork = false;


    [SerializeField] private Animation anim;
    [SerializeField] private Transform Dog;
    [SerializeField] private Image fadeImage;

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

        StartCoroutine(FadeInAndOut());
        

        canInteractWithBed = true;
        canInteract = false;

    }
    IEnumerator FadeInAndOut()
    {
       
        float time = 0f;
        bool fade = false;
        Color color = fadeImage.color;

        while (true)
        {
            if (!fade)
            {
                fadeImage.gameObject.SetActive(true);
                time += Time.deltaTime ;
                if (time >= 1.25f)
                {
                    Dog.transform.localPosition = Vector3.zero;
                    
                    time = 1f;
                    fade = true;
                }
            }
            else
            {
                time -= Time.deltaTime ;
                if (time <= 0f)
                {
                    fadeImage.gameObject.SetActive(false);
                    anim.Play("Scene");
                    time = 0f;
                    break; // End after fade out
                }
            }

            color.a = time;
            fadeImage.color = color;
            yield return null;
        }
    }
}
