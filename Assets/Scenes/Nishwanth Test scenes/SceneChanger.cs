using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    public bool istrigger = false;
    [SerializeField] private CanvasGroup interactiImage;
    [SerializeField] private PlayerInteract playerInteract;

    [Header("Scene Settings")]
    [SerializeField] private int targetSceneIndex;

    [Header("Puzzle Requirement")]
    public PuzzleData requiredPuzzle;
    private bool canChangeScene = false;

    void Start()
    {
        if(requiredPuzzle == null)
        {
            canChangeScene = true;
        }
    }

    void Update()
    {
        if (requiredPuzzle != null && requiredPuzzle.isCompleted)
        {
            canChangeScene = true;
        }
        if(!canChangeScene)
            return;

        if (playerInteract.SceneChangerHandler() == this && !istrigger)
        {
            if (interactiImage != null)
            {
                interactiImage.alpha = 1;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(LevelLoader.Instance.ChangeLevel(targetSceneIndex));
                if (interactiImage != null)
                {
                    interactiImage.alpha = 0;
                }


            }
        }
        else
        {
            if (interactiImage != null)
            {
                interactiImage.alpha = 0;
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
