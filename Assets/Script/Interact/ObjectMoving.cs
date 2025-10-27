using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMoving : MonoBehaviour
{  
    public static ObjectMoving instance;
    public enum ObjectType{ DogBed, DogBowl};
   [SerializeField] private ObjectType objectType;

    [SerializeField] private CanvasGroup inRange;
    [SerializeField] private CanvasGroup outRange;
    [SerializeField] private PlayerInteract playerInteract;
    private ObjectInteract objectInteract;

    [SerializeField] private GameObject foodBowlEmpty, foodBowlFilled;
    public DialogManager dialogManager;
    public AudioManager audioManager;
    public TextMeshProUGUI dialogText;
    public GameObject dialogContainer;

    private bool isCompleted = false;
    [HideInInspector]public static bool canInteract = false;
    private static bool canInteractWithBed = false;


    [HideInInspector] public bool shouldWork = false;


    [SerializeField] private Animation anim;
    [SerializeField] private Transform Dog;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject bed_dummy;
    [SerializeField] private GameObject bed_Main;


    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private DialogAudio dogBark;

    private void Awake()
    {
        instance = this;
    }
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
                        else
                        {
                            StartCoroutine(TextEnabler(infoText));
                            audioManager.PlayDialogBigLine(dogBark);
                            infoText.text = "Can't Interact, if only I could get Fred to move";
                        }
                        break;

                     case ObjectType.DogBowl:
                        if (!canInteract)
                        {
                            TypeLine(0);
                             canInteract = true;
                        }
                        else
                        {
                            dialogContainer.SetActive(false);
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
        float objectMove = 1.15f;
        float z = transform.position.z ;  
        transform.position = new Vector3(transform.position.x, transform.position.y, z -= objectMove);
        isCompleted = true;
        enabled = false;
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

    private IEnumerator TextEnabler(TextMeshProUGUI text)
    {
        text.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        text.gameObject.SetActive(false);
    }
    void TypeLine(int currentImageIndex)
    {
        dialogContainer.SetActive(true);
        dialogText.SetText(dialogManager.dialogLines[currentImageIndex]);

        if (dialogManager.dialogAudio.Length > currentImageIndex
                && dialogManager.dialogAudio[currentImageIndex] != null
                && dialogManager.dialogAudio[currentImageIndex].sorce != null)
        {
            audioManager.PlayDialogLine(dialogManager, currentImageIndex);
        }
    }
}
