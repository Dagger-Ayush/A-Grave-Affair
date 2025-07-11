using System.Collections;
using Unity.Cinemachine.Samples;
using UnityEngine;
using UnityEngine.AI;

public class PlayerDialog : MonoBehaviour
{
    private PointAndMovement pointAndMovement;
    [SerializeField] private GameObject[] dialogueImages;
    [SerializeField] private DialogAudio[] dialogAudio;
    private int currentImageIndex = 0;
    [HideInInspector] public bool isInteraction;
    private bool isEndDialogRunning;
    private PlayerInteract playerInteract;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInteract = GetComponent<PlayerInteract>();
        pointAndMovement = GetComponent<PointAndMovement>();
        StartCoroutine(StartInteraction());
    }

    // Update is called once per frame
    void Update()
    {
        if (PuzzleValidator.CorrectFilledCount >= 2)
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
            dialogueImages[currentImageIndex].transform.rotation = Quaternion.Euler(0, 44, 0);
            dialogueImages[currentImageIndex].transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);

        }
        if (pointAndMovement.isMoving)
        {
            dialogueImages[currentImageIndex].SetActive(false);
            isInteraction = false;
        }

        

    }
    private IEnumerator StartInteraction()
    {

        yield return new WaitForSeconds(4.18f);
        isInteraction = true;
        currentImageIndex = 0;

        if (dialogueImages.Length > 0)
        {
            dialogAudio[currentImageIndex].sorce.Play();
            dialogueImages[currentImageIndex].SetActive(true);
        }
    }

    private void NextDialogueImage()
    {
        if (dialogueImages.Length == 0)
            return;
        if (currentImageIndex < 2)
        {
            dialogAudio[currentImageIndex].sorce.Stop();
        }

        dialogueImages[currentImageIndex].SetActive(false);
        currentImageIndex++;

        if (currentImageIndex < dialogueImages.Length)
        {
            if (currentImageIndex < 2)
            {
                dialogAudio[currentImageIndex].sorce.Play();
            }
            dialogueImages[currentImageIndex].SetActive(true);
        }
        else
        {
            isInteraction = false;
        }
        if (currentImageIndex > 1)
        {
            pointAndMovement.enabled = true;

        }


    }

    public void EndDialog()
    {
        if (TabletManager.isTabletOpen ) return;

       

        if (!isInteraction && !isEndDialogRunning)
        {
            isInteraction = true;
            isEndDialogRunning = true;
            playerInteract.enabled = false;
            pointAndMovement.enabled = false;

            dialogAudio[2].sorce.Play();
            dialogueImages[3].SetActive(true);

        }
        else if (isInteraction && Input.GetKeyDown(KeyCode.E))
        {
            
               
                dialogueImages[3].SetActive(false);
                dialogAudio[2].sorce.Stop();

                pointAndMovement.enabled = true;
            playerInteract.enabled = true;


            isInteraction = false;
        }
    }
}