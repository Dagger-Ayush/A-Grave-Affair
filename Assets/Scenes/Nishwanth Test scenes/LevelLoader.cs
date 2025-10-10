using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    [Header("Transition Settings")]
    public Animator animator;
    public float transitionTime = 1f;
    //public int SceneNumber;

    [Header("Optional Spawn Setting")]
    public string targetSpawnID;

    private void Awake()
    {
        Instance = this;

        //if (Instance != null && Instance != this)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //Instance = this;
        //DontDestroyOnLoad(gameObject);
    }
  
    private void LoadLevel()
    {
        if(!string.IsNullOrEmpty(targetSpawnID))
        {
            SceneTransitionManager.targetSpawnID = targetSpawnID;
        }
        //SceneManager.LoadScene(sceneIndex);
    }

    public IEnumerator ChangeLevel(int sceneIndex)
    {
        
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        LoadLevel();
        SceneManager.LoadScene(sceneIndex);
    }
}
