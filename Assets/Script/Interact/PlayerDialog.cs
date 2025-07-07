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
    [HideInInspector]public bool isInteraction;
    [SerializeField] private NavMeshObstacle casket;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AfterAnimationPosition());
        pointAndMovement = GetComponent<PointAndMovement>();
        StartCoroutine(StartInteraction());
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
        
        yield return new WaitForSeconds(2);
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
        if(currentImageIndex < 2)
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
    private IEnumerator AfterAnimationPosition()
    {

        yield return new WaitForSeconds(5.30f);

        casket.enabled = true;
    }

}
