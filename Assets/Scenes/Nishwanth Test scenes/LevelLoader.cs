using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;
    public Animator animator;
    public float transitionTime = 1f;
    public int SceneNumber;

    private void Awake()
    {
        Instance = this;
    }
  
    private void LoadLevel()
    {
        SceneManager.LoadScene(SceneNumber);
    }

    public IEnumerator ChangeLevel()
    {
        animator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        LoadLevel();
    }
}
