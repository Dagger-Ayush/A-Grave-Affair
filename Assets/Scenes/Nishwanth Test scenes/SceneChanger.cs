using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public bool istrigger = false;
    [SerializeField] private CanvasGroup interactiImageIn;
    [SerializeField] private CanvasGroup interactiImageout;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private ObjectInteract objectInteract;
    public GameObject door;

    [Header("Scene Settings")]
    [SerializeField] private int targetSceneIndex;

    [Header("Puzzle Requirement")]
    public PuzzleData requiredPuzzle_1;
    public PuzzleData requiredPuzzle_2;
    private bool canChangeScene = false;

    void Start()
    {
    }

    void Update()
    {
        //if (requiredPuzzle_1 != null && requiredPuzzle_1.isCompleted && requiredPuzzle_2 != null && requiredPuzzle_2.isCompleted)
        //{
        //    canChangeScene = true;
        //}

        bool puzzle1Complete = requiredPuzzle_1 == null || requiredPuzzle_1.isCompleted;
        bool puzzle2Complete = requiredPuzzle_2 == null || requiredPuzzle_2.isCompleted;

        canChangeScene = puzzle1Complete && puzzle2Complete;

        if (objectInteract != null && objectInteract.isInteractionComplete)
        {
            canChangeScene = true;

            if (door != null && !door.activeSelf)
                door.SetActive(true);
        }
        if (!canChangeScene)
            return;

        if (playerInteract.SceneChangerHandler() == this && !istrigger)
        {
            if (interactiImageIn != null)
            {
                interactiImageIn.alpha = 1;
                interactiImageout.alpha = 0;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(LevelLoader.Instance.ChangeLevel(targetSceneIndex));
                if (interactiImageIn != null)
                {
                    interactiImageIn.alpha = 0;
                    interactiImageout.alpha = 0;
                }
            }
        }
        else
        {
            if (interactiImageIn != null && interactiImageout !=null)
            {
                interactiImageIn.alpha = 0;
                interactiImageout.alpha = 1;
            }
        }
    }
   
    private void OnTriggerEnter(Collider other)
    {
        if(!canChangeScene)
            return ;

        if (other.gameObject.CompareTag("Player")&& istrigger)
        {
            StartCoroutine(LevelLoader.Instance.ChangeLevel(targetSceneIndex));
        }
    }
}
