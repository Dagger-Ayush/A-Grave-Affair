using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] GameObject LoadGameButton;
    void Start()
    {
        LoadGameButton.SetActive(SaveLoadManager.Instance.HasSavedGames);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadMainLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SaveSystem");
    }
}
