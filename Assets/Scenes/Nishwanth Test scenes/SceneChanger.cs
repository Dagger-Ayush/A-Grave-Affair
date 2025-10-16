using UnityEngine;
using static SceneChanger;

public class SceneChanger : MonoBehaviour
{
    public enum DoorType { protoDoor, OutSideDoor,InsideDoor,NancyRoomDoor}
    public DoorType doortype;
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


    void Update()
    {
        //if (requiredPuzzle_1 != null && requiredPuzzle_1.isCompleted && requiredPuzzle_2 != null && requiredPuzzle_2.isCompleted)
        //{
        //    canChangeScene = true;
        //}
        switch (doortype)
        {
            case DoorType.protoDoor:
                bool puzzle1Complete = requiredPuzzle_1 == null || requiredPuzzle_1.isCompleted;
                bool puzzle2Complete = requiredPuzzle_2 == null || requiredPuzzle_2.isCompleted;

                canChangeScene = puzzle1Complete && puzzle2Complete;

                break;
            case DoorType.OutSideDoor:

                if (objectInteract != null && objectInteract.isInteractionComplete)
                {
                    canChangeScene = true;

                    if (door != null && !door.activeSelf)
                        door.SetActive(true);
                }
                break;
            case DoorType.InsideDoor:

                bool puzzle3Complete = requiredPuzzle_1 == null || requiredPuzzle_1.isCompleted;

                canChangeScene = puzzle3Complete;
                break;
            case DoorType.NancyRoomDoor:
                canChangeScene = true;

                break;
        
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
