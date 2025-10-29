using System.Collections;
using UnityEditor;
using UnityEngine;
using static SceneChanger;

public class SceneChanger : MonoBehaviour
{
    public enum DoorType { protoDoor, OutSideDoor,InsideDoor,NancyRoomDoor, EndScreenDoor}
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
    private bool canChangeScene = false;

    public GameObject gregObject;
    public GameObject endPage;

    private void Start()
    {
        // Check if the outside door was already entered
        if (doortype == DoorType.OutSideDoor)
            {
            if (PlayerPrefs.GetInt("EnteredOutsideDoor", 0) == 1)
            {
                canChangeScene = true;

                // Destroy gregObject if it exists
                if (gregObject != null)
                {
                    Destroy(gregObject);
                }

                if (GamePhaseManager.MotelLobbyPhase >= 9)
                {
                    if(door != null)
                    {
                        door.SetActive(false);
                    }
                    return;
                }
                    // Make door active if assigned
                    else if (door != null && !door.activeSelf)
                    {
                        door.SetActive(true);
                    }

                    Debug.Log("Outside door already entered: canChangeScene=true & gregObject destroyed");
                }
            }
    }
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

                canChangeScene = puzzle1Complete;

                break; 
            case DoorType.EndScreenDoor:
                bool puzzle1CompleteTest = requiredPuzzle_1 == null || requiredPuzzle_1.isCompleted;

                canChangeScene = puzzle1CompleteTest;

                break;
            case DoorType.OutSideDoor:
                if (objectInteract != null && objectInteract.isInteractionComplete)
                {
                    canChangeScene = true;

                    if (door != null && !door.activeSelf)
                        door.SetActive(true);

                    // --- Set PlayerPref and destroy gregObject once ---
                    if (PlayerPrefs.GetInt("EnteredOutsideDoor", 0) == 0)
                    {
                        PlayerPrefs.SetInt("EnteredOutsideDoor", 1); // 1 = true
                        PlayerPrefs.Save();

                        if (gregObject != null)
                            Destroy(gregObject);

                        Debug.Log("Entered outside door: PlayerPref saved & gregObject destroyed");
                    }
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

        if ((playerInteract != null&& playerInteract.SceneChangerHandler() == this) && !istrigger)
        {
            if (interactiImageIn != null)
            {
                interactiImageIn.alpha = 1;
                interactiImageout.alpha = 0;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (doortype != DoorType.EndScreenDoor)
                {
                    StartCoroutine(LevelLoader.Instance.ChangeLevel(targetSceneIndex));
                    if (interactiImageIn != null)
                    {
                        interactiImageIn.alpha = 0;
                        interactiImageout.alpha = 0;
                    }
                }
                else
                {
                    endPage.SetActive(true);
                    StartCoroutine(Exit());
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
    public IEnumerator Exit()
    {
        yield return new WaitForSeconds(2);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
