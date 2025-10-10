using UnityEngine;

public class GameStartReset : MonoBehaviour
{
    void Awake()
    {
        PlayerPrefs.DeleteKey("MotelLobby_Phase3Complete");
        PlayerPrefs.Save();
    }
}
