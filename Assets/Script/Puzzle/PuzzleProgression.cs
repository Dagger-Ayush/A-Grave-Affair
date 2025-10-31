using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ObjectPickHandler;

public class PuzzleProgression : MonoBehaviour
{
    [Header("Puzzle Panel")]
    public GameObject puzzle1;
    public GameObject puzzle2;
    public GameObject puzzle3;
    public GameObject puzzle4;
    public GameObject puzzle5;
    public GameObject puzzle6;
    public GameObject puzzle7;
    public GameObject puzzle8;
    public TMP_Text feedbackText;

    public GlobalPuzzleStateData puzzleStateData;

    private bool puzzle1Solved = false;
    private bool puzzle2Solved = false;
    private bool puzzle3Solved = false;

    private bool puzzle5Solved = false;

    private bool puzzle6Solved = false;
    private bool puzzle7Solved = false;

    [SerializeField] private ObjectInteract dummyObjectDialog;
    [SerializeField] private ObjectInteract nancyRoomDialog1;
    [SerializeField] private ObjectInteract nancyRoomDialog2;
    [SerializeField] private ObjectInteract outsideMotelDialog;

    [SerializeField] private PlayerInteract playerInteract;

    private bool isDialogStarted = false;
    [SerializeField] private bool isPuzzleCompleted = false;

    [Header("To Be Continued UI")]
    [SerializeField] private GameObject toBeContinuedPanel;
    [SerializeField] private TMP_Text toBeContinuedText;

    //public event Action OnPuzzle6And7Completed;

    public event Action OnPuzzle6Completed;
    public event Action OnPuzzle7Completed;

    //bool trackPuzzle6and7 = false;

    void Start()
    {
        if (puzzle1 != null) puzzle1.SetActive(true);
        if (puzzle2 != null) puzzle2.SetActive(false);
        if (puzzle3 != null) puzzle3.SetActive(false);
        if (puzzle4 != null) puzzle4.SetActive(false);
        if (puzzle5 != null) puzzle5.SetActive(false);
        if (puzzle6 != null) puzzle6.SetActive(false);
        if (puzzle7 != null) puzzle7.SetActive(false);
        //if (puzzle8 != null) puzzle8.SetActive(false);

        // Automatically unlock puzzle 6 & 7 if puzzle 5 is missing
        if (puzzle5 == null)
        {
            if (puzzle6 != null) puzzle6.SetActive(true);
            if (puzzle7 != null) puzzle7.SetActive(true);

            Debug.Log("Puzzle 5 missing — unlocked Puzzle 6 & 7 directly at start.");
            //trackPuzzle6and7 = true;
        }

        if (puzzleStateData != null && puzzle6 != null && puzzle7 != null)
        {
            if (puzzleStateData.IsComplete(6))
            {
                puzzle6Solved = true;
                puzzle6?.SetActive(false);
            }
            if (puzzleStateData.IsComplete(7))
            {
                puzzle7Solved = true;
                puzzle7?.SetActive(false);
            }
        }
    }
    private void Update()
    {
        PuzzleCompleteDialog();
        EndDialog();
    }
    public void OnPuzzle1Solved()
    {

        if (puzzle1Solved) return;
        puzzle1Solved = true;
        EnablingObjects();
        StartCoroutine(CloseTabletAfterDelay(() =>
        {
            puzzle1.SetActive(false);

            TabletManager.Instance.puzzlePanel.SetActive(false);
            TabletManager.Instance.clueBox.SetActive(false);
            ClueBoxManager.Instance.RestoreClueBoxPosition();
            feedbackText.text = " ";

            if (puzzle2 != null) puzzle2.SetActive(true);
            if (puzzle3 != null) puzzle3.SetActive(true);

            Debug.Log("Puzzle 2 & 3 unlocked.");
            isPuzzleCompleted = true;
        }));
    }

    public void OnPuzzle2Solved()
    {
        if (puzzle2Solved) return;
        puzzle2Solved = true;
        Debug.Log("Puzzle 2 solved");
        CheckPuzzle2And3Completion();
    }

    public void OnPuzzle3Solved()
    {
        if (puzzle3Solved) return;
        puzzle3Solved = true;
        Debug.Log("Puzzle 3 solved");
        CheckPuzzle2And3Completion();
    }

    public void CheckPuzzle2And3Completion()
    {
        if (puzzle2Solved && puzzle3Solved)
        {
            StartCoroutine(CloseTabletAfterDelay(() =>
            {
                LetterEnabling();
                if (puzzle2 != null) puzzle2.SetActive(false);
                if (puzzle3 != null) puzzle3.SetActive(false);
                if (puzzle4 != null) puzzle4.SetActive(true);

                TabletManager.Instance.puzzlePanel.SetActive(false);
                TabletManager.Instance.clueBox.SetActive(false);
                feedbackText.text = " ";

                Debug.Log("Puzzle 4 unlocked, replacing Puzzle 2 & 3.");
            }));
        }
    }

    public void OnPuzzle5Solved()
    {
        // If puzzle5 exists, proceed as usual
        //if (puzzle5Solved) return;
        puzzle5Solved = true;
        if (puzzleStateData == null || puzzleStateData.IsComplete(5)) return;

        puzzleStateData.MarkComplete(5);

        StartCoroutine(CloseTabletAfterDelay(() =>
        {
            puzzle5.SetActive(false);

            TabletManager.Instance.puzzlePanel.SetActive(false);
            TabletManager.Instance.clueBox.SetActive(false);
            ClueBoxManager.Instance.RestoreClueBoxPosition();
            feedbackText.text = " ";

            if (puzzle6 != null) puzzle6.SetActive(true);
            if (puzzle7 != null) puzzle7.SetActive(true);

            Debug.Log("Puzzle 6 & 7 unlocked.");
            isPuzzleCompleted = true;
        }));
    }
    public void OnPuzzle6Solved()
    {
        //if (puzzle6Solved) return;

        if (puzzleStateData == null || puzzleStateData.IsComplete(6)) return;

        puzzleStateData.MarkComplete(6);

        puzzle6Solved = true;
        StartCoroutine(CloseTabletAfterDelay(() =>
        {
            TriggerNancyRoomDialog1();

            //if (puzzle6 != null) puzzle6.SetActive(false);
            //if (puzzle7 != null) puzzle7.SetActive(false);

            TabletManager.Instance.puzzlePanel.SetActive(false);
            TabletManager.Instance.clueBox.SetActive(false);
            ClueBoxManager.Instance.RestoreClueBoxPosition();
            feedbackText.text = " ";
            OnPuzzle6Completed?.Invoke();
        }));
        
        Debug.Log("Puzzle 6 solved");
        //CheckPuzzle6And7Completion();
    }

    public void OnPuzzle7Solved()
    {
        //if (puzzle7Solved) return;

        if (puzzleStateData == null || puzzleStateData.IsComplete(7)) return;

        puzzleStateData.MarkComplete(7);
        puzzle7Solved = true;
        StartCoroutine(CloseTabletAfterDelay(() =>
        {
            TriggerNancyRoomDialog2();

            //if (puzzle6 != null) puzzle6.SetActive(false);
            //if (puzzle7 != null) puzzle7.SetActive(false);

            TabletManager.Instance.puzzlePanel.SetActive(false);
            TabletManager.Instance.clueBox.SetActive(false);
            ClueBoxManager.Instance.RestoreClueBoxPosition();
            feedbackText.text = " ";
            OnPuzzle7Completed?.Invoke();
        }));
        
        Debug.Log("Puzzle 7 solved");
        
        //CheckPuzzle6And7Completion();
    }

    public void ActivatePuzzle8()
    {
        if(puzzle8 != null)
            puzzle8.SetActive(true);
    }

    public void OnPuzzle8Solved()
    {
        //if (puzzle6Solved) return;

        if (puzzleStateData == null || puzzleStateData.IsComplete(8)) return;

        puzzleStateData.MarkComplete(8);

        StartCoroutine(CloseTabletAfterDelay(() =>
        {
            OutsideMotelDialog();
            
            TabletManager.Instance.puzzlePanel.SetActive(false);
            TabletManager.Instance.clueBox.SetActive(false);
            ClueBoxManager.Instance.RestoreClueBoxPosition();
            feedbackText.text = " ";
            
        }));

        Debug.Log("Puzzle 8 solved");
    }

    private bool nancyDialog1Triggered = false;
    private bool nancyDialog2Triggered = false;

    private void TriggerNancyRoomDialog1()
    {
        if (nancyDialog1Triggered) return; // Prevent multiple triggers
        nancyDialog1Triggered = true;

        int sceneNumber = SceneManager.GetActiveScene().buildIndex;

        // Check if player is in Nancy's scene (scene 3 in this example)
        if (sceneNumber == 4 && nancyRoomDialog1 != null)
        {
            nancyRoomDialog1.enabled = true;
            GamePhaseManager.ONsentence1Complete = true;
            Debug.Log("Nancy Room Dialog triggered.");
            if(puzzle6Solved && puzzle7Solved)
            {
                GamePhaseManager.MotelLobbyPhase = 5;
            }
            
        }
    }
    private void TriggerNancyRoomDialog2()
    {
        if (nancyDialog2Triggered) return; // Prevent multiple triggers
        nancyDialog2Triggered = true;

        int sceneNumber = SceneManager.GetActiveScene().buildIndex;

        // Check if player is in Nancy's scene (scene 3 in this example)
        if (sceneNumber == 4 && nancyRoomDialog2 != null)
        {
            nancyRoomDialog2.enabled = true;
            GamePhaseManager.ONsentence2Complete = true;
            Debug.Log("Nancy Room Dialog triggered.");
            if (puzzle6Solved && puzzle7Solved)
            {
                GamePhaseManager.MotelLobbyPhase = 5;
            }

        }
    }

    private bool outsideMotelDialogTriggered = false;

    private void OutsideMotelDialog()
    {
        if(outsideMotelDialogTriggered) return;
        outsideMotelDialogTriggered = true;

        int sceneNumber = SceneManager.GetActiveScene().buildIndex;

        if(sceneNumber == 2 && outsideMotelDialog != null)
        {
            outsideMotelDialog.enabled = true;
            GamePhaseManager.MotelLobbyPhase = 11;
            Debug.Log("Outside Motel Dialog triggered.");
        }
    }

    private void EndDialog()
    {
        if (outsideMotelDialog != null && outsideMotelDialog.isAutoComplete)
        {
            ImageFade.instance.FadeInOut();
            if(toBeContinuedPanel != null && toBeContinuedText != null)
            {
                ShowToBeContinuedAfterFade();
            }
        }
    }
    private IEnumerator CloseTabletAfterDelay(System.Action onClose)
    {
        Time.timeScale = 1f;

        yield return new WaitForSeconds(2f);


        if (TabletManager.Instance != null)
        {
            TabletManager.Instance.CloseTablet();


        }

        yield return new WaitForSeconds(0.5f);

        onClose?.Invoke();
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



                if (objectPickHandler.type == InspectType.Letter_1 || objectPickHandler.type == InspectType.TutorialLetter)
                {
                    objectPickHandler.enabled = false;
                    objectPickHandler.shouldWork = false;

                }
            }
            if (collider.TryGetComponent(out ObjectMoving objectMoving))
            {
                objectMoving.shouldWork = true;

            }
        }
    }
    private void LetterEnabling()
    {

        Collider[] colliderArray = Physics.OverlapSphere(transform.position, 100);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ObjectPickHandler objectPickHandler))
            {

                if (objectPickHandler.type == InspectType.Letter_1)
                {
                    objectPickHandler.enabled = true;
                    objectPickHandler.shouldWork = true;
                }
                if (objectPickHandler.type == InspectType.TutorialLetter)
                {
                    objectPickHandler.enabled = true;
                    objectPickHandler.shouldWork = true;
                }

            }
        }

    }
    void PuzzleCompleteDialog()
    {  
        if (!isDialogStarted && isPuzzleCompleted && dummyObjectDialog != null)
            {
                dummyObjectDialog.enabled = true;
                isDialogStarted = true;
            }
      
    }
    public bool puzzle2and3Check()
    {
        if(puzzle2Solved && puzzle3Solved) return true;
        else return false;
    } 
    public bool puzzle6and7Check()
    {
        if(puzzle6Solved && puzzle7Solved) return true;
        else return false;
    } public bool InfoPanelEnable()
    {
        if(puzzle5Solved) return true;
        else return false;
    }

    public void ShowToBeContinuedAfterFade()
    {
        StartCoroutine(ShowAfterFadeRoutine());
    }

    private IEnumerator ShowAfterFadeRoutine()
    {
        // Wait for fade to finish — adjust duration to match your fade time
        yield return new WaitForSeconds(ImageFade.instance.fadeTime);

        if (PlayerInteract.Instance != null)
        {
            PlayerInteract.Instance.enabled = false; // disable the script
            if (PlayerInteract.Instance.tabletImage != null)
                PlayerInteract.Instance.tabletImage.enabled = false; // hide the tablet UI
        }

        // Now show the panel
        if (toBeContinuedPanel != null && toBeContinuedText != null)
        {
            toBeContinuedPanel.SetActive(true);
            toBeContinuedText.text = "To Be Continued...";
        }

        Debug.Log("Fade completed — showing To Be Continued message.");

        yield return new WaitForSeconds(3f);

        // Load the main menu scene
        SceneManager.LoadScene(0);
    }
}
