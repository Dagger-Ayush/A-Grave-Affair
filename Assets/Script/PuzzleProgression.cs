using System.Collections;
using TMPro;
using UnityEngine;

public class PuzzleProgression : MonoBehaviour
{
    [Header("Puzzle Panel")]
    public GameObject puzzle1;
    public GameObject puzzle2;
    public GameObject puzzle3;
    public TMP_Text feedbackText;

    private bool puzzle1Solved = false;

    [SerializeField] private GameObject dummyObjectDialog;
    [SerializeField] private DialogAudio dialogAudio;

    private bool isDialogEnabled = false;
    private bool isDialogStarted = false;
    private bool isPuzzleCompleted = false;

    void Start()
    {
        puzzle1.SetActive(true);
        puzzle2.SetActive(false); 
        puzzle3.SetActive(false);
    }
    private void Update()
    {
        PuzzleCompleteDialog();
    }
    public void OnPuzzle1Solved()
    {
       
        if (puzzle1Solved) return;
        puzzle1Solved = true;
        EnablingObjects();
        StartCoroutine(CloseTabletAfterDelay());        
    }

    private IEnumerator CloseTabletAfterDelay()
    {
        Time.timeScale = 1f;

        yield return new WaitForSeconds(2f);


        if(TabletManager.Instance !=null)
        {
            TabletManager.Instance.CloseTablet();
          
           
        }

        yield return new WaitForSeconds(0.5f);
        
        puzzle1.SetActive(false);
        
        TabletManager.Instance.puzzlePanel.SetActive(false);
        TabletManager.Instance.clueBox.SetActive(false);
        feedbackText.text = " ";

        if (puzzle2 != null) puzzle2.SetActive(true);
        if(puzzle3 != null) puzzle3.SetActive(true);

        Debug.Log("Puzzle 2 & 3 unlocked.");
        isPuzzleCompleted = true;
    }

    private void EnablingObjects()
    {
        //FindAnyObjectByType<ObjectInteract>().shouldWork = true;
       // FindAnyObjectByType<ObjectPickHandler>().shouldWork = true;
        //FindAnyObjectByType<ObjectMoving>().shouldWork = true;

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
    }
    void PuzzleCompleteDialog()
    {
        if (!isDialogEnabled && isPuzzleCompleted)
        {
            if (!isDialogStarted)
            {
                dummyObjectDialog.SetActive(true);
                dialogAudio.sorce.Play();
                isDialogStarted = true;
            }
            else if (isDialogStarted && Input.GetKeyDown(KeyCode.E))
            {
                dummyObjectDialog.SetActive(false);
                dialogAudio.sorce.Stop();
                isDialogEnabled = true;


            }

        }

    }
}
