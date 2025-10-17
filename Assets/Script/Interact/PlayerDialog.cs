using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerDialog : MonoBehaviour
{
    public static PlayerDialog instance;
    private PointAndMovement pointAndMovement;
    
    private int currentImageIndex = 0;
    [HideInInspector] public bool isInteraction;
    private bool isEndDialogRunning;
    private PlayerInteract playerInteract;
    [SerializeField] private Collider bedCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

     [Header("Dialog System")]
    public DialogManager dialogManager;
    public AudioManager audioManager;
    public TextMeshProUGUI dialogText;
    public GameObject dialogContainer;
    public ObjectInteract LastDialog;
    public GameObject movementTutorial;
    private bool HasRun = false;

    [SerializeField] private DialogAudio lastDialogAudio;

    public PuzzleProgression progression;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        playerInteract = GetComponent<PlayerInteract>();
        pointAndMovement = GetComponent<PointAndMovement>();
        StartCoroutine(StartInteraction());
    }

    // Update is called once per frame
    void Update()
    {
        if (progression.puzzle1and2Check())
        {
            EndDialog();
        }
        if (isEndDialogRunning)return;

        if (Input.GetKeyDown(KeyCode.E) && currentImageIndex <= 1)
        {
            if (isInteraction)
            {
                NextDialogueImage();
            }
        }

        if (currentImageIndex <= 1)
        {
            pointAndMovement.enabled = false;
            /*
            dialogueImages[currentImageIndex].transform.rotation = Quaternion.Euler(0, 44, 0);
            dialogueImages[currentImageIndex].transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
            */
        }
        if (pointAndMovement.isMoving && !HasRun)
        {
            isInteraction = false;
            movementTutorial.SetActive(false);
            HasRun = true;
        }

        

    }
    private IEnumerator StartInteraction()
    {

        yield return new WaitForSeconds(4.18f);

        if (bedCollider != null)
        {
            bedCollider.enabled = true;
        }
        isInteraction = true;
        currentImageIndex = 0;

        if (dialogManager.dialogLines.Length > 0)
        {
            TypeLine(dialogContainer,currentImageIndex, currentImageIndex);
        }
    }

    private void NextDialogueImage()
    {
        if (dialogManager.dialogLines.Length == 0)
            return;
        if (currentImageIndex < 2)
        {
            audioManager.Stop();
        }
       
        dialogContainer.SetActive(false);
        currentImageIndex++;
        if (currentImageIndex == 2)
        {
            movementTutorial.SetActive(true);
            pointAndMovement.enabled = true;
            return;
        }
        if (currentImageIndex < dialogManager.dialogLines.Length)
        {
           
            TypeLine(dialogContainer,currentImageIndex, currentImageIndex);
        }
        else
        {
            isInteraction = false;
        }
        
    }

    public void EndDialog()
    {
        if (TabletManager.isTabletOpen) return;

        if (!isInteraction && !isEndDialogRunning)
        {
            isEndDialogRunning = true;
          
            LastDialog.enabled = true;
            audioManager.PlayDialogBigLine(lastDialogAudio);
        }
       
    }
    void TypeLine(GameObject gameObject,int currentImageIndex, int audioIndex)
    {
        gameObject.SetActive(true);
        dialogText.SetText(dialogManager.dialogLines[currentImageIndex]);

        if (dialogManager.dialogAudio.Length > currentImageIndex
                && dialogManager.dialogAudio[currentImageIndex] != null
                && dialogManager.dialogAudio[currentImageIndex].sorce != null)
        {
            if (currentImageIndex < 2)
            {
                audioManager.PlayDialogLine(dialogManager, audioIndex);
            }
            
        }
    }
}