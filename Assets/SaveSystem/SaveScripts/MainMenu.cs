using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject LoadGameButton;
    void Start()
    {
        LoadGameButton.SetActive(SaveLoadManager.Instance.HasSavedGames);

    }

    public void NewGame()
    {
        SaveLoadManager.Instance.ClearSave();
        LoadMainLevel();
    }

    public void LoadMainLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SaveSystem");
    }
}
