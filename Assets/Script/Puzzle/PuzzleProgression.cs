using System.Collections;
using TMPro;
using UnityEngine;
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
    public TMP_Text feedbackText;

    private bool puzzle1Solved = false;
    private bool puzzle2Solved = false;
    private bool puzzle3Solved = false;

    private bool puzzle5Solved = false;

    [SerializeField] private GameObject dummyObjectDialog;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private DialogAudio dummyDialogAudio;
    [SerializeField] private PlayerInteract playerInteract;

    [HideInInspector] public bool isDialogEnabled = false;
    private bool isDialogStarted = false;
    private bool isPuzzleCompleted = false;

    void Start()
    {
        if (puzzle1 != null) puzzle1.SetActive(true);
        if (puzzle2 != null) puzzle2.SetActive(false);
        if (puzzle3 != null) puzzle3.SetActive(false);
        if (puzzle4 != null) puzzle4.SetActive(false);
        if (puzzle5 != null) puzzle5.SetActive(false);
        if (puzzle6 != null) puzzle6.SetActive(false);
        if (puzzle7 != null) puzzle7.SetActive(false);
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
        StartCoroutine(CloseTabletAfterDelay(() =>
        {
            puzzle1.SetActive(false);

            TabletManager.Instance.puzzlePanel.SetActive(false);
            TabletManager.Instance.clueBox.SetActive(false);
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

        if (puzzle5Solved) return;
        puzzle5Solved = true;
        StartCoroutine(CloseTabletAfterDelay(() =>
        {
            puzzle5.SetActive(false);

            TabletManager.Instance.puzzlePanel.SetActive(false);
            TabletManager.Instance.clueBox.SetActive(false);
            feedbackText.text = " ";

            if (puzzle6 != null) puzzle6.SetActive(true);
            if (puzzle7 != null) puzzle7.SetActive(true);

            Debug.Log("Puzzle 6 & 7 unlocked.");
            isPuzzleCompleted = true;
        }));
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
                //audioManager.PlayDialogBigLine(dummyDialogAudio);
                isDialogStarted = true;
            }
            else if (isDialogStarted && Input.GetKeyDown(KeyCode.E))
            {
                dummyObjectDialog.SetActive(false);
                //audioManager.Stop();
                isDialogEnabled = true;


            }

        }

    }
}
