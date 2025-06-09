using Unity.Cinemachine.Samples;
using UnityEngine;

public class PlayerDialog : MonoBehaviour
{
     private PointAndMovement pointAndMovement;
    [SerializeField] private GameObject[] dialogueImages;
    private int currentImageIndex = 0;
    [HideInInspector]public bool isInteraction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointAndMovement = GetComponent<PointAndMovement>();
        StartInteraction();
    }

    // Update is called once per frame
    void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.E) && currentImageIndex <= 1)
        {
            if (isInteraction)
            {
                NextDialogueImage();
            }
        }

        if (currentImageIndex <= 1)
        {
            dialogueImages[currentImageIndex].transform.rotation = Quaternion.Euler(0, 44, 0);
            dialogueImages[currentImageIndex].transform.position = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
            pointAndMovement.enabled = false;
        }
        if (pointAndMovement.isMoving)
        {
            dialogueImages[currentImageIndex].SetActive(false);
            isInteraction = false;
        }

    }
    private void StartInteraction()
    {   
        isInteraction = true;
        currentImageIndex = 0;

        if (dialogueImages.Length > 0)
        {
            dialogueImages[currentImageIndex].SetActive(true);
        }
    }

    private void NextDialogueImage()
    {
        if (dialogueImages.Length == 0)
            return;

        dialogueImages[currentImageIndex].SetActive(false);
        currentImageIndex++;

        if (currentImageIndex < dialogueImages.Length)
        {
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
    private void DialogAndMovementHandler()
    {


        if (currentImageIndex == 2)
        {

        }
    }

}
