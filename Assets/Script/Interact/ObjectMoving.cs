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

    [SerializeField] private CanvasGroup inRange;
    [SerializeField] private CanvasGroup outRange;
    [SerializeField] private PlayerInteract playerInteract;
    private ObjectInteract objectInteract;

    [SerializeField] private GameObject foodBowlEmpty, foodBowlFilled;
    [SerializeField] private GameObject dialogueImages;

    private bool isCompleted = false;
    [HideInInspector]public static bool canInteract = false;
    private static bool canInteractWithBed = false;

   
    [SerializeField] private DialogAudio dialogAudio;

    [HideInInspector] public bool shouldWork = false;


    [SerializeField] private Animation anim;
    [SerializeField] private Transform Dog;
    [SerializeField] private Image fadeImage;

    private void Start()
    {
         objectInteract = GetComponent<ObjectInteract>();
    }
    void Update()
    {
        if (!shouldWork) return;
        if (playerInteract.ObjectMovingHandler() == this && !isCompleted)
        {
            
            outRange.alpha = 0;
            if (inRange != null )
            {
               inRange.alpha = 1; 
            }
            if (Input.GetKeyDown(KeyCode.E) )
            {
               
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
        else if(playerInteract.ObjectMovingHandler() == null && !isCompleted && shouldWork)
        {
            if (inRange != null)
            {
                outRange.alpha = 1;
                inRange.alpha = 0;
            }

        }
        if (inRange != null && isCompleted)
        {
            outRange.alpha = 0;
            inRange.alpha = 0;
        }
    }
    void DogBedMoving()
    {

       

        objectInteract.enabled = true;

      

        float objectMove = 2;

        float z = transform.position.z ;
        
        transform.position = new Vector3(transform.position.x, transform.position.y, z -= objectMove);

        isCompleted = true;


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
    public IEnumerator FadeInAndOut()
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
